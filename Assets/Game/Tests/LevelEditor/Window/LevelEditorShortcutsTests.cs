using System.Linq;
using Game.Editor.LevelEditor;
using NUnit.Framework;
using UnityEditor.ShortcutManagement;

namespace Game.Tests.LevelEditor
{
    public sealed class LevelEditorShortcutsTests
    {
        [Test]
        public void EveryTool_HasAShortcut()
        {
            var ids = ShortcutManager.instance.GetAvailableShortcutIds().ToList();
            foreach (var tool in LevelEditorTools.Create(new SelectTool()))
                CollectionAssert.Contains(ids, LevelEditorShortcuts.ToolId(tool.Name), tool.Name);
        }

        [Test]
        public void EveryColorKey_Delete_AndTheLevelList_HaveAShortcut()
        {
            var ids = ShortcutManager.instance.GetAvailableShortcutIds().ToList();

            for (var i = 0; i < LevelEditorShortcuts.ColorKeyCount; i++)
                CollectionAssert.Contains(ids, LevelEditorShortcuts.ColorId(i));
            CollectionAssert.Contains(ids, LevelEditorShortcuts.DeleteId);
            CollectionAssert.Contains(ids, LevelEditorShortcuts.LevelListId);
        }
    }
}
