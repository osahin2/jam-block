using System;
using Game.Logic.Coins;
using NUnit.Framework;

namespace Game.Tests.Coins
{
    public sealed class CoinWalletTests
    {
        [Test]
        public void New_StartsWithTheStoredBalance()
        {
            var wallet = new CoinWallet(new MemoryCoinStore(120));

            Assert.AreEqual(120, wallet.Balance);
        }

        [Test]
        public void Add_RaisesTheBalance()
        {
            var wallet = new CoinWallet(new MemoryCoinStore(120));

            wallet.Add(40);

            Assert.AreEqual(160, wallet.Balance);
        }

        [Test]
        public void Add_SavesTheNewBalance()
        {
            var store = new MemoryCoinStore(120);
            var wallet = new CoinWallet(store);

            wallet.Add(40);

            Assert.AreEqual(160, store.Saved);
        }

        [Test]
        public void New_AfterAnAdd_StartsWhereTheLastWalletStopped()
        {
            var store = new MemoryCoinStore(0);
            new CoinWallet(store).Add(40);

            var wallet = new CoinWallet(store);

            Assert.AreEqual(40, wallet.Balance);
        }

        [Test]
        public void Add_ANegativeAmount_ThrowsAndSavesNothing()
        {
            var store = new MemoryCoinStore(120);
            var wallet = new CoinWallet(store);

            Assert.Throws<ArgumentOutOfRangeException>(() => wallet.Add(-1));
            Assert.AreEqual(120, wallet.Balance);
            Assert.AreEqual(0, store.SaveCount);
        }

        [Test]
        public void CanSpend_ComparesTheAmountWithTheBalance()
        {
            var wallet = new CoinWallet(new MemoryCoinStore(120));

            Assert.IsTrue(wallet.CanSpend(120));
            Assert.IsFalse(wallet.CanSpend(121));
        }

        [Test]
        public void TrySpend_WithEnoughCoins_LowersTheBalanceAndSaves()
        {
            var store = new MemoryCoinStore(120);
            var wallet = new CoinWallet(store);

            Assert.IsTrue(wallet.TrySpend(120));

            Assert.AreEqual(0, wallet.Balance);
            Assert.AreEqual(0, store.Saved);
        }

        [Test]
        public void TrySpend_WithTooFewCoins_ChangesAndSavesNothing()
        {
            var store = new MemoryCoinStore(120);
            var wallet = new CoinWallet(store);

            Assert.IsFalse(wallet.TrySpend(121));

            Assert.AreEqual(120, wallet.Balance);
            Assert.AreEqual(0, store.SaveCount);
        }

        [Test]
        public void TrySpend_ANegativeAmount_ThrowsAndSavesNothing()
        {
            var store = new MemoryCoinStore(120);
            var wallet = new CoinWallet(store);

            Assert.Throws<ArgumentOutOfRangeException>(() => wallet.TrySpend(-1));
            Assert.AreEqual(120, wallet.Balance);
            Assert.AreEqual(0, store.SaveCount);
        }

        [Test]
        public void New_WithANegativeStoredBalance_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CoinWallet(new MemoryCoinStore(-1)));
        }

        private sealed class MemoryCoinStore : ICoinStore
        {
            public int Saved { get; private set; }
            public int SaveCount { get; private set; }

            public MemoryCoinStore(int saved) => Saved = saved;

            public int Load() => Saved;

            public void Save(int balance)
            {
                Saved = balance;
                SaveCount++;
            }
        }
    }
}
