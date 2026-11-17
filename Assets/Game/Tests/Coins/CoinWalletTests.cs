using System;
using Game.Logic.Coins;
using NUnit.Framework;

namespace Game.Tests.Coins
{
    public sealed class CoinWalletTests
    {
        [Test]
        public void New_StartsWithTheGivenBalance()
        {
            var wallet = new CoinWallet(120);

            Assert.AreEqual(120, wallet.Balance);
        }

        [Test]
        public void Add_RaisesTheBalance()
        {
            var wallet = new CoinWallet(120);

            wallet.Add(40);

            Assert.AreEqual(160, wallet.Balance);
        }

        [Test]
        public void Add_ANegativeAmount_Throws()
        {
            var wallet = new CoinWallet(120);

            Assert.Throws<ArgumentOutOfRangeException>(() => wallet.Add(-1));
            Assert.AreEqual(120, wallet.Balance);
        }

        [Test]
        public void New_WithANegativeBalance_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CoinWallet(-1));
        }
    }
}
