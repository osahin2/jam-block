using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class ArrowMechanic : IMoveRule
    {
        private static readonly ArrowData NoArrow = new ArrowData();

        private readonly ArrowData[] _arrows;

        public ArrowMechanic(LevelData level)
        {
            _arrows = new ArrowData[level.Blocks.Count];
            for (var i = 0; i < _arrows.Length; i++)
                _arrows[i] = level.Blocks[i].Arrow;
        }

        public static bool AxisAllows(ArrowAxis axis, Direction direction) =>
            axis == ArrowAxis.None ||
            (axis == ArrowAxis.Horizontal ? direction.Offset.Y == 0 : direction.Offset.X == 0);

        public ArrowData ArrowOf(IReadOnlyBoard board, int block)
        {
            var part = board.Blocks[block];
            var arrow = _arrows[part.LevelBlock];
            return arrow.IsSet && arrow.LiesOn(part.Shape) ? arrow : NoArrow;
        }

        public bool Allows(IReadOnlyBoard board, int block, Direction direction) =>
            AxisAllows(ArrowOf(board, block).Axis, direction);
    }
}
