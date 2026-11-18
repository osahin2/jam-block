using Core.Save;
using Game.Data.Coins;
using Game.Logic.Coins;

namespace Game.Coins
{
    // Keeps the coin balance in one save document.
    public sealed class SavedCoinStore : ICoinStore
    {
        private const string Key = "coins";

        private readonly ISaveService _save;

        public SavedCoinStore(ISaveService save) => _save = save;

        public int Load() => _save.Load<CoinsDocument>(Key).Balance;

        public void Save(int balance) => _save.Save(Key, new CoinsDocument { Balance = balance });
    }
}
