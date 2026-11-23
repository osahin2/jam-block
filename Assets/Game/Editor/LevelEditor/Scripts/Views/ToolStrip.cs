using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    public sealed class ToolStrip
    {
        private const string SelectedClass = "tool-button--selected";

        public event Action<int> Chosen;

        private readonly VisualElement _buttons;

        public ToolStrip(VisualElement buttons, IReadOnlyList<ILevelEditorTool> tools)
        {
            _buttons = buttons;
            for (var i = 0; i < tools.Count; i++)
                _buttons.Add(ToolButton(i, tools[i]));
        }

        public void Show(int index)
        {
            var i = 0;
            foreach (var button in _buttons.Children())
                button.EnableInClassList(SelectedClass, i++ == index);
        }

        private Button ToolButton(int index, ILevelEditorTool tool)
        {
            var key = LevelEditorShortcuts.Key(LevelEditorShortcuts.ToolId(tool.Name));
            var button = new Button(() => Chosen?.Invoke(index)) { text = $"{tool.Name} ({key})", tooltip = tool.Tooltip };
            button.AddToClassList("tool-button");
            return button;
        }
    }
}
