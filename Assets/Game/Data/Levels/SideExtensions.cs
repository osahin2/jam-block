namespace Game.Data.Levels
{
    public static class SideExtensions
    {
        public static bool IsHorizontal(this Side side) => side == Side.Top || side == Side.Bottom;

        // A cell's position along the side: its column on Top and Bottom, its row on Left and Right.
        public static int Along(this Side side, Cell cell) => side.IsHorizontal() ? cell.X : cell.Y;

        public static int Length(this Side side, int width, int height) => side.IsHorizontal() ? width : height;
    }
}
