using System;
using System.Collections.Generic;

namespace Game.Data.Levels
{
    public static class ShapeMiddle
    {
        public static BoardPoint Of(IReadOnlyList<Cell> shape)
        {
            var center = CenterOfCells(shape);
            return IsOnShape(shape, center) ? center : CellNearest(shape, center);
        }

        private static BoardPoint CenterOfCells(IReadOnlyList<Cell> shape)
        {
            var sumX = 0;
            var sumY = 0;
            foreach (var cell in shape)
            {
                sumX += cell.X;
                sumY += cell.Y;
            }
            return new BoardPoint((float)sumX / shape.Count, (float)sumY / shape.Count);
        }

        private static bool IsOnShape(IReadOnlyList<Cell> shape, BoardPoint point)
        {
            var under = new Cell((int)Math.Floor(point.X + 0.5f), (int)Math.Floor(point.Y + 0.5f));
            foreach (var cell in shape)
            {
                if (cell == under)
                    return true;
            }
            return false;
        }

        private static BoardPoint CellNearest(IReadOnlyList<Cell> shape, BoardPoint point)
        {
            var nearest = shape[0];
            foreach (var cell in shape)
            {
                if (DistanceSquared(cell, point) < DistanceSquared(nearest, point))
                    nearest = cell;
            }
            return new BoardPoint(nearest.X, nearest.Y);
        }

        private static float DistanceSquared(Cell cell, BoardPoint point)
        {
            var dx = cell.X - point.X;
            var dy = cell.Y - point.Y;
            return dx * dx + dy * dy;
        }
    }
}
