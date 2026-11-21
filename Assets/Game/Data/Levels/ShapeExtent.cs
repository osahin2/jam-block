using System;
using System.Collections.Generic;

namespace Game.Data.Levels
{
    public static class ShapeExtent
    {
        public static int Along(IReadOnlyList<Cell> shape, Side side) => Extent(shape, side, (s, cell) => s.Along(cell));

        public static int Across(IReadOnlyList<Cell> shape, Side side) => Extent(shape, side, (s, cell) => s.Across(cell));

        private static int Extent(IReadOnlyList<Cell> shape, Side side, Func<Side, Cell, int> coordinate)
        {
            var min = int.MaxValue;
            var max = int.MinValue;
            for (var i = 0; i < shape.Count; i++)
            {
                var value = coordinate(side, shape[i]);
                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }
            return max - min + 1;
        }
    }
}
