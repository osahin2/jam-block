using Game.Data.Levels;
using Game.Logic.Levels;
using Game.Tests.Play;
using NUnit.Framework;

namespace Game.Tests.Levels
{
    public sealed class BlockReachTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void Find_OnAnEmptyBoard_ReachesEveryOtherPlace()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Build());
            var reach = new BlockReach();

            reach.Find(board, 0);

            Assert.IsFalse(reach.CanLeave);
            Assert.AreEqual(8, reach.Places.Count);
            CollectionAssert.DoesNotContain(reach.Places, new Cell(1, 1));
        }

        [Test]
        public void Find_AroundAnotherBlock_ReachesOnlyFreePlaces()
        {
            var board = Boards.Of(new LevelBuilder(3, 1).Rect(Red, 0, 0, 1, 1).Rect(Blue, 1, 0, 1, 1).Build());
            var reach = new BlockReach();

            reach.Find(board, 0);

            Assert.IsEmpty(reach.Places);
        }

        [Test]
        public void Find_AnIcedBlock_ReachesNothing()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Ice(1).Build());
            var reach = new BlockReach();

            reach.Find(board, 0);

            Assert.IsEmpty(reach.Places);
        }

        [Test]
        public void Find_AWayToItsDoor_CanLeaveAtThePlaceTouchingIt()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 2, 1, 1).Door(Red, Side.Right, 0).Build());
            var reach = new BlockReach();

            reach.Find(board, 0);

            Assert.IsTrue(reach.CanLeave);
            Assert.AreEqual(new Cell(2, 0), reach.ExitPlace);
        }

        [Test]
        public void Find_LeavesTheBlockWhereItStood()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 2, 1, 1).Door(Red, Side.Right, 0).Build());
            var reach = new BlockReach();

            reach.Find(board, 0);

            Assert.AreEqual(new Cell(0, 2), board.Blocks[0].Position);
            Assert.AreEqual(0, board.BlockAt(new Cell(0, 2)));
            Assert.AreEqual(1, board.RemainingBlocks);
        }
    }
}
