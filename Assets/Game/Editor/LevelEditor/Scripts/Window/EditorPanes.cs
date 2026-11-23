using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    [Serializable]
    public sealed class EditorPanes
    {
        private static readonly Vector2 LevelsWidthRange = new Vector2(140f, 400f);
        private static readonly Vector2 SideWidthRange = new Vector2(180f, 420f);
        private static readonly Vector2 BottomHeightRange = new Vector2(110f, 520f);

        [SerializeField] private bool _levelsShown = true;
        [SerializeField] private float _levelsWidth = 180f;
        [SerializeField] private float _sideWidth = 230f;
        [SerializeField] private float _bottomHeight = 240f;

        private VisualElement _levels;
        private VisualElement _levelsHandle;
        private VisualElement _side;
        private VisualElement _bottom;
        private ToolbarToggle _levelsToggle;

        public void Bind(VisualElement root)
        {
            _levels = root.Q("levels");
            _levelsHandle = root.Q("levels-handle");
            _side = root.Q("side");
            _bottom = root.Q("bottom");
            SetLevelsWidth(_levelsWidth);
            SetSideWidth(_sideWidth);
            SetBottomHeight(_bottomHeight);
            _levelsHandle.AddManipulator(new PaneResizer(Vector2.right, () => _levelsWidth, SetLevelsWidth));
            root.Q("side-handle").AddManipulator(new PaneResizer(Vector2.right, () => _sideWidth, SetSideWidth));
            root.Q("bottom-handle").AddManipulator(new PaneResizer(Vector2.down, () => _bottomHeight, SetBottomHeight));
            BindLevelListToggle(root);
        }

        public void ToggleLevelList() => ShowLevelList(!_levelsShown);

        private void BindLevelListToggle(VisualElement root)
        {
            _levelsToggle = root.Q<ToolbarToggle>("levels-toggle");
            _levelsToggle.tooltip = "Shows or hides the level list, for more room for the board. " +
                                    $"Shortcut: {LevelEditorShortcuts.Key(LevelEditorShortcuts.LevelListId)}";
            _levelsToggle.RegisterValueChangedCallback(evt => ShowLevelList(evt.newValue));
            ShowLevelList(_levelsShown);
        }

        private void ShowLevelList(bool shown)
        {
            _levelsShown = shown;
            _levelsToggle.SetValueWithoutNotify(shown);
            var display = shown ? DisplayStyle.Flex : DisplayStyle.None;
            _levels.style.display = display;
            _levelsHandle.style.display = display;
        }

        private void SetLevelsWidth(float width)
        {
            _levelsWidth = Mathf.Clamp(width, LevelsWidthRange.x, LevelsWidthRange.y);
            _levels.style.width = _levelsWidth;
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
    }
}
