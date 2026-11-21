using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class ArrowMechanic : IMoveRule
    {
        private readonly ArrowAxis[] _axes;

        public ArrowMechanic(LevelData level)
        {
            _axes = new ArrowAxis[level.Blocks.Count];
            for (var i = 0; i < _axes.Length; i++)
                _axes[i] = level.Blocks[i].Arrow.Axis;
        }

        public static bool AxisAllows(ArrowAxis axis, Direction direction) =>
            axis == ArrowAxis.None ||
            (axis == ArrowAxis.Horizontal ? direction.Offset.Y == 0 : direction.Offset.X == 0);

        public bool Allows(IReadOnlyBoard board, int block, Direction direction) =>
            block >= _axes.Length || AxisAllows(_axes[block], direction);
    }
}
