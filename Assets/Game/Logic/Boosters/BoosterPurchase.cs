using System;
using Game.Logic.Coins;

namespace Game.Logic.Boosters
{
    public sealed class BoosterPurchase
    {
        private readonly BoosterInventory _inventory;
        private readonly CoinWallet _wallet;

        public BoosterPurchase(BoosterInventory inventory, CoinWallet wallet)
        {
            _inventory = inventory;
            _wallet = wallet;
        }

        public bool CanAfford(int price) => _wallet.CanSpend(price);

        public bool TryBuy(string id, int amount, int price)
        {
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), price, "A price cannot be negative.");
            if (!CanAfford(price))
                return false;
            _inventory.Add(id, amount);
            return _wallet.TrySpend(price);
        }
    }
}
