using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class BoardHitCellTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void HitCell_OnABlocksOnlyCell_TakesTheBlockOffTheBoard()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Blue, 2, 2, 1, 1).Build());

            board.HitCell(new Cell(0, 0));

            Assert.IsTrue(board.Blocks[0].IsGone);
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(0, 0)));
            Assert.AreEqual(1, board.RemainingBlocks);
            Assert.AreEqual(1, board.GoneCount);
            Assert.AreEqual(2, board.Blocks.Count);
        }

        [Test]
        public void HitCell_OnAnEndCell_ShrinksTheBlockWithoutASplit()
        {
            var board = Boards.Of(new LevelBuilder(4, 3).Rect(Red, 0, 0, 3, 1).Build());

            board.HitCell(new Cell(2, 0));

            Assert.AreEqual(1, board.Blocks.Count);
            Assert.AreEqual(2, board.Blocks[0].Shape.Count);
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(2, 0)));
            Assert.AreEqual(0, board.BlockAt(new Cell(1, 0)));
            Assert.AreEqual(1, board.RemainingBlocks);
            Assert.AreEqual(0, board.GoneCount);
            Assert.AreEqual(2, board.Blocks[0].ExtentAcross(Side.Left));
        }

        [Test]
        public void HitCell_PartsOfPartsKeepTheLevelBlockTheyCameFrom()
        {
            var board = Boards.Of(new LevelBuilder(8, 3).Rect(Blue, 0, 0, 1, 1).Rect(Red, 0, 1, 7, 1).Build());

            board.HitCell(new Cell(1, 1));
            board.HitCell(new Cell(3, 1));

            Assert.AreEqual(4, board.Blocks.Count);
            Assert.AreEqual(0, board.Blocks[0].LevelBlock);
            Assert.AreEqual(1, board.Blocks[board.BlockAt(new Cell(0, 1))].LevelBlock);
            Assert.AreEqual(1, board.Blocks[board.BlockAt(new Cell(2, 1))].LevelBlock);
            Assert.AreEqual(1, board.Blocks[board.BlockAt(new Cell(5, 1))].LevelBlock);
        }

        [Test]
        public void HitCell_OnTheMiddleOfALine_SplitsItIntoTwoBlocks()
        {
            var board = Boards.Of(new LevelBuilder(4, 3).Rect(Red, 0, 1, 3, 1).Build());

            board.HitCell(new Cell(1, 1));

            Assert.AreEqual(2, board.Blocks.Count);
            Assert.AreEqual(0, board.BlockAt(new Cell(0, 1)));
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(1, 1)));
            Assert.AreEqual(1, board.BlockAt(new Cell(2, 1)));
            Assert.AreEqual(Red, board.Blocks[1].ColorId);
            Assert.AreEqual(2, board.RemainingBlocks);
            Assert.AreEqual(0, board.GoneCount);
            Assert.AreEqual(1, board.Blocks[1].ExtentAcross(Side.Left));
        }

        [Test]
        public void HitCell_SplitParts_MoveOnTheirOwn()
        {
            var board = Boards.Of(new LevelBuilder(4, 3).Rect(Red, 0, 1, 3, 1).Build());

            board.HitCell(new Cell(1, 1));

            Assert.IsTrue(board.Step(1, Direction.Left).Moved);
            Assert.AreEqual(1, board.BlockAt(new Cell(1, 1)));
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(2, 1)));
            Assert.AreEqual(0, board.BlockAt(new Cell(0, 1)));
            Assert.IsFalse(board.CanStep(0, Direction.Right));
        }

        [Test]
        public void HitCell_OnTheCornerOfAThreeCellL_LeavesTwoSingleCells()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Block(Red, 0, 0, new Cell(0, 0), new Cell(0, 1), new Cell(1, 1)).Build());

            board.HitCell(new Cell(0, 1));

            Assert.AreEqual(2, board.Blocks.Count);
            Assert.AreEqual(1, board.Blocks[0].Shape.Count);
            Assert.AreEqual(1, board.Blocks[1].Shape.Count);
            Assert.AreEqual(0, board.BlockAt(new Cell(0, 0)));
            Assert.AreEqual(1, board.BlockAt(new Cell(1, 1)));
        }

        [Test]
        public void HitCell_OnTheMiddleOfAnL_KeepsTheIndexForTheFirstPartAndAppendsTheOther()
        {
            var board = Boards.Of(new LevelBuilder(4, 4)
                .Rect(Blue, 3, 3, 1, 1)
                .Block(Red, 0, 0, new Cell(0, 0), new Cell(0, 1), new Cell(0, 2), new Cell(1, 2)).Build());

            board.HitCell(new Cell(0, 1));

            Assert.AreEqual(3, board.Blocks.Count);
            Assert.AreEqual(1, board.Blocks[1].Shape.Count);
            Assert.AreEqual(1, board.BlockAt(new Cell(0, 0)));
            Assert.AreEqual(2, board.BlockAt(new Cell(0, 2)));
            Assert.AreEqual(2, board.BlockAt(new Cell(1, 2)));
            Assert.AreEqual(3, board.RemainingBlocks);
        }

        [Test]
        public void HitCell_OnTheCenterOfAPlus_LeavesFourBlocks()
        {
            var board = Boards.Of(new LevelBuilder(3, 3)
                .Block(Red, 0, 0, new Cell(1, 0), new Cell(0, 1), new Cell(1, 1), new Cell(2, 1), new Cell(1, 2)).Build());

            board.HitCell(new Cell(1, 1));

            Assert.AreEqual(4, board.Blocks.Count);
            Assert.AreEqual(4, board.RemainingBlocks);
            Assert.AreEqual(0, board.GoneCount);
        }

        [Test]
        public void HitCell_OnAnEmptyCell_ChangesNothing()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 0, 2, 1).Build());

            board.HitCell(new Cell(2, 2));

            Assert.AreEqual(1, board.Blocks.Count);
            Assert.AreEqual(2, board.Blocks[0].Shape.Count);
            Assert.AreEqual(1, board.RemainingBlocks);
        }

        [Test]
        public void HitCell_OnAGuardedBlock_TakesOneIceAndKeepsTheCell()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 3, 1).Build();
            level.Blocks[0].Ice = 2;
            var mechanics = LevelMechanics.For(level);
            var board = new Board(level, mechanics.Rules, mechanics.Guards);

            board.HitCell(new Cell(1, 0));

            Assert.AreEqual(1, mechanics.Ice.IceLeft(board, 0));
            Assert.AreEqual(1, board.Blocks.Count);
            Assert.AreEqual(3, board.Blocks[0].Shape.Count);
            Assert.AreEqual(0, board.BlockAt(new Cell(1, 0)));
        }

        [Test]
        public void ASplitPart_IsNotABlockGone_ButEachPartGoneCountsAndMeltsIce()
        {
            var level = new LevelBuilder(4, 4).Rect(Blue, 3, 3, 1, 1).Rect(Red, 0, 0, 3, 1).Build();
            level.Blocks[0].Ice = 3;
            var mechanics = LevelMechanics.For(level);
            var board = new Board(level, mechanics.Rules, mechanics.Guards);

            board.HitCell(new Cell(1, 0));
            Assert.AreEqual(3, mechanics.Ice.IceLeft(board, 0));
            Assert.AreEqual(0, mechanics.Ice.IceLeft(board, 2));

            board.HitCell(new Cell(0, 0));
            board.HitCell(new Cell(2, 0));

            Assert.AreEqual(2, board.GoneCount);
            Assert.AreEqual(1, mechanics.Ice.IceLeft(board, 0));
            Assert.AreEqual(1, board.RemainingBlocks);
        }

        [Test]
        public void ASplitPart_LeavesThroughItsDoor_AndTheLastPartGoneClearsTheBoard()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 1, 3, 1).Door(Red, Side.Right, 1).Build());

            board.HitCell(new Cell(1, 1));

            Assert.IsTrue(board.TryLeave(1, out var door));
            Assert.AreEqual(0, door);
            Assert.AreEqual(1, board.GoneCount);
            Assert.IsFalse(board.IsCleared);

            board.HitCell(new Cell(0, 1));

            Assert.IsTrue(board.IsCleared);
        }
    }
}
