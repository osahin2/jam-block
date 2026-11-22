using Game.Logic.Play;

namespace Game.Tests.Play
{
    public sealed class ForbiddenDirectionRule : IMoveRule
    {
        private readonly Direction _forbidden;

        public ForbiddenDirectionRule(Direction forbidden)
        {
            _forbidden = forbidden;
        }

        public bool Allows(IReadOnlyBoard board, int block, Direction direction) => direction.Offset != _forbidden.Offset;
    }
}
