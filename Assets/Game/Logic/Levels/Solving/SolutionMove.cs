using Game.Data.Levels;

namespace Game.Logic.Levels
{
    public readonly struct SolutionMove
    {
        public int Block { get; }
        public Cell Position { get; }
        public bool Leaves { get; }

        private SolutionMove(int block, Cell position, bool leaves)
        {
            Block = block;
            Position = position;
            Leaves = leaves;
        }

        public static SolutionMove To(int block, Cell position) => new SolutionMove(block, position, false);

        public static SolutionMove LeaveFrom(int block, Cell position) => new SolutionMove(block, position, true);

        public override string ToString() => Leaves ? $"{Block} leaves from {Position}" : $"{Block} to {Position}";
    }
}
