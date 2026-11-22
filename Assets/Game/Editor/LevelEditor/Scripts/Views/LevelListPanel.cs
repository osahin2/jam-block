using System;
using System.Collections.Generic;
using Game.Levels;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    public sealed class LevelListPanel : VisualElement
    {
        private const float RowHeight = 20f;
        private const string MissingName = "Missing level";
        private const string MissingClass = "level-row--missing";
        private const float ClickToleranceSqr = 16f;
        private static readonly int[] NoRows = Array.Empty<int>();

        public event Action<LevelAsset> Picked;
        public event Action<LevelAsset> DuplicateRequested;
        public event Action<int> DeleteRequested;

        private readonly LevelLibrary _library;
        private readonly ToolbarSearchField _search;
        private readonly ListView _list;
        private readonly List<int> _shown = new List<int>();
        private readonly int[] _selectedRow = new int[1];
        private LevelAsset _open;
        private Vector3 _pressPosition;

        public LevelListPanel(LevelLibrary library)
        {
            _library = library;
            AddToClassList("level-list");
            _search = CreateSearch();
            Add(_search);
            _list = CreateList();
            Add(_list);
            Rebuild();
        }

        public void ShowOpen(LevelAsset open)
        {
            var changed = open != _open;
            _open = open;
            SelectOpen(changed);
        }

        public void Rebuild()
        {
            _shown.Clear();
            var query = _search.value ?? string.Empty;
            var levels = _library.Levels;
            for (var i = 0; i < levels.Count; i++)
            {
                if (LevelSearch.Matches(i + 1, NameOf(levels[i]), query))
                    _shown.Add(i);
            }
            _list.reorderable = _shown.Count == levels.Count;
            _list.Rebuild();
            SelectOpen(false);
        }

        private static string NameOf(LevelAsset level) => level != null ? level.name : MissingName;

        private ToolbarSearchField CreateSearch()
        {
            var search = new ToolbarSearchField { tooltip = "Find a level by its number in the play order or by its name." };
            search.AddToClassList("level-search");
            search.RegisterValueChangedCallback(_ => Rebuild());
            return search;
        }

        private ListView CreateList()
        {
            var list = new ListView(_shown, RowHeight, MakeRow, BindRow)
            {
                selectionType = SelectionType.Single,
                reorderMode = ListViewReorderMode.Animated,
                tooltip = "Click a level to open it. Drag a level to change the play order. Right-click for more."
            };
            list.AddToClassList("level-rows");
            list.itemIndexChanged += OnReordered;
            return list;
        }

        private void SelectOpen(bool scroll)
        {
            var row = RowOf(_open);
            if (row < 0)
            {
                _list.SetSelectionWithoutNotify(NoRows);
                return;
            }
            _selectedRow[0] = row;
            _list.SetSelectionWithoutNotify(_selectedRow);
            if (scroll)
                _list.ScrollToItem(row);
        }

        private int RowOf(LevelAsset level)
        {
            if (level == null)
                return -1;
            for (var row = 0; row < _shown.Count; row++)
            {
                if (_library.Levels[_shown[row]] == level)
                    return row;
            }
            return -1;
        }

        private VisualElement MakeRow()
        {
            var row = new Label();
            row.AddToClassList("level-row");
            row.AddManipulator(new ContextualMenuManipulator(evt => FillMenu(evt, (int)row.userData)));
            row.RegisterCallback<PointerDownEvent>(evt => _pressPosition = evt.position, TrickleDown.TrickleDown);
            row.RegisterCallback<ClickEvent>(evt => OnRowClicked(evt, (int)row.userData));
            return row;
        }

        private void BindRow(VisualElement row, int rowIndex)
        {
            var index = _shown[rowIndex];
            var level = _library.Levels[index];
            row.userData = index;
            ((Label)row).text = $"{index + 1}. {NameOf(level)}";
            row.EnableInClassList(MissingClass, level == null);
        }

        private void FillMenu(ContextualMenuPopulateEvent evt, int index)
        {
            var level = _library.Levels[index];
            var status = level != null ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
            evt.menu.AppendAction("Open", _ => Picked?.Invoke(level), status);
            evt.menu.AppendAction("Duplicate", _ => DuplicateRequested?.Invoke(level), status);
            evt.menu.AppendAction("Show in Project", _ => EditorGUIUtility.PingObject(level), status);
            evt.menu.AppendSeparator();
            evt.menu.AppendAction(level != null ? "Delete..." : "Remove from the list", _ => DeleteRequested?.Invoke(index));
        }

        private void OnRowClicked(ClickEvent evt, int index)
        {
            var level = _library.Levels[index];
            var isClick = (evt.position - _pressPosition).sqrMagnitude <= ClickToleranceSqr;
            if (isClick && level != null && level != _open)
                Picked?.Invoke(level);
            SelectOpen(false);
        }

        private void OnReordered(int from, int to)
        {
            _library.Move(from, to);
            Rebuild();
        }
    }
}
