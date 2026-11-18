using Game.Data.Levels;

namespace Game.Editor.LevelEditor.Editing
{
    // One drag draws one block in the chosen color; a drag that starts on a block extends that block.
    public sealed class BlockTool : ILevelEditorTool
    {
        private int _block = LevelEdits.NoBlock;

        public string Name => "Block";
        public string Tooltip => "Drag over empty cells to draw a block. Start on a block to extend it.";

        public string Describe(LevelData level, Cell cell)
        {
            if (LevelEdits.BlockAt(level, cell) != LevelEdits.NoBlock)
                return "Block: drag from it onto empty cells to make it bigger.";
            return level.IsOnBoard(cell)
                ? "Empty cell: click, or drag over empty cells, to draw a new block in the selected color."
                : "Frame: nothing to draw here. Blocks are drawn inside the board.";
        }

        public bool Press(LevelData level, Cell cell, int colorId)
        {
            _block = LevelEdits.BlockAt(level, cell);
            if (_block != LevelEdits.NoBlock || !level.IsOnBoard(cell))
                return false;
            _block = LevelEdits.AddBlock(level, colorId, cell);
            return true;
        }

        public bool Drag(LevelData level, Cell cell) =>
            _block != LevelEdits.NoBlock && LevelEdits.TryGrowBlock(level, _block, cell);

        public void Release() => _block = LevelEdits.NoBlock;
    }
}
