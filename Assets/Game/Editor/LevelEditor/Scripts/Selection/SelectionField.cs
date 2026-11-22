using System;
using Game.Data.Levels;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    public sealed class SelectionField
    {
        private readonly Func<LevelData, int> _get;
        private readonly Action<LevelData, int> _set;
        private IntegerField _input;

        public string Label { get; }
        public string Tooltip { get; }

        public SelectionField(string label, string tooltip, Func<LevelData, int> get, Action<LevelData, int> set)
        {
            Label = label;
            Tooltip = tooltip;
            _get = get;
            _set = set;
        }

        public VisualElement CreateInput(Action<string, Action<LevelData>> edit)
        {
            _input = new IntegerField(Label) { isDelayed = true, tooltip = Tooltip };
            _input.RegisterValueChangedCallback(evt => edit($"Set {Label}", level => Set(level, evt.newValue)));
            return _input;
        }

        public void Show(LevelData level) => _input?.SetValueWithoutNotify(Value(level));

        public int Value(LevelData level) => _get(level);

        public void Set(LevelData level, int value) => _set(level, value);
    }
}
