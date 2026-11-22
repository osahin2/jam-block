using System;
using System.Collections.Generic;

namespace Game.Data.Levels
{
    [Serializable]
    public sealed class BlockData
    {
        public int ColorId;
        public Cell Position;
        public List<Cell> Shape = new List<Cell>();
        public int Ice;
        public ArrowData Arrow = new ArrowData();

        public int ExtentAlong(Side side) => ShapeExtent.Along(Shape, side);
    }
}
