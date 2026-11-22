using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class BoardArrangementTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void Arrange_ASavedArrangement_RestoresPositionsAndBlocksGone()
        {
            var board = Boards.Of(TwoBlocks());
            var saved = board.CurrentArrangement();
            board.Step(0, Direction.Down);
            board.TryLeave(1, out _);

            board.Arrange(saved);

            Assert.AreEqual(new Cell(0, 0), board.Blocks[0].Position);
            Assert.IsFalse(board.Blocks[1].IsGone);
            Assert.AreEqual(1, board.BlockAt(new Cell(2, 0)));
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(0, 1)));
            Assert.AreEqual(2, board.RemainingBlocks);
            Assert.AreEqual(0, board.GoneCount);
        }

        [Test]
        public void Arrange_WithABlockGone_FreesItsCellsAndCountsIt()
        {
            var board = Boards.Of(TwoBlocks());
            board.TryLeave(1, out _);
            var saved = board.CurrentArrangement();
            board.Arrange(Boards.Of(TwoBlocks()).CurrentArrangement());

            board.Arrange(saved);

            Assert.IsTrue(board.Blocks[1].IsGone);
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(2, 0)));
            Assert.AreEqual(1, board.GoneCount);
            Assert.IsFalse(board.IsCleared);
        }

        [Test]
        public void Place_MovesTheBlockAndItsCells()
        {
            var board = Boards.Of(TwoBlocks());

            board.Place(0, new Cell(1, 1));

            Assert.AreEqual(new Cell(1, 1), board.Blocks[0].Position);
            Assert.AreEqual(0, board.BlockAt(new Cell(1, 1)));
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(0, 0)));
        }

        [Test]
        public void CanLeave_AtItsDoor_IsTrueAndChangesNothing()
        {
            var board = Boards.Of(TwoBlocks());

            Assert.IsTrue(board.CanLeave(1));
            Assert.IsFalse(board.CanLeave(0));
            Assert.AreEqual(2, board.RemainingBlocks);
        }

        private static LevelData TwoBlocks() =>
            new LevelBuilder(3, 2)
                .Rect(Red, 0, 0, 1, 1)
                .Rect(Blue, 2, 0, 1, 1)
                .Door(Blue, Side.Right, 0)
                .Build();
    }
}
