using Game.Data.Levels;

namespace Game.Logic.Play
{
    public interface IBoard : IReadOnlyBoard
    {
        void Hit(int block);

        void HitCell(Cell cell);
    }
}
