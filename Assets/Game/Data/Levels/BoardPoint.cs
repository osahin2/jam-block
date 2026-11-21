namespace Game.Data.Levels
{
    public readonly struct BoardPoint
    {
        public float X { get; }
        public float Y { get; }

        public BoardPoint(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
