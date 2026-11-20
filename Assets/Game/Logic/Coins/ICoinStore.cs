namespace Game.Logic.Coins
{
    public interface ICoinStore
    {
        int Load();

        // Persists before returning; the wallet calls it on every change.
        void Save(int balance);
    }
}
