namespace Game.Editor.LevelEditor
{
    public static class LevelEditorTools
    {
        public static ILevelEditorTool[] Create(SelectTool select) =>
            new ILevelEditorTool[] { select, new BlockTool(), new DoorTool(), new ArrowTool(), new IceTool(), new EraseTool() };
    }
}
