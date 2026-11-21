using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public sealed class BlockSelection : ILevelSelection
    {
        private readonly BlockData _block;
        private Cell _start;

        public BlockSelection(BlockData block)
        {
            _block = block;
        }

        public bool Exists(LevelData level) => level.Blocks.Contains(_block);

        public int ColorId(LevelData level) => _block.ColorId;

        public string Describe(LevelData level) => $"block of {_block.Shape.Count} cells";

        public IEnumerable<Cell> Cells(LevelData level) => LevelEdits.BlockCells(level, level.Blocks.IndexOf(_block));

        public void BeginMove(LevelData level) => _start = _block.Position;

        public bool MoveBy(LevelData level, Cell offset) =>
            LevelEdits.TryMoveBlock(level, level.Blocks.IndexOf(_block), _start + offset);

        public void Recolor(LevelData level, int colorId) => _block.ColorId = colorId;

        public void Delete(LevelData level) => level.Blocks.Remove(_block);
    }
}
