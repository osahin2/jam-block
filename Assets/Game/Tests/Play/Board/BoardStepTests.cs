using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class BoardStepTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void Step_IntoFreeCell_MovesTheBlock()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Build());

            var result = board.Step(0, Direction.Right);

            Assert.IsTrue(result.Moved);
            Assert.AreEqual(new Cell(2, 1), result.Position);
            Assert.AreEqual(new Cell(2, 1), board.Blocks[0].Position);
            Assert.IsFalse(result.HasLeft);
        }

        [Test]
        public void BlockAt_FindsTheBlockOnACellAndNothingElsewhere()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Blue, 1, 1, 2, 1).Build());

            Assert.AreEqual(1, board.BlockAt(new Cell(2, 1)));
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(2, 2)));
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(3, 0)));
        }

        [Test]
        public void CanStep_PastTheBoardEdge_IsFalse()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Build());

            Assert.IsFalse(board.CanStep(0, Direction.Up));
            Assert.IsFalse(board.CanStep(0, Direction.Left));
            Assert.IsTrue(board.CanStep(0, Direction.Down));
            Assert.IsTrue(board.CanStep(0, Direction.Right));
        }

        [Test]
        public void CanStep_IntoAnotherBlock_IsFalse()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Blue, 1, 0, 1, 1).Build());

            Assert.IsFalse(board.CanStep(0, Direction.Right));
        }

        [Test]
        public void Step_FreesTheCellsTheBlockLeft()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Blue, 1, 0, 1, 1).Build());

            board.Step(1, Direction.Down);

            Assert.IsTrue(board.CanStep(0, Direction.Right));
        }

        [Test]
        public void CanStep_BlockLongerThanOneCell_DoesNotBlockItself()
        {
            var board = Boards.Of(new LevelBuilder(1, 3).Rect(Red, 0, 0, 1, 2).Build());

            var result = board.Step(0, Direction.Down);

            Assert.AreEqual(new Cell(0, 1), result.Position);
        }

        [Test]
        public void CanStep_LShape_PassesWhereOnlyItsNotchOverlaps()
        {
            var level = new LevelBuilder(3, 3)
                .Block(Red, 0, 0, new Cell(0, 0), new Cell(0, 1), new Cell(1, 1))
                .Rect(Blue, 2, 0, 1, 1)
                .Build();
            var board = Boards.Of(level);

            var result = board.Step(0, Direction.Right);

            Assert.AreEqual(new Cell(1, 0), result.Position);
        }

        [Test]
        public void CanStep_LShape_IsBlockedByAnyOfItsCells()
        {
            var level = new LevelBuilder(3, 3)
                .Block(Red, 0, 0, new Cell(0, 0), new Cell(0, 1), new Cell(1, 1))
                .Rect(Blue, 2, 1, 1, 1)
                .Build();
            var board = Boards.Of(level);

            Assert.IsFalse(board.CanStep(0, Direction.Right));
        }

        [Test]
        public void Step_WhenTheStepIsNotFree_LeavesTheBlockInPlace()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Blue, 1, 0, 1, 1).Build());

            var result = board.Step(0, Direction.Right);

            Assert.IsFalse(result.Moved);
            Assert.AreEqual(new Cell(0, 0), result.Position);
            Assert.AreEqual(new Cell(0, 0), board.Blocks[0].Position);
            Assert.IsFalse(board.CanStep(1, Direction.Left));
        }
    }
}
