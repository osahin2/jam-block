using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public sealed class BlockSelection : ILevelSelection
    {
        private const string IceTooltip =
            "Ice locks the block until this many blocks have left the board, or a hammer or rocket breaks it off. 0: no ice.";

        private readonly BlockData _block;
        private readonly SelectionField[] _fields;
        private Cell _start;

        public IReadOnlyList<SelectionField> Fields => _fields;

        public BlockSelection(BlockData block)
        {
            _block = block;
            _fields = new[] { new SelectionField("Ice", IceTooltip, _ => _block.Ice, (_, ice) => LevelEdits.SetIce(_block, ice)) };
        }

        public bool Exists(LevelData level) => level.Blocks.Contains(_block);

        public int ColorId(LevelData level) => _block.ColorId;

        public string Describe(LevelData level)
        {
            var text = $"block of {_block.Shape.Count} cells";
            if (_block.Ice > 0)
                text += $", ice {_block.Ice}";
            if (_block.Arrow.IsSet)
                text += $", {_block.Arrow.Axis.ToString().ToLowerInvariant()} arrow of {_block.Arrow.Length}";
            return text;
        }

        public IEnumerable<Cell> Cells(LevelData level) => LevelEdits.BlockCells(level, level.Blocks.IndexOf(_block));

        public void BeginMove(LevelData level) => _start = _block.Position;

        public bool MoveBy(LevelData level, Cell offset) =>
            LevelEdits.TryMoveBlock(level, level.Blocks.IndexOf(_block), _start + offset);

        public void Recolor(LevelData level, int colorId) => _block.ColorId = colorId;

        public void Delete(LevelData level) => level.Blocks.Remove(_block);
    }
}
