namespace Game.Logic.Play
{
    public interface IMoveRule
    {
        bool Allows(IBoardFacts board, int block, Direction direction);
    }
}
