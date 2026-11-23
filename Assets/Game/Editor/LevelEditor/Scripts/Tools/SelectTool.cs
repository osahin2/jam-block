using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public sealed class SelectTool : ILevelEditorTool
    {
        private Cell _grab;

        public ILevelSelection Selection { get; private set; }
        public string Name => "Select";
        public string Tooltip =>
            "Click a block or door to select it. Drag it to move it, click a color to recolor it, or delete it.";

        public string Describe(LevelData level, Cell cell)
        {
            if (LevelLookup.BlockAt(level, cell) != LevelLookup.NoBlock)
                return "Block: click to select it, drag to move it.";
            if (LevelLookup.DoorAt(level, cell) != LevelLookup.NoDoor)
                return "Door: click to select it, drag along the frame to move it.";
            return "Empty: click to clear the selection.";
        }

        public bool Press(LevelData level, Cell cell, int colorId)
        {
            Selection = LevelSelections.At(level, cell);
            _grab = cell;
            Selection?.BeginMove(level);
            return false;
        }

        public bool Drag(LevelData level, Cell cell) => Selection != null && Selection.MoveBy(level, cell - _grab);

        public void Release()
        {
        }

        public bool RightClick(LevelData level, Cell cell) => false;

        public void Select(ILevelSelection selection) => Selection = selection;

        public void ClearSelection() => Selection = null;
    }
}
