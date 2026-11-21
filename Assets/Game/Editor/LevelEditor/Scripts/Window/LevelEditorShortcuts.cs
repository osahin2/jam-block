using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Game.Editor.LevelEditor
{
    public static class LevelEditorShortcuts
    {
        public const string DeleteId = Prefix + "Delete Selection";
        public const string LevelListId = Prefix + "Show or Hide Level List";
        // Colors 1 to 9 have keys; a larger palette's other colors are picked with the mouse.
        public const int ColorKeyCount = 9;
        private const string Prefix = "Level Editor/";

        // A tool's shortcut is found by its name, so renaming a tool means renaming its shortcut below.
        public static string ToolId(string toolName) => $"{Prefix}{toolName} Tool";

        // Colors are numbered from 1 for the keys, from 0 in the palette.
        public static string ColorId(int colorId) => $"{Prefix}Color {colorId + 1}";

        public static string Key(string shortcutId) => ShortcutManager.instance.GetShortcutBinding(shortcutId).ToString();

        [Shortcut(Prefix + "Select Tool", typeof(LevelEditorWindow), KeyCode.S)]
        private static void UseSelectTool(ShortcutArguments args) => Run(args, window => window.UseTool<SelectTool>());

        [Shortcut(Prefix + "Block Tool", typeof(LevelEditorWindow), KeyCode.B)]
        private static void UseBlockTool(ShortcutArguments args) => Run(args, window => window.UseTool<BlockTool>());

        [Shortcut(Prefix + "Door Tool", typeof(LevelEditorWindow), KeyCode.D)]
        private static void UseDoorTool(ShortcutArguments args) => Run(args, window => window.UseTool<DoorTool>());

        [Shortcut(Prefix + "Erase Tool", typeof(LevelEditorWindow), KeyCode.E)]
        private static void UseEraseTool(ShortcutArguments args) => Run(args, window => window.UseTool<EraseTool>());

        [Shortcut(DeleteId, typeof(LevelEditorWindow), KeyCode.Backspace)]
        private static void DeleteSelection(ShortcutArguments args) => Run(args, window => window.DeleteSelection());

        [Shortcut(LevelListId, typeof(LevelEditorWindow), KeyCode.L)]
        private static void ToggleLevelList(ShortcutArguments args) => Run(args, window => window.ToggleLevelList());

        [Shortcut(Prefix + "Color 1", typeof(LevelEditorWindow), KeyCode.Alpha1)]
        private static void Color1(ShortcutArguments args) => PickColor(args, 0);

        [Shortcut(Prefix + "Color 2", typeof(LevelEditorWindow), KeyCode.Alpha2)]
        private static void Color2(ShortcutArguments args) => PickColor(args, 1);

        [Shortcut(Prefix + "Color 3", typeof(LevelEditorWindow), KeyCode.Alpha3)]
        private static void Color3(ShortcutArguments args) => PickColor(args, 2);

        [Shortcut(Prefix + "Color 4", typeof(LevelEditorWindow), KeyCode.Alpha4)]
        private static void Color4(ShortcutArguments args) => PickColor(args, 3);

        [Shortcut(Prefix + "Color 5", typeof(LevelEditorWindow), KeyCode.Alpha5)]
        private static void Color5(ShortcutArguments args) => PickColor(args, 4);

        [Shortcut(Prefix + "Color 6", typeof(LevelEditorWindow), KeyCode.Alpha6)]
        private static void Color6(ShortcutArguments args) => PickColor(args, 5);

        [Shortcut(Prefix + "Color 7", typeof(LevelEditorWindow), KeyCode.Alpha7)]
        private static void Color7(ShortcutArguments args) => PickColor(args, 6);

        [Shortcut(Prefix + "Color 8", typeof(LevelEditorWindow), KeyCode.Alpha8)]
        private static void Color8(ShortcutArguments args) => PickColor(args, 7);

        [Shortcut(Prefix + "Color 9", typeof(LevelEditorWindow), KeyCode.Alpha9)]
        private static void Color9(ShortcutArguments args) => PickColor(args, 8);

        private static void PickColor(ShortcutArguments args, int colorId) => Run(args, window => window.PickColor(colorId));

        private static void Run(ShortcutArguments args, System.Action<LevelEditorWindow> action)
        {
            var window = (LevelEditorWindow)args.context;
            if (!window.IsEditingText())
                action(window);
        }
    }
}
