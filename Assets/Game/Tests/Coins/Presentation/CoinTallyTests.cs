using Game.Coins;
using NUnit.Framework;

namespace Game.Tests.Coins
{
    public sealed class CoinTallyTests
    {
        [TestCase(40, 10)]
        [TestCase(7, 3)]
        [TestCase(3, 10)]
        [TestCase(0, 4)]
        public void Share_AllLandingsAddUpToTheAmount(int amount, int coins)
        {
            var total = 0;
            for (var landing = 1; landing <= coins; landing++)
                total += CoinTally.Share(landing, coins, amount);

            Assert.AreEqual(amount, total);
        }

        [TestCase(7, 3)]
        [TestCase(3, 10)]
        public void Share_IsNeverNegative(int amount, int coins)
        {
            for (var landing = 1; landing <= coins; landing++)
                Assert.GreaterOrEqual(CoinTally.Share(landing, coins, amount), 0);
        }

        [Test]
        public void Share_SplitsAnEvenAmountEvenly()
        {
            Assert.AreEqual(4, CoinTally.Share(5, 10, 40));
        }

        [Test]
        public void Share_TwoInterleavedFlightsEndOnBothAmounts()
        {
            var shown = 100;
            for (var landing = 1; landing <= 10; landing++)
            {
                shown += CoinTally.Share(landing, 10, 40);
                shown += CoinTally.Share(landing, 10, 25);
            }

            Assert.AreEqual(165, shown);
        }
    }
}
