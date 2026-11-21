using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class BoardRemovalTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void Hit_OnAnUnguardedBlock_TakesItOffTheBoardAndFreesItsCells()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 0, 2, 1).Rect(Blue, 0, 1, 1, 1).Build());

            board.Hit(0);

            Assert.IsTrue(board.Blocks[0].IsGone);
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(0, 0)));
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(1, 0)));
            Assert.AreEqual(1, board.RemainingBlocks);
            Assert.IsTrue(board.CanStep(1, Direction.Up));
        }

        [Test]
        public void Hit_OnTheLastBlock_ClearsTheBoard()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Build());

            board.Hit(0);

            Assert.IsTrue(board.IsCleared);
        }

        [Test]
        public void Hit_OnABlockAlreadyGone_ChangesNothing()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Blue, 2, 2, 1, 1).Build());
            board.Hit(0);

            board.Hit(0);

            Assert.AreEqual(1, board.RemainingBlocks);
            Assert.AreEqual(1, board.GoneCount);
        }

        [Test]
        public void GoneCount_CountsEveryBlockGone_ThroughADoorOrHit()
        {
            var board = Boards.Of(new LevelBuilder(3, 3)
                .Rect(Red, 1, 1, 1, 1).Rect(Blue, 0, 2, 1, 1).Rect(Blue, 2, 2, 1, 1)
                .Door(Red, Side.Top, 1).Build());
            Assert.AreEqual(0, board.GoneCount);

            board.Step(0, Direction.Up);
            board.Hit(1);

            Assert.AreEqual(2, board.GoneCount);
        }

        [Test]
        public void GoneCount_DoesNotCountATouchThatLeavesNothing()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Door(Blue, Side.Top, 1).Build());

            board.Step(0, Direction.Up);
            board.TryLeave(0, out _);

            Assert.AreEqual(0, board.GoneCount);
        }

        [Test]
        public void ExtentAcross_OfABoardBlock_IsHowFarItReachesFromTheSide()
        {
            var board = Boards.Of(new LevelBuilder(4, 4).Block(Red, 0, 0, new Cell(0, 0), new Cell(0, 1), new Cell(0, 2), new Cell(1, 2)).Build());

            Assert.AreEqual(3, board.Blocks[0].ExtentAcross(Side.Top));
            Assert.AreEqual(2, board.Blocks[0].ExtentAcross(Side.Left));
        }
    }
}
