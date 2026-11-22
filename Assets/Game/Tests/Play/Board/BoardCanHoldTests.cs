using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class BoardCanHoldTests
    {
        private const int Red = 1;

        [Test]
        public void CanHold_BlocksApartOnTheBoard_IsTrue()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 2, 1).Rect(Red, 0, 2, 1, 1).Build();

            Assert.IsTrue(Board.CanHold(level));
        }

        [Test]
        public void CanHold_ABlockCellOffTheBoard_IsFalse()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 2, 0, 2, 1).Build();

            Assert.IsFalse(Board.CanHold(level));
        }

        [Test]
        public void CanHold_TwoBlocksOnOneCell_IsFalse()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 2, 1).Rect(Red, 1, 0, 1, 1).Build();

            Assert.IsFalse(Board.CanHold(level));
        }
    }
}
