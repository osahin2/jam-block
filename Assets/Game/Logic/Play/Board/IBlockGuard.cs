namespace Game.Logic.Play
{
    public interface IBlockGuard
    {
        bool Guards(IBoardFacts board, int block);

        void TakeHit(int block);
    }
}
