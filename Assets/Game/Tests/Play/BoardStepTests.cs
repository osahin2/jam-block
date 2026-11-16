using System;
using Game.Data.Levels;
using Game.Logic.Play;
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
            var board = new Board(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Build());

            var result = board.Step(0, Direction.Right);

            Assert.AreEqual(new Cell(2, 1), result.Position);
            Assert.AreEqual(new Cell(2, 1), board.Blocks[0].Position);
            Assert.IsFalse(result.HasLeft);
        }

        [Test]
        public void CanStep_PastTheBoardEdge_IsFalse()
        {
            var board = new Board(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Build());

            Assert.IsFalse(board.CanStep(0, Direction.Up));
            Assert.IsFalse(board.CanStep(0, Direction.Left));
            Assert.IsTrue(board.CanStep(0, Direction.Down));
            Assert.IsTrue(board.CanStep(0, Direction.Right));
        }

        [Test]
        public void CanStep_IntoAnotherBlock_IsFalse()
        {
            var board = new Board(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Blue, 1, 0, 1, 1).Build());

            Assert.IsFalse(board.CanStep(0, Direction.Right));
        }

        [Test]
        public void Step_FreesTheCellsTheBlockLeft()
        {
            var board = new Board(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Blue, 1, 0, 1, 1).Build());

            board.Step(1, Direction.Down);

            Assert.IsTrue(board.CanStep(0, Direction.Right));
        }

        [Test]
        public void CanStep_BlockLongerThanOneCell_DoesNotBlockItself()
        {
            var board = new Board(new LevelBuilder(1, 3).Rect(Red, 0, 0, 1, 2).Build());

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
            var board = new Board(level);

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
            var board = new Board(level);

            Assert.IsFalse(board.CanStep(0, Direction.Right));
        }

        [Test]
        public void Step_WhenTheStepIsNotFree_Throws()
        {
            var board = new Board(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Build());

            Assert.Throws<InvalidOperationException>(() => board.Step(0, Direction.Up));
        }

        [Test]
        public void Constructor_OverlappingBlocks_Throws()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 2, 1).Rect(Blue, 1, 0, 1, 1).Build();

            Assert.Throws<ArgumentException>(() => new Board(level));
        }

        [Test]
        public void Constructor_BlockOutsideTheBoard_Throws()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 2, 0, 2, 1).Build();

            Assert.Throws<ArgumentException>(() => new Board(level));
        }

        [Test]
        public void Constructor_DoorPastTheEndOfItsSide_Throws()
        {
            var level = new LevelBuilder(3, 4).Rect(Red, 0, 0, 1, 1).Door(Red, Side.Top, 2, 2).Build();

            Assert.Throws<ArgumentException>(() => new Board(level));
        }
    }
}
