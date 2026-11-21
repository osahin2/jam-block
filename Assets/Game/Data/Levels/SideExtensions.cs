namespace Game.Data.Levels
{
    public static class SideExtensions
    {
        public static bool IsHorizontal(this Side side) => side == Side.Top || side == Side.Bottom;

        public static int Along(this Side side, Cell cell) => side.IsHorizontal() ? cell.X : cell.Y;

        public static int Across(this Side side, Cell cell) => side.IsHorizontal() ? cell.Y : cell.X;

        public static int Length(this Side side, int width, int height) => side.IsHorizontal() ? width : height;
    }
}
