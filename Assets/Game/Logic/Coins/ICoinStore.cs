namespace Game.Logic.Coins
{
    public interface ICoinStore
    {
        int Load();

        void Save(int balance);
    }
}
