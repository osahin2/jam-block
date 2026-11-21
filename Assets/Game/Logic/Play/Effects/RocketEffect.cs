using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class RocketEffect : IBoardEffect
    {
        public bool CanTarget(IReadOnlyBoard board, Cell cell) => board.BlockAt(cell) != Board.NoBlock;

        public void Apply(IBoard board, Cell cell) => board.HitCell(cell);
    }
}
