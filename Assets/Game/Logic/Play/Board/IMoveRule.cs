namespace Game.Logic.Play
{
    public interface IMoveRule
    {
        bool Allows(IReadOnlyBoard board, int block, Direction direction);
    }
}
