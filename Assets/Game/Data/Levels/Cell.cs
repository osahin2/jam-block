using System;

namespace Game.Data.Levels
{
    [Serializable]
    public struct Cell : IEquatable<Cell>
    {
        public int X;
        public int Y;

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Cell operator +(Cell a, Cell b) => new Cell(a.X + b.X, a.Y + b.Y);

        public static Cell operator -(Cell a, Cell b) => new Cell(a.X - b.X, a.Y - b.Y);

        public static bool operator ==(Cell a, Cell b) => a.Equals(b);

        public static bool operator !=(Cell a, Cell b) => !a.Equals(b);

        public bool IsInside(int width, int height) => X >= 0 && X < width && Y >= 0 && Y < height;

        public int IndexIn(int width) => Y * width + X;

        public bool Touches(Cell other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y) == 1;

        public bool Equals(Cell other) => X == other.X && Y == other.Y;

        public override bool Equals(object obj) => obj is Cell other && Equals(other);

        public override int GetHashCode() => (X * 397) ^ Y;

        public override string ToString() => $"({X}, {Y})";
    }
}
