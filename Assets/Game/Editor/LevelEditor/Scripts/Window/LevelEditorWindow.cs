using System;
using System.Collections.Generic;
using Game.Data.Levels;
using Game.Levels;
using Game.Logic.Levels;
using Game.Play;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    public sealed class LevelEditorWindow : EditorWindow
    {
        private const string Title = "Level Editor";
        private const string LayoutPath = "Assets/Game/Editor/LevelEditor/Layouts/LevelEditorWindow.uxml";
        private const string StylePath = "Assets/Game/Editor/LevelEditor/Styles/LevelEditorWindow.uss";
        private const string SelectedSwatchClass = "color-swatch--selected";
        private const string SelectedToolClass = "tool-button--selected";
        private const string SelectedClass = "selected";
        private const string FaultClass = "problem-fault";
        private const string FocusClass = "problem-focus";
        private const string SolutionFromClass = "solution-from";
        private const string SolutionToClass = "solution-to";
        private static readonly Vector2 LevelsWidthRange = new Vector2(140f, 400f);
        private static readonly Vector2 SideWidthRange = new Vector2(180f, 420f);
        private static readonly Vector2 BottomHeightRange = new Vector2(110f, 520f);

        [SerializeField] private LevelDocument _document = new LevelDocument();
        [SerializeField] private int _colorId;
        [SerializeField] private int _toolIndex;
        [SerializeField] private bool _levelsShown = true;
        [SerializeField] private float _levelsWidth = 180f;
        [SerializeField] private float _sideWidth = 230f;
        [SerializeField] private float _bottomHeight = 240f;

        private readonly SelectTool _selectTool = new SelectTool();
        private readonly List<BoardMark> _marks = new List<BoardMark>();
        private ILevelEditorTool[] _tools;
        private ColorPalette _palette;
        private LevelCatalog _catalog;
        private LevelLibrary _library;
        private SolvabilityCheck _solvability;
        private LevelBoardView _board;
        private Label _levelName;
        private LevelListPanel _levels;
        private IntegerField _width;
        private IntegerField _height;
        private Button _save;
        private Button _play;
        private Button _revert;
        private Button _duplicate;
        private Label _unsaved;
        private LevelProblemList _problems;
        private SolverPanel _solver;
        private Label _status;
        private Label _colorName;
        private Label _selectionLabel;
        private HelpBox _noLevel;
        private VisualElement _content;
        private VisualElement _colors;
        private VisualElement _toolButtons;
        private VisualElement _selectionPanel;
        private SelectionFieldsView _selectionFields;
        private ToolbarToggle _levelsToggle;
        private VisualElement _levelsPane;
        private VisualElement _levelsHandle;
        private VisualElement _side;
        private VisualElement _bottom;
        private LevelReport _report;
        private LevelSolution _solution;
        private Cell _hoverCell;
        private bool _hovering;
        private bool _stroking;

        private ILevelEditorTool Tool => _tools[_toolIndex];
        private ILevelSelection ShownSelection => Tool == _selectTool && !_solver.IsPlaying ? _selectTool.Selection : null;
        private LevelDraft Draft => _document.Draft;
        private string LevelName => _document.Name(_library);

        private void OnEnable()
        {
            _document.EnsureDraft();
            _palette = EditorAssets.FindFirst<ColorPalette>();
            _catalog = EditorAssets.FindFirst<LevelCatalog>();
            _library = _catalog != null ? new LevelLibrary(_catalog) : null;
            var solverSettings = EditorAssets.FindFirst<SolverSettings>();
            _solvability = solverSettings != null ? new SolvabilityCheck(solverSettings) : null;
            _tools = new ILevelEditorTool[] { _selectTool, new BlockTool(), new DoorTool(), new ArrowTool(), new IceTool(), new EraseTool() };
            _toolIndex = Mathf.Clamp(_toolIndex, 0, _tools.Length - 1);
            saveChangesMessage = "This level has unsaved changes. Save them?";
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnDisable() => Undo.undoRedoPerformed -= OnUndoRedo;

        private void OnDestroy()
        {
            LevelPreviewSource.ReleaseEdited();
            _document.Close();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LayoutPath).CloneTree(root);
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath));

            _levelName = root.Q<Label>("level");
            var newLevel = root.Q<Button>("new");
            newLevel.clicked += NewLevel;
            newLevel.SetEnabled(_library != null);
            _duplicate = root.Q<Button>("duplicate");
            _duplicate.clicked += DuplicateLevel;
            _save = root.Q<Button>("save");
            _save.clicked += () => TrySave();
            _play = root.Q<Button>("play");
            _play.clicked += Play;
            _revert = root.Q<Button>("revert");
            _revert.clicked += () => Open(_document.Asset);
            _unsaved = root.Q<Label>("unsaved");
            _noLevel = root.Q<HelpBox>("no-level");
            _content = root.Q("content");
            _status = root.Q<Label>("status");
            _colorName = root.Q<Label>("color-name");
            _selectionPanel = root.Q("selection");
            _selectionLabel = root.Q<Label>("selection-label");
            _selectionFields = new SelectionFieldsView(Edit);
            root.Q("selection-fields").Add(_selectionFields);
            var delete = root.Q<Button>("delete-selection");
            delete.clicked += DeleteSelection;
            delete.tooltip = $"Removes the selected block or door. Shortcut: {LevelEditorShortcuts.Key(LevelEditorShortcuts.DeleteId)}";

            _levelsPane = root.Q("levels");
            _levelsHandle = root.Q("levels-handle");
            BuildLevelList();
            _levelsToggle = root.Q<ToolbarToggle>("levels-toggle");
            _levelsToggle.tooltip = $"Shows or hides the level list, for more room for the board. Shortcut: {LevelEditorShortcuts.Key(LevelEditorShortcuts.LevelListId)}";
            _levelsToggle.RegisterValueChangedCallback(evt => ShowLevelList(evt.newValue));
            ShowLevelList(_levelsShown);
            _side = root.Q("side");
            _bottom = root.Q("bottom");
            SetLevelsWidth(_levelsWidth);
            SetSideWidth(_sideWidth);
            SetBottomHeight(_bottomHeight);
            _levelsHandle.AddManipulator(new PaneResizer(Vector2.right, () => _levelsWidth, SetLevelsWidth));
            root.Q("side-handle").AddManipulator(new PaneResizer(Vector2.right, () => _sideWidth, SetSideWidth));
            root.Q("bottom-handle").AddManipulator(new PaneResizer(Vector2.down, () => _bottomHeight, SetBottomHeight));

            _solver = new SolverPanel(BlockName);
            _solver.PlaybackChanged += OnPlaybackChanged;
            _solver.FullCheckRequested += OnFullCheckRequested;
            _bottom.Add(_solvability != null
                ? _solver
                : new HelpBox("No solver settings asset found, so the level is not checked for a solution.", HelpBoxMessageType.Error));
            _problems = new LevelProblemList(NameOf);
            _problems.FocusChanged += DrawBoard;
            _problems.Picked += SelectSubject;
            _bottom.Add(_problems);

            _width = root.Q<IntegerField>("width");
            _width.RegisterValueChangedCallback(evt => Resize(evt.newValue, Draft.Level.Height));
            _height = root.Q<IntegerField>("height");
            _height.RegisterValueChangedCallback(evt => Resize(Draft.Level.Width, evt.newValue));

            _toolButtons = root.Q("tools");
            BuildTools();
            _colors = root.Q("colors");
            BuildColors();

            _board = new LevelBoardView();
            _board.Pressed += OnPressed;
            _board.RightClicked += OnRightClicked;
            _board.Dragged += OnDragged;
            _board.Released += OnReleased;
            _board.Hovered += OnHovered;
            _board.HoverEnded += OnHoverEnded;
            root.Q("board-area").Add(_board);

            var draft = new SerializedObject(Draft);
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
            if (TrySave())
                base.SaveChanges();
        }

        internal void UseTool<T>() where T : ILevelEditorTool => ChooseTool(Array.FindIndex(_tools, tool => tool is T));

        internal void PickColor(int colorId)
        {
            if (_palette == null || !_palette.Has(colorId))
                return;
            ShowColor(colorId);
            var selection = ShownSelection;
            if (selection == null)
                return;
            Edit("Recolor", level => selection.Recolor(level, colorId));
        }

        internal void DeleteSelection()
        {
            var selection = ShownSelection;
            if (selection == null)
                return;
            Edit("Delete", level =>
            {
                selection.Delete(level);
                _selectTool.ClearSelection();
            });
        }

        internal void ToggleLevelList() => ShowLevelList(!_levelsShown);

        internal bool IsEditingText()
        {
            var focused = rootVisualElement.focusController?.focusedElement as VisualElement;
            return focused != null && (focused.GetFirstOfType<IntegerField>() != null || focused.GetFirstOfType<TextField>() != null);
        }

        private void Open(LevelAsset asset) => Load(() => _document.Open(asset));

        private void NewLevel() => Load(_document.StartNew);

        private void DuplicateLevel()
        {
            _document.Duplicate();
            _selectTool.ClearSelection();
            Refresh();
        }

        private void DuplicateLevel(LevelAsset level)
        {
            if (level == _document.Asset)
            {
                DuplicateLevel();
                return;
            }
            Load(() =>
            {
                _document.Open(level);
                _document.Duplicate();
            });
        }

        private void DeleteLevel(int index)
        {
            var level = _library.Levels[index];
            var question = level != null
                ? $"Delete {level.name}? It is removed from the game and its file goes to the trash."
                : "Remove this missing level from the list?";
            if (!EditorUtility.DisplayDialog(Title, question, level != null ? "Delete" : "Remove", "Cancel"))
                return;
            var deletesOpenLevel = level != null && level == _document.Asset;
            _library.Delete(index);
            if (deletesOpenLevel)
            {
                _document.Unload();
                _selectTool.ClearSelection();
            }
            Refresh();
        }

        private void Load(Action load)
        {
            if (!ConfirmDiscard())
                return;
            load();
            _selectTool.ClearSelection();
            Refresh();
        }

        private bool ConfirmDiscard() => !hasUnsavedChanges ||
            EditorUtility.DisplayDialog(Title, $"Discard the unsaved changes to {LevelName}?", "Discard", "Keep editing");

        private bool TrySave()
        {
            if (!ConfirmSolvable())
                return false;
            if (!_document.TrySave(_library))
            {
                EditorUtility.DisplayDialog(Title, $"{LevelName} was not saved: {WhyNotSaved()}", "OK");
                return false;
            }
            Refresh();
            return true;
        }

        private string WhyNotSaved() => _report != null && _report.HasErrors
            ? "it has errors. Fix them first."
            : "no level catalog was found to add it to.";

        private bool ConfirmSolvable() => _solution == null || _solution.Verdict != Solvability.Unsolvable ||
            EditorUtility.DisplayDialog(Title,
                $"{LevelName} cannot be solved: no order of moves gets every block out without boosters. Save it anyway?",
                "Save anyway", "Keep editing");

        private void Play()
        {
            if (TrySave() && !LevelPlayStarter.TryPlay(_document.Asset, out var problem))
                ShowNotification(new GUIContent(problem));
        }

        private void BuildLevelList()
        {
            if (_library == null)
            {
                _levelsPane.Add(new HelpBox("No level catalog found.", HelpBoxMessageType.Error));
                return;
            }
            _levels = new LevelListPanel(_library);
            _levels.Picked += Open;
            _levels.DuplicateRequested += DuplicateLevel;
            _levels.DeleteRequested += DeleteLevel;
            _levels.TrackSerializedObjectValue(new SerializedObject(_catalog), _ => _levels.Rebuild());
            _levelsPane.Add(_levels);
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
                var material = _palette.ForBlock(i);
                var key = i < LevelEditorShortcuts.ColorKeyCount ? LevelEditorShortcuts.Key(LevelEditorShortcuts.ColorId(i)) : "";
                var swatch = new Button(() => PickColor(colorId)) { tooltip = key.Length > 0 ? $"{ColorName(i)} ({key})" : ColorName(i) };
                swatch.AddToClassList("color-swatch");
                if (key.Length > 0)
                {
                    var badge = new Label(key);
                    badge.AddToClassList("color-swatch-key");
                    swatch.Add(badge);
                }
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

        private string ColorName(int colorId)
        {
            var material = _palette != null && _palette.Has(colorId) ? _palette.ForBlock(colorId) : null;
            return material != null ? material.name.Substring(material.name.LastIndexOf('_') + 1) : $"Missing color {colorId}";
        }

        private void ShowLevelList(bool shown)
        {
            _levelsShown = shown;
            _levelsToggle.SetValueWithoutNotify(shown);
            var display = shown ? DisplayStyle.Flex : DisplayStyle.None;
            _levelsPane.style.display = display;
            _levelsHandle.style.display = display;
        }

        private void SetLevelsWidth(float width)
        {
            _levelsWidth = Mathf.Clamp(width, LevelsWidthRange.x, LevelsWidthRange.y);
            _levelsPane.style.width = _levelsWidth;
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

        private void Resize(int width, int height) =>
            Edit("Resize Board", level => LevelEdits.Resize(level, Mathf.Max(1, width), Mathf.Max(1, height)));

        private void OnPressed(Cell cell)
        {
            Draft.BeginStroke();
            _stroking = true;
            Tool.Press(Draft.Level, cell, _colorId);
            Refresh();
        }

        private void OnDragged(Cell cell)
        {
            if (Tool.Drag(Draft.Level, cell))
                Refresh();
        }

        private void OnRightClicked(Cell cell)
        {
            Draft.BeginStroke();
            if (Tool.RightClick(Draft.Level, cell))
                Refresh();
            Draft.EndStroke(Tool.Name);
        }

        private void OnReleased()
        {
            Tool.Release();
            Draft.EndStroke(Tool.Name);
            _stroking = false;
            Refresh();
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

        private void OnUndoRedo()
        {
            _selectTool.ClearSelection();
            Refresh();
        }

        private void OnPlaybackChanged() => Refresh();

        private void OnFullCheckRequested()
        {
            _solvability.SearchFully(Draft.Level);
            Refresh();
        }

        private void UpdateStatus()
        {
            if (_solver.IsPlaying)
            {
                _status.text = "Showing the solution. Close it in the solver panel to edit the level.";
                return;
            }
            var hint = _hovering ? Tool.Describe(Draft.Level, _hoverCell) : Tool.Tooltip;
            var problem = _hovering ? ProblemAt(_hoverCell) : null;
            _status.text = problem == null ? $"{Tool.Name} tool: {hint}" : $"{Tool.Name} tool: {hint}\nProblem here: {problem}";
        }

        private string ProblemAt(Cell cell)
        {
            if (_report == null)
                return null;
            foreach (var problem in _report.Problems)
            {
                var subject = LevelProblemGuide.SubjectOf(problem, Draft.Level);
                if (subject == null)
                    continue;
                foreach (var subjectCell in subject.Cells(Draft.Level))
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
            _levels?.ShowOpen(_document.Asset);
            var hasLevel = _document.IsOpen;
            _noLevel.style.display = hasLevel ? DisplayStyle.None : DisplayStyle.Flex;
            _content.style.display = hasLevel ? DisplayStyle.Flex : DisplayStyle.None;
            if (!hasLevel)
            {
                UpdateToolbar(false);
                LevelPreviewSource.ReleaseEdited();
                return;
            }

            var level = Draft.Level;
            if (_selectTool.Selection != null && !_selectTool.Selection.Exists(level))
                _selectTool.ClearSelection();
            _width.SetValueWithoutNotify(level.Width);
            _height.SetValueWithoutNotify(level.Height);

            _report = LevelValidator.Validate(level);
            _problems.Show(_report, level);
            if (!_stroking)
                _solution = _report.HasErrors || _solvability == null ? null : _solvability.Solve(level);
            _solver.Show(_solution, level, _solvability != null && _solvability.SearchedFully);

            var selection = ShownSelection;
            _selectionPanel.style.display = selection != null ? DisplayStyle.Flex : DisplayStyle.None;
            if (selection != null)
                _selectionLabel.text = NameOf(selection);
            _selectionFields.Show(selection, level);
            DrawBoard();
            UpdateToolbar(!_report.HasErrors);
            UpdateStatus();
            LevelPreviewSource.ShowEdited(level);
        }

        private void Edit(string name, Action<LevelData> change)
        {
            Draft.Record(name);
            change(Draft.Level);
            Refresh();
        }

        private void UpdateToolbar(bool canSave)
        {
            _levelName.text = _document.IsOpen ? LevelName : "No level open";
            _save.SetEnabled(canSave);
            _play.SetEnabled(canSave);
            hasUnsavedChanges = _document.HasUnsavedChanges;
            _unsaved.style.display = hasUnsavedChanges ? DisplayStyle.Flex : DisplayStyle.None;
            _revert.SetEnabled(_document.CanRevert);
            _duplicate.SetEnabled(_document.IsOpen && _library != null);
        }

        private void DrawBoard()
        {
            _marks.Clear();
            _board.TakesEdits = !_solver.IsPlaying;
            if (_solver.IsPlaying)
            {
                DrawSolution();
                return;
            }
            var problems = _report.Problems;
            foreach (var problem in problems)
                MarkProblem(problem, $"problem--{LevelProblemList.SeverityName(problem.Severity)}");
            var selection = ShownSelection;
            if (selection != null)
                Mark(selection.Cells(Draft.Level), SelectedClass);
            if (_problems.Focused != LevelProblemList.NoProblem)
                MarkProblem(problems[_problems.Focused], FocusClass);
            _board.Show(Draft.Level, _palette, _marks);
        }

        private void DrawSolution()
        {
            if (_solver.ShowsNextMove)
            {
                Mark(_solver.NextBlockCells(), SolutionFromClass);
                Mark(_solver.NextTargetCells(), SolutionToClass);
            }
            _board.Show(_solver.PlayedLevel, _palette, _marks);
        }

        private void MarkProblem(LevelProblem problem, string className)
        {
            var subject = LevelProblemGuide.SubjectOf(problem, Draft.Level);
            if (subject != null)
                Mark(subject.Cells(Draft.Level), className);
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
            $"{ColorName(selection.ColorId(Draft.Level))} {selection.Describe(Draft.Level)}";

        private string BlockName(int block) => NameOf(new BlockSelection(Draft.Level.Blocks[block]));
    }
}
