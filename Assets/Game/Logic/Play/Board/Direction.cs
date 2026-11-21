using Game.Data.Levels;

namespace Game.Logic.Play
{
    public readonly struct Direction
    {
        public static readonly Direction Up = new Direction(0, -1);
        public static readonly Direction Down = new Direction(0, 1);
        public static readonly Direction Left = new Direction(-1, 0);
        public static readonly Direction Right = new Direction(1, 0);

        private static readonly Direction[] OutOfSide = { Up, Down, Left, Right };

        public Cell Offset { get; }

        private Direction(int x, int y) => Offset = new Cell(x, y);

        public static Direction OutOf(Side side) => OutOfSide[(int)side];
    }
}
