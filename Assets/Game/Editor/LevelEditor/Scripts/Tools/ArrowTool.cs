using System;
using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public sealed class ArrowTool : ILevelEditorTool
    {
        public const int MaxLength = 3;

        private BlockData _block;
        private Cell _anchor;

        public string Name => "Arrow";
        public string Tooltip =>
            "Drag along a block's row or column to draw its arrow: the block will move only that way. Click a block to remove its arrow.";

        public string Describe(LevelData level, Cell cell)
        {
            var index = LevelEdits.BlockAt(level, cell);
            if (index == LevelEdits.NoBlock)
                return "Nothing to draw here. Arrows are drawn on blocks.";
            return level.Blocks[index].Arrow.IsSet
                ? "Block with an arrow: click to remove it, or drag along the block to draw a new one."
                : $"Block: drag along its row or column to draw an arrow, up to {MaxLength} cells. The block will move only that way.";
        }

        public bool Press(LevelData level, Cell cell, int colorId)
        {
            var index = LevelEdits.BlockAt(level, cell);
            if (index == LevelEdits.NoBlock)
            {
                _block = null;
                return false;
            }
            _block = level.Blocks[index];
            _anchor = cell - _block.Position;
            return ArrowEdits.Remove(_block);
        }

        public bool Drag(LevelData level, Cell cell)
        {
            if (_block == null)
                return false;
            var pull = cell - _block.Position - _anchor;
            if (pull == new Cell(0, 0))
                return ArrowEdits.Remove(_block);
            var axis = Math.Abs(pull.X) >= Math.Abs(pull.Y) ? ArrowAxis.Horizontal : ArrowAxis.Vertical;
            var end = FarthestBlockCellToward(axis, pull);
            var start = new Cell(Math.Min(_anchor.X, end.X), Math.Min(_anchor.Y, end.Y));
            var length = Math.Abs(end.X - _anchor.X) + Math.Abs(end.Y - _anchor.Y) + 1;
            return ArrowEdits.Set(_block, axis, start, length);
        }

        public void Release() => _block = null;

        public bool RightClick(LevelData level, Cell cell) => false;

        private Cell FarthestBlockCellToward(ArrowAxis axis, Cell pull)
        {
            var reach = axis == ArrowAxis.Horizontal ? pull.X : pull.Y;
            var step = axis == ArrowAxis.Horizontal ? new Cell(Math.Sign(reach), 0) : new Cell(0, Math.Sign(reach));
            var stepsAllowed = Math.Min(Math.Abs(reach), MaxLength - 1);
            var end = _anchor;
            for (var i = 0; i < stepsAllowed && _block.Shape.Contains(end + step); i++)
                end += step;
            return end;
        }
    }
}
