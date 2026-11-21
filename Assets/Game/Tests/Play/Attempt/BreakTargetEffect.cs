using Game.Data.Levels;
using Game.Logic.Play;

namespace Game.Tests.Play
{
    public sealed class BreakTargetEffect : IBoardEffect
    {
        public int Applied { get; private set; }

        public bool CanTarget(IReadOnlyBoard board, Cell cell) => board.BlockAt(cell) != Board.NoBlock;

        public void Apply(Board board, Cell cell)
        {
            Applied++;
            board.Hit(board.BlockAt(cell));
        }
    }
}
