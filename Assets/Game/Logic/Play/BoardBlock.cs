using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class BoardBlock
    {
        private readonly Cell[] _shape;

        public int ColorId { get; }
        public Cell Position { get; internal set; }
        public bool HasLeft { get; internal set; }
        public IReadOnlyList<Cell> Shape => _shape;
        internal int CellCount => _shape.Length;

        internal BoardBlock(BlockData data)
        {
            ColorId = data.ColorId;
            Position = data.Position;
            _shape = data.Shape.ToArray();
        }

        internal Cell CellAt(int index) => Position + _shape[index];
    }
}
