using System;
using Game.Data.Levels;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    public sealed class SelectionFieldsView : VisualElement
    {
        private readonly Action<string, Action<LevelData>> _edit;
        private ILevelSelection _shown;

        public SelectionFieldsView(Action<string, Action<LevelData>> edit)
        {
            _edit = edit;
        }

        public void Show(ILevelSelection selection, LevelData level)
        {
            if (selection != _shown)
                BuildInputsFor(selection);
            if (selection == null)
                return;
            foreach (var field in selection.Fields)
                field.Show(level);
        }

        private void BuildInputsFor(ILevelSelection selection)
        {
            _shown = selection;
            Clear();
            if (selection == null)
                return;
            foreach (var field in selection.Fields)
                Add(field.CreateInput(_edit));
        }
    }
}
