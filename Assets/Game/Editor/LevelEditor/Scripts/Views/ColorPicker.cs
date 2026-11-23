using System;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    public sealed class ColorPicker
    {
        private const string SelectedClass = "color-swatch--selected";

        public event Action<int> Picked;

        private readonly VisualElement _swatches;
        private readonly Label _name;
        private readonly EditorPalette _palette;

        public ColorPicker(VisualElement swatches, Label name, EditorPalette palette)
        {
            _swatches = swatches;
            _name = name;
            _palette = palette;
            Build();
        }

        public void Show(int colorId)
        {
            var i = 0;
            foreach (var swatch in _swatches.Children())
                swatch.EnableInClassList(SelectedClass, i++ == colorId);
            _name.text = $"Selected: {_palette.ColorName(colorId)}";
        }

        private void Build()
        {
            if (!_palette.Exists)
            {
                _swatches.Add(new HelpBox("No color palette asset found.", HelpBoxMessageType.Error));
                return;
            }
            for (var i = 0; i < _palette.Count; i++)
                _swatches.Add(Swatch(i));
        }

        private Button Swatch(int colorId)
        {
            var key = ColorKey(colorId);
            var name = _palette.ColorName(colorId);
            var swatch = new Button(() => Picked?.Invoke(colorId)) { tooltip = key.Length > 0 ? $"{name} ({key})" : name };
            swatch.AddToClassList("color-swatch");
            if (key.Length > 0)
                swatch.Add(KeyBadge(key));
            swatch.style.backgroundColor = _palette.ColorOf(colorId);
            return swatch;
        }

        private static string ColorKey(int colorId) =>
            colorId < LevelEditorShortcuts.ColorKeyCount ? LevelEditorShortcuts.Key(LevelEditorShortcuts.ColorId(colorId)) : "";

        private static Label KeyBadge(string key)
        {
            var badge = new Label(key);
            badge.AddToClassList("color-swatch-key");
            return badge;
        }
    }
}
