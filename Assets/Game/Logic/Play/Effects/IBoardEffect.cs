using Game.Data.Levels;

namespace Game.Logic.Play
{
    public interface IBoardEffect
    {
        bool CanTarget(IReadOnlyBoard board, Cell cell);

        void Apply(Board board, Cell cell);
    }
}
