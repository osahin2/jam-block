using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public sealed class EraseTool : ILevelEditorTool
    {
        public string Name => "Erase";
        public string Tooltip => "Click or drag over block cells to erase them. Click a door to remove it.";

        public string Describe(LevelData level, Cell cell)
        {
            if (LevelEdits.BlockAt(level, cell) != LevelEdits.NoBlock)
                return "Block cell: click or drag to erase cells. A block cut in two is shown as an error.";
            if (LevelEdits.DoorAt(level, cell) != LevelEdits.NoDoor)
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
            var door = LevelEdits.DoorAt(level, cell);
            if (door == LevelEdits.NoDoor)
                return false;
            level.Doors.RemoveAt(door);
            return true;
        }
    }
}
