using System;
using System.Collections.Generic;

namespace Game.Data.Levels
{
    [Serializable]
    public sealed class BlockData
    {
        public int ColorId;
        public Cell Position;
        // Relative to Position.
        public List<Cell> Shape = new List<Cell>();

        // How many cells the block spans along the side: the width a door on that side needs to let it out.
        public int ExtentAlong(Side side) => Extent(side, (s, cell) => s.Along(cell));

        // How many cells the block spans across the side: how far it slides to pass a door on that side.
        public int ExtentAcross(Side side) => Extent(side, (s, cell) => s.Across(cell));

        private int Extent(Side side, Func<Side, Cell, int> coordinate)
        {
            var min = int.MaxValue;
            var max = int.MinValue;
            foreach (var cell in Shape)
            {
                var value = coordinate(side, cell);
                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }
            return max - min + 1;
        }
    }
}
