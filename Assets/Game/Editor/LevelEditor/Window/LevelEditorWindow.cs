using System;
using System.Collections.Generic;
using Game.Data.Levels;
using Game.Editor.LevelEditor.Editing;
using Game.Editor.LevelEditor.Preview;
using Game.Levels;
using Game.Logic.Levels;
using Game.Play.Settings;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor.Window
{
    // The level editor: shows a level's working copy, sends pointer input to the chosen tool and writes the copy back
    // to the asset on save. Its state is in serialized fields, so it survives script reloads with its undo history.
    public sealed class LevelEditorWindow : EditorWindow
    {
        private const string Title = "Level Editor";
        private const string LayoutPath = "Assets/Game/Editor/LevelEditor/Window/LevelEditorWindow.uxml";
        private const string StylePath = "Assets/Game/Editor/LevelEditor/Window/LevelEditorWindow.uss";
        private const string SelectedSwatchClass = "color-swatch--selected";
        private const string SelectedToolClass = "tool-button--selected";
        private const string SelectedClass = "selected";
        private const string FaultClass = "problem-fault";
        private const string FocusClass = "problem-focus";
        // Minimum and maximum, in pixels.
        private static readonly Vector2 SideWidthRange = new Vector2(180f, 420f);
        private static readonly Vector2 BottomHeightRange = new Vector2(110f, 520f);

        [SerializeField] private LevelAsset _asset;
        [SerializeField] private LevelDraft _draft;
        [SerializeField] private int _colorId;
        [SerializeField] private int _toolIndex;
        [SerializeField] private float _sideWidth = 230f;
        [SerializeField] private float _bottomHeight = 240f;

        private readonly SelectTool _selectTool = new SelectTool();
        private readonly List<BoardMark> _marks = new List<BoardMark>();
        private ILevelEditorTool[] _tools;
        private ColorPalette _palette;
        private LevelBoardView _board;
        private ObjectField _levelField;
        private IntegerField _width;
        private IntegerField _height;
        private Button _save;
        private Button _revert;
        private Label _unsaved;
        private LevelProblemList _problems;
        private Label _status;
        private Label _colorName;
        private Label _selectionLabel;
        private HelpBox _noLevel;
        private VisualElement _content;
        private VisualElement _colors;
        private VisualElement _toolButtons;
        private VisualElement _selectionPanel;
        private VisualElement _side;
        private VisualElement _bottom;
        private LevelReport _report;
        private Cell _hoverCell;
        private bool _hovering;

        private ILevelEditorTool Tool => _tools[_toolIndex];
        private ILevelSelection ShownSelection => Tool == _selectTool ? _selectTool.Selection : null;

        private void OnEnable()
        {
            if (_draft == null)
                RecoverDraft();
            _palette = EditorAssets.FindFirst<ColorPalette>();
            _tools = new ILevelEditorTool[] { _selectTool, new BlockTool(), new DoorTool(), new EraseTool() };
            _toolIndex = Mathf.Clamp(_toolIndex, 0, _tools.Length - 1);
            saveChangesMessage = "This level has unsaved changes. Save them?";
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnDisable() => Undo.undoRedoPerformed -= OnUndoRedo;

        private void OnDestroy()
        {
            LevelPreviewSource.ReleaseEdited();
            _draft.Discard();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LayoutPath).CloneTree(root);
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath));

            _levelField = root.Q<ObjectField>("level");
            _levelField.objectType = typeof(LevelAsset);
            _levelField.SetValueWithoutNotify(_asset);
            _levelField.RegisterValueChangedCallback(evt => Open(evt.newValue as LevelAsset));
            _save = root.Q<Button>("save");
            _save.clicked += () => TrySave();
            _revert = root.Q<Button>("revert");
            _revert.clicked += () => Open(_asset);
            _unsaved = root.Q<Label>("unsaved");
            _noLevel = root.Q<HelpBox>("no-level");
            _content = root.Q("content");
            _status = root.Q<Label>("status");
            _colorName = root.Q<Label>("color-name");
            _selectionPanel = root.Q("selection");
            _selectionLabel = root.Q<Label>("selection-label");
            var delete = root.Q<Button>("delete-selection");
            delete.clicked += DeleteSelection;
            delete.tooltip = $"Removes the selected block or door. Shortcut: {LevelEditorShortcuts.Key(LevelEditorShortcuts.DeleteId)}";

            _side = root.Q("side");
            _bottom = root.Q("bottom");
            SetSideWidth(_sideWidth);
            SetBottomHeight(_bottomHeight);
            root.Q("side-handle").AddManipulator(new PaneResizer(Vector2.right, () => _sideWidth, SetSideWidth));
            root.Q("bottom-handle").AddManipulator(new PaneResizer(Vector2.down, () => _bottomHeight, SetBottomHeight));

            _problems = new LevelProblemList(NameOf);
            _problems.FocusChanged += DrawBoard;
            _problems.Picked += SelectSubject;
            _bottom.Add(_problems);

            _width = root.Q<IntegerField>("width");
            _width.RegisterValueChangedCallback(evt => Resize(evt.newValue, _draft.Level.Height));
            _height = root.Q<IntegerField>("height");
            _height.RegisterValueChangedCallback(evt => Resize(_draft.Level.Width, evt.newValue));

            _toolButtons = root.Q("tools");
            BuildTools();
            _colors = root.Q("colors");
            BuildColors();

            _board = new LevelBoardView();
            _board.Pressed += OnPressed;
            _board.Dragged += OnDragged;
            _board.Released += OnReleased;
            _board.Hovered += OnHovered;
            _board.HoverEnded += OnHoverEnded;
            root.Q("board-area").Add(_board);

            // Undo, redo and the bound time field change the draft outside the tools; this brings the view along.
            var draft = new SerializedObject(_draft);
            root.Bind(draft);
            root.TrackSerializedObjectValue(draft, _ => Refresh());
            Refresh();
        }

        [MenuItem("Game/Level Editor")]
        public static void OpenWindow() => GetWindow<LevelEditorWindow>(Title);

        [OnOpenAsset]
        public static bool OpenLevelAsset(int instanceId, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceId) is not LevelAsset asset)
                return false;
            GetWindow<LevelEditorWindow>(Title).Open(asset);
            return true;
        }

        public override void SaveChanges()
        {
            if (!TrySave())
            {
                EditorUtility.DisplayDialog(Title, $"{_asset.name} has errors and was not saved. Fix them first.", "OK");
                return;
            }
            base.SaveChanges();
        }

        internal void UseTool<T>() where T : ILevelEditorTool => ChooseTool(Array.FindIndex(_tools, tool => tool is T));

        // Picks the color for new blocks and doors; with a selection, recolors it too.
        internal void PickColor(int colorId)
        {
            if (_palette == null || !_palette.Has(colorId))
                return;
            ShowColor(colorId);
            var selection = ShownSelection;
            if (selection == null)
                return;
            _draft.Record("Recolor");
            selection.Recolor(_draft.Level, colorId);
            Refresh();
        }

        internal void DeleteSelection()
        {
            var selection = ShownSelection;
            if (selection == null)
                return;
            _draft.Record("Delete");
            selection.Delete(_draft.Level);
            _selectTool.ClearSelection();
            Refresh();
        }

        // Shortcuts stay quiet while a number is typed into a field.
        internal bool IsEditingText()
        {
            var focused = rootVisualElement.focusController?.focusedElement as VisualElement;
            return focused != null && (focused is IntegerField || focused.GetFirstAncestorOfType<IntegerField>() != null);
        }

        // The working copy is never saved: after the editor restarts the window remembers its level but not the copy,
        // so the level is loaded again from its asset.
        private void RecoverDraft()
        {
            _draft = LevelDraft.Create();
            if (_asset != null)
                _draft.Load(_asset.Level);
        }

        // Every way of opening a level comes here, so unsaved changes are never dropped without asking. Opening the level
        // that is already open loads it again from its asset, which is how Revert works; clearing the field keeps the
        // working copy.
        private void Open(LevelAsset asset)
        {
            if (asset != null && ConfirmDiscard())
            {
                _asset = asset;
                _draft.Load(asset.Level);
                _selectTool.ClearSelection();
            }
            _levelField?.SetValueWithoutNotify(_asset);
            Refresh();
        }

        private bool ConfirmDiscard() => !hasUnsavedChanges ||
            EditorUtility.DisplayDialog(Title, $"Discard the unsaved changes to {_asset.name}?", "Discard", "Keep editing");

        private bool TrySave()
        {
            if (_asset == null || LevelValidator.Validate(_draft.Level).HasErrors)
                return false;
            _draft.WriteTo(_asset.Level);
            EditorUtility.SetDirty(_asset);
            AssetDatabase.SaveAssetIfDirty(_asset);
            Refresh();
            return true;
        }

        private void BuildTools()
        {
            for (var i = 0; i < _tools.Length; i++)
            {
                var index = i;
                var tool = _tools[i];
                var key = LevelEditorShortcuts.Key(LevelEditorShortcuts.ToolId(tool.Name));
                var button = new Button(() => ChooseTool(index)) { text = $"{tool.Name} ({key})", tooltip = tool.Tooltip };
                button.AddToClassList("tool-button");
                _toolButtons.Add(button);
            }
            ChooseTool(_toolIndex);
        }

        private void ChooseTool(int index)
        {
            if (index < 0)
                return;
            _toolIndex = index;
            var i = 0;
            foreach (var button in _toolButtons.Children())
                button.EnableInClassList(SelectedToolClass, i++ == index);
            Refresh();
        }

        private void BuildColors()
        {
            if (_palette == null)
            {
                _colors.Add(new HelpBox("No color palette asset found.", HelpBoxMessageType.Error));
                return;
            }
            for (var i = 0; i < _palette.Count; i++)
            {
                var colorId = i;
                var material = _palette.For(i);
                var key = i < LevelEditorShortcuts.ColorKeyCount ? $" ({LevelEditorShortcuts.Key(LevelEditorShortcuts.ColorId(i))})" : "";
                var swatch = new Button(() => PickColor(colorId)) { tooltip = ColorName(i) + key };
                swatch.AddToClassList("color-swatch");
                swatch.style.backgroundColor = material != null ? material.color : Color.magenta;
                _colors.Add(swatch);
            }
            ShowColor(Mathf.Clamp(_colorId, 0, _palette.Count - 1));
        }

        private void ShowColor(int colorId)
        {
            _colorId = colorId;
            var i = 0;
            foreach (var swatch in _colors.Children())
                swatch.EnableInClassList(SelectedSwatchClass, i++ == colorId);
            _colorName.text = $"Selected: {ColorName(colorId)}";
        }

        // The palette's material names end in the color ("Block_Red").
        private string ColorName(int colorId)
        {
            var material = _palette != null && _palette.Has(colorId) ? _palette.For(colorId) : null;
            return material != null ? material.name.Substring(material.name.LastIndexOf('_') + 1) : $"Missing color {colorId}";
        }

        private void SetSideWidth(float width)
        {
            _sideWidth = Mathf.Clamp(width, SideWidthRange.x, SideWidthRange.y);
            _side.style.width = _sideWidth;
        }

        private void SetBottomHeight(float height)
        {
            _bottomHeight = Mathf.Clamp(height, BottomHeightRange.x, BottomHeightRange.y);
            _bottom.style.height = _bottomHeight;
        }

        private void Resize(int width, int height)
        {
            _draft.Record("Resize Board");
            LevelEdits.Resize(_draft.Level, Mathf.Max(1, width), Mathf.Max(1, height));
            Refresh();
        }

        private void OnPressed(Cell cell)
        {
            _draft.BeginStroke();
            Tool.Press(_draft.Level, cell, _colorId);
            // Refreshed even when the level did not change: a press can change the selection.
            Refresh();
        }

        private void OnDragged(Cell cell)
        {
            if (Tool.Drag(_draft.Level, cell))
                Refresh();
        }

        private void OnReleased()
        {
            Tool.Release();
            _draft.EndStroke(Tool.Name);
        }

        private void OnHovered(Cell cell)
        {
            _hovering = true;
            _hoverCell = cell;
            UpdateStatus();
        }

        private void OnHoverEnded()
        {
            _hovering = false;
            UpdateStatus();
        }

        // Undo may restore the level as new block and door objects, which a selection no longer holds; it is dropped.
        private void OnUndoRedo()
        {
            _selectTool.ClearSelection();
            Refresh();
        }

        // What the selected tool would do under the pointer, or what it does at all when the pointer is off the board,
        // and what is wrong with the block or door under the pointer.
        private void UpdateStatus()
        {
            var hint = _hovering ? Tool.Describe(_draft.Level, _hoverCell) : Tool.Tooltip;
            var problem = _hovering ? ProblemAt(_hoverCell) : null;
            _status.text = problem == null ? $"{Tool.Name} tool: {hint}" : $"{Tool.Name} tool: {hint}\nProblem here: {problem}";
        }

        private string ProblemAt(Cell cell)
        {
            if (_report == null)
                return null;
            foreach (var problem in _report.Problems)
            {
                var subject = LevelProblemGuide.SubjectOf(problem, _draft.Level);
                if (subject == null)
                    continue;
                foreach (var subjectCell in subject.Cells(_draft.Level))
                {
                    if (subjectCell == cell)
                        return LevelProblemGuide.Explain(problem.Kind);
                }
            }
            return null;
        }

        private void Refresh()
        {
            if (_board == null)
                return;
            var hasLevel = _asset != null;
            _noLevel.style.display = hasLevel ? DisplayStyle.None : DisplayStyle.Flex;
            _content.style.display = hasLevel ? DisplayStyle.Flex : DisplayStyle.None;
            if (!hasLevel)
                return;

            var level = _draft.Level;
            if (_selectTool.Selection != null && !_selectTool.Selection.Exists(level))
                _selectTool.ClearSelection();
            _width.SetValueWithoutNotify(level.Width);
            _height.SetValueWithoutNotify(level.Height);
            var selection = ShownSelection;
            _selectionPanel.style.display = selection != null ? DisplayStyle.Flex : DisplayStyle.None;
            if (selection != null)
                _selectionLabel.text = NameOf(selection);

            _report = LevelValidator.Validate(level);
            _problems.Show(_report, level);
            DrawBoard();
            _save.SetEnabled(!_report.HasErrors);
            hasUnsavedChanges = !_draft.Matches(_asset.Level);
            _unsaved.style.display = hasUnsavedChanges ? DisplayStyle.Flex : DisplayStyle.None;
            _revert.SetEnabled(hasUnsavedChanges);
            UpdateStatus();
            LevelPreviewSource.ShowEdited(level);
        }

        // Every problem's block or door in its severity's color, the cell at fault stronger; the selection; and the
        // problem under the pointer in the list over everything.
        private void DrawBoard()
        {
            _marks.Clear();
            var problems = _report.Problems;
            foreach (var problem in problems)
                MarkProblem(problem, $"problem--{LevelProblemList.SeverityName(problem.Severity)}");
            var selection = ShownSelection;
            if (selection != null)
                Mark(selection.Cells(_draft.Level), SelectedClass);
            if (_problems.Focused != LevelProblemList.NoProblem)
                MarkProblem(problems[_problems.Focused], FocusClass);
            _board.Show(_draft.Level, _palette, _marks);
        }

        private void MarkProblem(LevelProblem problem, string className)
        {
            var subject = LevelProblemGuide.SubjectOf(problem, _draft.Level);
            if (subject != null)
                Mark(subject.Cells(_draft.Level), className);
            if (problem.Cell.HasValue)
                _marks.Add(new BoardMark(problem.Cell.Value, FaultClass));
        }

        private void Mark(IEnumerable<Cell> cells, string className)
        {
            foreach (var cell in cells)
                _marks.Add(new BoardMark(cell, className));
        }

        private void SelectSubject(ILevelSelection subject)
        {
            UseTool<SelectTool>();
            _selectTool.Select(subject);
            Refresh();
        }

        private string NameOf(ILevelSelection selection) =>
            $"{ColorName(selection.ColorId(_draft.Level))} {selection.Describe(_draft.Level)}";
    }
}
