using System;

namespace Game.Logic.Coins
{
    // The player's coin balance. Coins are only earned so far; the balance never goes below zero.
    public sealed class CoinWallet
    {
        public int Balance { get; private set; }

        public CoinWallet(int balance)
        {
            if (balance < 0)
                throw new ArgumentOutOfRangeException(nameof(balance), balance, "A balance cannot be negative.");
            Balance = balance;
        }

        public void Add(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "An amount to add cannot be negative.");
            Balance += amount;
        }
    }
}
