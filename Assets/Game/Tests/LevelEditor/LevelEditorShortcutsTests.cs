using System.Linq;
using Game.Editor.LevelEditor.Editing;
using Game.Editor.LevelEditor.Window;
using NUnit.Framework;
using UnityEditor.ShortcutManagement;

namespace Game.Tests.LevelEditor
{
    public sealed class LevelEditorShortcutsTests
    {
        // The window finds a tool's key by the tool's name; a renamed tool without a renamed shortcut breaks it.
        [Test]
        public void EveryTool_HasAShortcut()
        {
            var ids = ShortcutManager.instance.GetAvailableShortcutIds().ToList();
            ILevelEditorTool[] tools = { new SelectTool(), new BlockTool(), new DoorTool(), new EraseTool() };

            foreach (var tool in tools)
                CollectionAssert.Contains(ids, LevelEditorShortcuts.ToolId(tool.Name), tool.Name);
        }

        [Test]
        public void EveryColorKey_AndDelete_HaveAShortcut()
        {
            var ids = ShortcutManager.instance.GetAvailableShortcutIds().ToList();

            for (var i = 0; i < LevelEditorShortcuts.ColorKeyCount; i++)
                CollectionAssert.Contains(ids, LevelEditorShortcuts.ColorId(i));
            CollectionAssert.Contains(ids, LevelEditorShortcuts.DeleteId);
        }
    }
}
