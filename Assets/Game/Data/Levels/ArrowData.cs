using System;
using System.Collections.Generic;

namespace Game.Data.Levels
{
    [Serializable]
    public sealed class ArrowData
    {
        public ArrowAxis Axis;
        public Cell Start;
        public int Length;

        public bool IsSet => Axis != ArrowAxis.None;
        public Cell Step => Axis == ArrowAxis.Horizontal ? new Cell(1, 0) : new Cell(0, 1);

        public Cell CellAt(int index) => new Cell(Start.X + Step.X * index, Start.Y + Step.Y * index);

        public BoardPoint Middle()
        {
            var half = (Length - 1) / 2f;
            return new BoardPoint(Start.X + Step.X * half, Start.Y + Step.Y * half);
        }

        public bool LiesOn(IReadOnlyList<Cell> shape)
        {
            if (Length < 1)
                return false;
            for (var i = 0; i < Length; i++)
            {
                if (!Contains(shape, CellAt(i)))
                    return false;
            }
            return true;
        }

        private static bool Contains(IReadOnlyList<Cell> shape, Cell cell)
        {
            for (var i = 0; i < shape.Count; i++)
            {
                if (shape[i] == cell)
                    return true;
            }
            return false;
        }
    }
}
