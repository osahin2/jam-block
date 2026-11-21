namespace Game.Logic.Play
{
    public interface IBlockGuard
    {
        bool Guards(IReadOnlyBoard board, int block);

        void TakeHit(int block);
    }
}
