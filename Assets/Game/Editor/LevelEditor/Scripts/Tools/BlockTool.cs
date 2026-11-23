using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public sealed class BlockTool : ILevelEditorTool
    {
        private int _block = LevelLookup.NoBlock;

        public string Name => "Block";
        public string Tooltip => "Drag over empty cells to draw a block. Start on a block to extend it.";

        public string Describe(LevelData level, Cell cell)
        {
            if (LevelLookup.BlockAt(level, cell) != LevelLookup.NoBlock)
                return "Block: drag from it onto empty cells to make it bigger.";
            return level.IsOnBoard(cell)
                ? "Empty cell: click, or drag over empty cells, to draw a new block in the selected color."
                : "Frame: nothing to draw here. Blocks are drawn inside the board.";
        }

        public bool Press(LevelData level, Cell cell, int colorId)
        {
            _block = LevelLookup.BlockAt(level, cell);
            return _block == LevelLookup.NoBlock && LevelEdits.TryAddBlock(level, colorId, cell, out _block);
        }

        public bool Drag(LevelData level, Cell cell) =>
            _block != LevelLookup.NoBlock && LevelEdits.TryGrowBlock(level, _block, cell);

        public void Release() => _block = LevelLookup.NoBlock;

        public bool RightClick(LevelData level, Cell cell) => false;
    }
}
