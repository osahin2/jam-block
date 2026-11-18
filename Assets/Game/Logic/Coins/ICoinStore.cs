namespace Game.Logic.Coins
{
    // Where the coin balance is kept between sessions. The wallet reads it once and writes every change.
    public interface ICoinStore
    {
        int Load();

        void Save(int balance);
    }
}
