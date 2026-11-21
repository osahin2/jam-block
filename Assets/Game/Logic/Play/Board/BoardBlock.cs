using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class BoardBlock
    {
        private readonly List<Cell> _shape;

        public int ColorId { get; }
        public Cell Position { get; internal set; }
        public bool IsGone { get; internal set; }
        public IReadOnlyList<Cell> Shape => _shape;
        internal int CellCount => _shape.Count;

        internal BoardBlock(int colorId, Cell position, IReadOnlyList<Cell> shape)
        {
            ColorId = colorId;
            Position = position;
            _shape = new List<Cell>(shape.Count);
            Reshape(shape);
        }

        public int ExtentAcross(Side side) => ShapeExtent.Across(_shape, side);

        internal Cell CellAt(int index) => Position + _shape[index];

        internal void RemoveCell(Cell cell) => _shape.Remove(cell - Position);

        internal void Reshape(IReadOnlyList<Cell> shape)
        {
            _shape.Clear();
            for (var i = 0; i < shape.Count; i++)
                _shape.Add(shape[i]);
        }
    }
}
