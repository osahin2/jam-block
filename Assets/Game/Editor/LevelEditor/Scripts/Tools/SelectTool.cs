using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    // Picks a block or a door; dragging moves it. The window recolors or deletes the selection.
    public sealed class SelectTool : ILevelEditorTool
    {
        private Cell _grab;

        public ILevelSelection Selection { get; private set; }
        public string Name => "Select";
        public string Tooltip =>
            "Click a block or door to select it. Drag it to move it, click a color to recolor it, or delete it.";

        public string Describe(LevelData level, Cell cell)
        {
            if (LevelEdits.BlockAt(level, cell) != LevelEdits.NoBlock)
                return "Block: click to select it, drag to move it.";
            if (LevelEdits.DoorAt(level, cell) != LevelEdits.NoDoor)
                return "Door: click to select it, drag along the frame to move it.";
            return "Empty: click to clear the selection.";
        }

        public bool Press(LevelData level, Cell cell, int colorId)
        {
            Selection = SelectionAt(level, cell);
            _grab = cell;
            Selection?.BeginMove(level);
            return false;
        }

        public bool Drag(LevelData level, Cell cell) => Selection != null && Selection.MoveBy(level, cell - _grab);

        public void Release()
        {
        }

        public void Select(ILevelSelection selection) => Selection = selection;

        public void ClearSelection() => Selection = null;

        private static ILevelSelection SelectionAt(LevelData level, Cell cell)
        {
            var block = LevelEdits.BlockAt(level, cell);
            if (block != LevelEdits.NoBlock)
                return new BlockSelection(level.Blocks[block]);
            var door = LevelEdits.DoorAt(level, cell);
            return door != LevelEdits.NoDoor ? new DoorSelection(level.Doors[door]) : null;
        }
    }
}
