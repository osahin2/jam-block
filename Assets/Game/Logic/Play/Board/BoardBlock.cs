using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class BoardBlock
    {
        private readonly Cell[] _shape;

        public int ColorId { get; }
        public Cell Position { get; internal set; }
        public bool IsGone { get; internal set; }
        internal int CellCount => _shape.Length;

        internal BoardBlock(BlockData data)
        {
            ColorId = data.ColorId;
            Position = data.Position;
            _shape = data.Shape.ToArray();
        }

        public int ExtentAcross(Side side) => ShapeExtent.Across(_shape, side);

        internal Cell CellAt(int index) => Position + _shape[index];
    }
}
