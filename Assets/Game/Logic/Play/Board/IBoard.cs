namespace Game.Logic.Play
{
    public interface IBoard : IReadOnlyBoard
    {
        void Hit(int block);
    }
}
