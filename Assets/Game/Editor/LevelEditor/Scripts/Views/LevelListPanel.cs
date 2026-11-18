using System;
using System.Collections.Generic;
using Game.Levels;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    // The game's levels in play order, searched by play number or name. A click opens a level, dragging a row changes
    // the play order, and the right-click menu opens, duplicates, shows or deletes a level. The list draws only the rows
    // on screen, so a catalog of thousands of levels stays quick. What opening, duplicating and deleting do is the
    // window's; the panel reports them.
    public sealed class LevelListPanel : VisualElement
    {
        private const float RowHeight = 20f;
        private const string MissingName = "Missing level";
        private const string MissingClass = "level-row--missing";
        // How far, squared and in pixels, the pointer may move between press and release for a click; further is a drag.
        private const float ClickToleranceSqr = 16f;
        private static readonly int[] NoRows = Array.Empty<int>();

        public event Action<LevelAsset> Picked;
        public event Action<LevelAsset> DuplicateRequested;
        // The catalog index; the entry may have lost its asset.
        public event Action<int> DeleteRequested;

        private readonly LevelLibrary _library;
        private readonly ToolbarSearchField _search;
        private readonly ListView _list;
        // The catalog index of each row shown.
        private readonly List<int> _shown = new List<int>();
        private readonly int[] _selectedRow = new int[1];
        private LevelAsset _open;
        private Vector3 _pressPosition;

        public LevelListPanel(LevelLibrary library)
        {
            _library = library;
            AddToClassList("level-list");

            _search = new ToolbarSearchField { tooltip = "Find a level by its number in the play order or by its name." };
            _search.AddToClassList("level-search");
            _search.RegisterValueChangedCallback(_ => Rebuild());
            Add(_search);

            _list = new ListView(_shown, RowHeight, MakeRow, BindRow)
            {
                selectionType = SelectionType.Single,
                reorderMode = ListViewReorderMode.Animated,
                tooltip = "Click a level to open it. Drag a level to change the play order. Right-click for more."
            };
            _list.AddToClassList("level-rows");
            _list.itemIndexChanged += OnReordered;
            Add(_list);
            Rebuild();
        }

        // Marks the open level, none for a new level; scrolls to it when another level was opened.
        public void ShowOpen(LevelAsset open)
        {
            var changed = open != _open;
            _open = open;
            SelectOpen(changed);
        }

        // The catalog changed: its order, or a level added or removed.
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
            // A drag says where in the play order a level goes only while every entry is shown, in catalog order.
            _list.reorderable = _shown.Count == levels.Count;
            _list.Rebuild();
            SelectOpen(false);
        }

        private static string NameOf(LevelAsset level) => level != null ? level.name : MissingName;

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
            // Opening follows a click, not the selection: the list selects a row as soon as it is pressed, which is
            // also how a drag to reorder starts.
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

        // An entry whose asset was deleted outside the editor can only be taken out of the list.
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

        // A press and release in about one place; the dragged row also ends under the pointer, so a click event alone
        // would open a level after every drag. Clicking the open level again changes nothing; it would otherwise reload
        // it, which is what Revert does. The selection goes back to the open level afterwards, also when the window kept
        // the level that was open (the designer chose to keep editing) or the row was a missing level.
        private void OnRowClicked(ClickEvent evt, int index)
        {
            var level = _library.Levels[index];
            var isClick = (evt.position - _pressPosition).sqrMagnitude <= ClickToleranceSqr;
            if (isClick && level != null && level != _open)
                Picked?.Invoke(level);
            SelectOpen(false);
        }

        // Only while the list is not filtered, so the rows are the catalog in order and the indices match it.
        private void OnReordered(int from, int to)
        {
            _library.Move(from, to);
            Rebuild();
        }
    }
}
