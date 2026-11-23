using System;
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

        [SerializeField] private LevelDocument _document = new LevelDocument();
        [SerializeField] private EditorPanes _panes = new EditorPanes();
        [SerializeField] private int _colorId;
        [SerializeField] private int _toolIndex;

        private readonly SelectTool _selectTool = new SelectTool();
        private readonly BoardMarks _marks = new BoardMarks();
        private ILevelEditorTool[] _tools;
        private EditorPalette _palette;
        private LevelCatalog _catalog;
        private LevelLibrary _library;
        private SolvabilityCheck _solvability;
        private LevelBoardView _board;
        private ToolStrip _toolStrip;
        private ColorPicker _colors;
        private LevelListPanel _levels;
        private LevelProblemList _problems;
        private SolverPanel _solver;
        private SelectionFieldsView _selectionFields;
        private Label _levelName;
        private IntegerField _width;
        private IntegerField _height;
        private Button _save;
        private Button _play;
        private Button _revert;
        private Button _duplicate;
        private Label _unsaved;
        private Label _status;
        private Label _selectionLabel;
        private HelpBox _noLevel;
        private VisualElement _content;
        private VisualElement _selectionPanel;
        private LevelReport _report;
        private LevelSolution _solution;
        private bool _stroking;

        private ILevelEditorTool Tool => _tools[_toolIndex];
        private ILevelSelection ShownSelection => Tool == _selectTool && !_solver.IsPlaying ? _selectTool.Selection : null;
        private LevelDraft Draft => _document.Draft;
        private string LevelName => _document.Name(_library);

        private void OnEnable()
        {
            _document.EnsureDraft();
            _palette = new EditorPalette(EditorAssets.FindFirst<ColorPalette>());
            _catalog = EditorAssets.FindFirst<LevelCatalog>();
            _library = _catalog != null ? new LevelLibrary(_catalog) : null;
            var solverSettings = EditorAssets.FindFirst<SolverSettings>();
            _solvability = solverSettings != null ? new SolvabilityCheck(solverSettings) : null;
            _tools = LevelEditorTools.Create(_selectTool);
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
            CreateToolbar(root);
            CreateSelectionPanel(root);
            _panes.Bind(root);
            CreateLevelList(root.Q("levels"));
            CreateSolverPanel(root.Q("bottom"));
            CreateProblemList(root.Q("bottom"));
            CreateBoardSize(root);
            CreateToolsAndColors(root);
            CreateBoard(root);
            BindDraft(root);
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
            if (!_palette.Has(colorId))
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

        internal void ToggleLevelList() => _panes.ToggleLevelList();

        internal bool IsEditingText()
        {
            var focused = rootVisualElement.focusController?.focusedElement as VisualElement;
            return focused != null && (focused.GetFirstOfType<IntegerField>() != null || focused.GetFirstOfType<TextField>() != null);
        }

        private void CreateToolbar(VisualElement root)
        {
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
        }

        private void CreateSelectionPanel(VisualElement root)
        {
            _selectionPanel = root.Q("selection");
            _selectionLabel = root.Q<Label>("selection-label");
            _selectionFields = new SelectionFieldsView(Edit);
            root.Q("selection-fields").Add(_selectionFields);
            var delete = root.Q<Button>("delete-selection");
            delete.clicked += DeleteSelection;
            delete.tooltip = $"Removes the selected block or door. Shortcut: {LevelEditorShortcuts.Key(LevelEditorShortcuts.DeleteId)}";
        }

        private void CreateLevelList(VisualElement pane)
        {
            if (_library == null)
            {
                pane.Add(new HelpBox("No level catalog found.", HelpBoxMessageType.Error));
                return;
            }
            _levels = new LevelListPanel(_library);
            _levels.Picked += Open;
            _levels.DuplicateRequested += DuplicateLevel;
            _levels.DeleteRequested += DeleteLevel;
            _levels.TrackSerializedObjectValue(new SerializedObject(_catalog), _ => _levels.Rebuild());
            pane.Add(_levels);
        }

        private void CreateSolverPanel(VisualElement pane)
        {
            _solver = new SolverPanel(_palette);
            _solver.PlaybackChanged += Refresh;
            _solver.FullCheckRequested += OnFullCheckRequested;
            pane.Add(_solvability != null
                ? _solver
                : new HelpBox("No solver settings asset found, so the level is not checked for a solution.", HelpBoxMessageType.Error));
        }

        private void CreateProblemList(VisualElement pane)
        {
            _problems = new LevelProblemList(_palette);
            _problems.FocusChanged += DrawBoard;
            _problems.Picked += SelectSubject;
            pane.Add(_problems);
        }

        private void CreateBoardSize(VisualElement root)
        {
            _width = root.Q<IntegerField>("width");
            _width.RegisterValueChangedCallback(evt => Resize(evt.newValue, Draft.Level.Height));
            _height = root.Q<IntegerField>("height");
            _height.RegisterValueChangedCallback(evt => Resize(Draft.Level.Width, evt.newValue));
        }

        private void CreateToolsAndColors(VisualElement root)
        {
            _toolStrip = new ToolStrip(root.Q("tools"), _tools);
            _toolStrip.Chosen += ChooseTool;
            ChooseTool(_toolIndex);
            _colors = new ColorPicker(root.Q("colors"), root.Q<Label>("color-name"), _palette);
            _colors.Picked += PickColor;
            if (_palette.Count > 0)
                ShowColor(Mathf.Clamp(_colorId, 0, _palette.Count - 1));
        }

        private void CreateBoard(VisualElement root)
        {
            _board = new LevelBoardView(new IBlockMechanicDrawing[] { new IceDrawing(), new ArrowDrawing() }, _palette);
            _board.Pressed += OnPressed;
            _board.RightClicked += OnRightClicked;
            _board.Dragged += OnDragged;
            _board.Released += OnReleased;
            _board.HoverChanged += UpdateStatus;
            root.Q("board-area").Add(_board);
        }

        private void BindDraft(VisualElement root)
        {
            var draft = new SerializedObject(Draft);
            root.Bind(draft);
            root.TrackSerializedObjectValue(draft, _ => Refresh());
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
            if (!_document.TrySave(_library, out var problem))
            {
                EditorUtility.DisplayDialog(Title, $"{LevelName} was not saved: {problem}", "OK");
                return false;
            }
            Refresh();
            return true;
        }

        private bool ConfirmSolvable() => _solution == null || _solution.Verdict != Solvability.Unsolvable ||
            EditorUtility.DisplayDialog(Title,
                $"{LevelName} cannot be solved: no order of moves gets every block out without boosters. Save it anyway?",
                "Save anyway", "Keep editing");

        private void Play()
        {
            if (TrySave() && !LevelPlayStarter.TryPlay(_document.Asset, out var problem))
                ShowNotification(new GUIContent(problem));
        }

        private void ChooseTool(int index)
        {
            if (index < 0)
                return;
            _toolIndex = index;
            _toolStrip.Show(index);
            Refresh();
        }

        private void ShowColor(int colorId)
        {
            _colorId = colorId;
            _colors.Show(colorId);
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

        private void OnUndoRedo()
        {
            _selectTool.ClearSelection();
            Refresh();
        }

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
            if (!_board.TryGetHoveredCell(out var cell) || _report == null)
            {
                _status.text = $"{Tool.Name} tool: {Tool.Tooltip}";
                return;
            }
            var hint = $"{Tool.Name} tool: {Tool.Describe(Draft.Level, cell)}";
            var problem = LevelProblemGuide.ExplainAt(_report, Draft.Level, cell);
            _status.text = problem == null ? hint : $"{hint}\nProblem here: {problem}";
        }

        private void Refresh()
        {
            if (_board == null)
                return;
            _levels?.ShowOpen(_document.Asset);
            var hasLevel = _document.IsOpen;
            _noLevel.style.display = hasLevel ? DisplayStyle.None : DisplayStyle.Flex;
            _content.style.display = hasLevel ? DisplayStyle.Flex : DisplayStyle.None;
            if (hasLevel)
                ShowLevel(Draft.Level);
            else
                ShowNoLevel();
        }

        private void ShowNoLevel()
        {
            UpdateToolbar(false);
            LevelPreviewSource.ReleaseEdited();
        }

        private void ShowLevel(LevelData level)
        {
            _width.SetValueWithoutNotify(level.Width);
            _height.SetValueWithoutNotify(level.Height);
            _report = LevelValidator.Validate(level);
            _problems.Show(_report, level);
            ShowSolution(level);
            ShowSelection(level);
            DrawBoard();
            UpdateToolbar(!_report.HasErrors);
            UpdateStatus();
            LevelPreviewSource.ShowEdited(level);
        }

        private void ShowSolution(LevelData level)
        {
            if (!_stroking)
                _solution = _report.HasErrors || _solvability == null ? null : _solvability.Solve(level);
            _solver.Show(_solution, level, _solvability != null && _solvability.SearchedFully);
        }

        private void ShowSelection(LevelData level)
        {
            if (_selectTool.Selection != null && !_selectTool.Selection.Exists(level))
                _selectTool.ClearSelection();
            var selection = ShownSelection;
            _selectionPanel.style.display = selection != null ? DisplayStyle.Flex : DisplayStyle.None;
            if (selection != null)
                _selectionLabel.text = _palette.NameOf(selection, level);
            _selectionFields.Show(selection, level);
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
                if (_solver.ShowsNextMove)
                    _marks.AddNextMove(_solver.NextBlockCells(), _solver.NextTargetCells());
                _board.Show(_solver.PlayedLevel, _marks.All);
                return;
            }
            var level = Draft.Level;
            _marks.AddProblems(_report, level);
            var selection = ShownSelection;
            if (selection != null)
                _marks.AddSelection(selection.Cells(level));
            if (_problems.Focused != LevelProblemList.NoProblem)
                _marks.AddFocus(_report.Problems[_problems.Focused], level);
            _board.Show(level, _marks.All);
        }

        private void SelectSubject(ILevelSelection subject)
        {
            UseTool<SelectTool>();
            _selectTool.Select(subject);
            Refresh();
        }
    }
}
