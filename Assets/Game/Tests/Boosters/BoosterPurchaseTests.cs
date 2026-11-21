using System;
using System.Collections.Generic;
using Game.Logic.Boosters;
using Game.Logic.Coins;
using NUnit.Framework;

namespace Game.Tests.Boosters
{
    public sealed class BoosterPurchaseTests
    {
        private const string Hammer = "hammer";

        private List<string> _log;
        private MemoryBoosterStore _boosters;
        private LoggedCoinStore _coins;
        private BoosterInventory _inventory;

        [SetUp]
        public void SetUp()
        {
            _log = new List<string>();
            _boosters = new MemoryBoosterStore(_log);
            _inventory = new BoosterInventory(_boosters, new Dictionary<string, int> { [Hammer] = 0 });
        }

        [Test]
        public void TryBuy_WithEnoughCoins_AddsThePackAndSpendsThePrice()
        {
            var wallet = Wallet(150);

            Assert.IsTrue(new BoosterPurchase(_inventory, wallet).TryBuy(Hammer, 3, 100));

            Assert.AreEqual(3, _inventory.Count(Hammer));
            Assert.AreEqual(50, wallet.Balance);
            Assert.AreEqual(50, _coins.Saved);
        }

        [Test]
        public void TryBuy_SavesTheStockBeforeSpendingTheCoins()
        {
            new BoosterPurchase(_inventory, Wallet(150)).TryBuy(Hammer, 3, 100);

            CollectionAssert.AreEqual(new[] { "boosters", "coins" }, _log);
        }

        [Test]
        public void TryBuy_WithTooFewCoins_ChangesAndSavesNothing()
        {
            var wallet = Wallet(99);

            Assert.IsFalse(new BoosterPurchase(_inventory, wallet).TryBuy(Hammer, 3, 100));

            Assert.AreEqual(0, _inventory.Count(Hammer));
            Assert.AreEqual(99, wallet.Balance);
            Assert.IsEmpty(_log);
        }

        [Test]
        public void TryBuy_ForANegativePrice_ThrowsAndSavesNothing()
        {
            var purchase = new BoosterPurchase(_inventory, Wallet(150));

            Assert.Throws<ArgumentOutOfRangeException>(() => purchase.TryBuy(Hammer, 3, -1));
            Assert.IsEmpty(_log);
        }

        [TestCase(100, true)]
        [TestCase(99, false)]
        public void CanAfford_ComparesTheBalanceWithThePrice(int balance, bool expected)
        {
            Assert.AreEqual(expected, new BoosterPurchase(_inventory, Wallet(balance)).CanAfford(100));
        }

        private CoinWallet Wallet(int balance)
        {
            _coins = new LoggedCoinStore(balance, _log);
            return new CoinWallet(_coins);
        }

        private sealed class LoggedCoinStore : ICoinStore
        {
            private readonly List<string> _log;

            public int Saved { get; private set; }

            public LoggedCoinStore(int saved, List<string> log)
            {
                Saved = saved;
                _log = log;
            }

            public int Load() => Saved;

            public void Save(int balance)
            {
                Saved = balance;
                _log.Add("coins");
            }
        }
    }
}
