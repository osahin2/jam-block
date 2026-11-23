using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public sealed class EraseTool : ILevelEditorTool
    {
        public string Name => "Erase";
        public string Tooltip => "Click or drag over block cells to erase them. Click a door to remove it.";

        public string Describe(LevelData level, Cell cell)
        {
            if (LevelLookup.BlockAt(level, cell) != LevelLookup.NoBlock)
                return "Block cell: click or drag to erase cells. A block cut in two is shown as an error.";
            if (LevelLookup.DoorAt(level, cell) != LevelLookup.NoDoor)
                return "Door: click to remove it.";
            return "Nothing to erase here.";
        }

        public bool Press(LevelData level, Cell cell, int colorId) => Erase(level, cell);

        public bool Drag(LevelData level, Cell cell) => Erase(level, cell);

        public void Release()
        {
        }

        public bool RightClick(LevelData level, Cell cell) => false;

        private static bool Erase(LevelData level, Cell cell)
        {
            if (LevelEdits.TryRemoveBlockCell(level, cell))
                return true;
            var door = LevelLookup.DoorAt(level, cell);
            if (door == LevelLookup.NoDoor)
                return false;
            LevelEdits.RemoveDoor(level, level.Doors[door]);
            return true;
        }
    }
}
