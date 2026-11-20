using System;

namespace Game.Logic.Coins
{
    public sealed class CoinWallet
    {
        private readonly ICoinStore _store;

        public int Balance { get; private set; }

        public CoinWallet(ICoinStore store)
        {
            _store = store;
            var balance = store.Load();
            if (balance < 0)
                throw new ArgumentOutOfRangeException(nameof(store), balance, "A stored balance cannot be negative.");
            Balance = balance;
        }

        public void Add(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "An amount to add cannot be negative.");
            Balance += amount;
            _store.Save(Balance);
        }
    }
}
