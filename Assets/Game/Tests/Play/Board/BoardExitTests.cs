using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class BoardExitTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void Step_ToTouchACoveringDoorOfItsColor_LeavesThroughIt()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Door(Red, Side.Top, 1).Build());

            var result = board.Step(0, Direction.Up);

            Assert.IsTrue(result.HasLeft);
            Assert.AreEqual(0, result.ExitDoor);
            Assert.IsTrue(board.Blocks[0].IsGone);
        }

        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        [TestCase(Side.Left)]
        [TestCase(Side.Right)]
        public void Step_OutOfTheMiddleTowardADoorOnEachSide_LeavesThroughIt(Side side)
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Door(Red, side, 1).Build());

            var result = board.Step(0, Direction.OutOf(side));

            Assert.IsTrue(result.HasLeft, side.ToString());
        }

        [Test]
        public void Step_AlongTheEdgeIntoTheDoor_Leaves()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Door(Red, Side.Top, 1).Build());

            var result = board.Step(0, Direction.Right);

            Assert.IsTrue(result.HasLeft);
        }

        [Test]
        public void Step_ToADoorOfAnotherColor_StaysAndTheDoorIsWall()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Door(Blue, Side.Top, 1).Build());

            var result = board.Step(0, Direction.Up);

            Assert.IsFalse(result.HasLeft);
            Assert.AreEqual(StepResult.NoDoor, result.ExitDoor);
            Assert.IsFalse(board.CanStep(0, Direction.Up));
        }

        [Test]
        public void Step_ToADoorCoveringPartOfTheBlock_Stays()
        {
            var board = Boards.Of(new LevelBuilder(4, 3).Rect(Red, 1, 1, 2, 1).Door(Red, Side.Top, 0, 2).Build());

            var result = board.Step(0, Direction.Up);

            Assert.IsFalse(result.HasLeft);
        }

        [Test]
        public void Step_ExtentCountsAlongTheDoorSideOnly()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 0, 0, 3, 1).Door(Red, Side.Left, 1).Build());

            var result = board.Step(0, Direction.Down);

            Assert.IsTrue(result.HasLeft);
        }

        [Test]
        public void Step_LShapeIsCoveredByItsWholeExtent()
        {
            var narrow = new LevelBuilder(3, 3)
                .Block(Red, 0, 1, new Cell(0, 0), new Cell(0, 1), new Cell(1, 1))
                .Door(Red, Side.Top, 0)
                .Build();
            var wide = new LevelBuilder(3, 3)
                .Block(Red, 0, 1, new Cell(0, 0), new Cell(0, 1), new Cell(1, 1))
                .Door(Red, Side.Top, 0, 2)
                .Build();

            Assert.IsFalse(Boards.Of(narrow).Step(0, Direction.Up).HasLeft);
            Assert.IsTrue(Boards.Of(wide).Step(0, Direction.Up).HasLeft);
        }

        [Test]
        public void Step_ToTouchItsDoor_WithABlockBetweenAnotherPartAndTheDoor_Stays()
        {
            var board = Boards.Of(new LevelBuilder(2, 3)
                .Block(Red, 0, 0, new Cell(0, 0), new Cell(1, 0), new Cell(1, 1))
                .Rect(Blue, 0, 2, 1, 1)
                .Door(Red, Side.Bottom, 0, 2)
                .Build());

            var result = board.Step(0, Direction.Down);

            Assert.IsTrue(result.Moved);
            Assert.IsFalse(result.HasLeft);
            Assert.AreEqual(0, board.BlockAt(new Cell(1, 2)), "the red block stays on the board");
        }

        [Test]
        public void Step_ToTouchItsDoor_WithTheWayToTheDoorFree_Leaves()
        {
            var board = Boards.Of(new LevelBuilder(2, 3)
                .Block(Red, 0, 0, new Cell(0, 0), new Cell(1, 0), new Cell(1, 1))
                .Door(Red, Side.Bottom, 0, 2)
                .Build());

            var result = board.Step(0, Direction.Down);

            Assert.IsTrue(result.HasLeft);
        }

        [Test]
        public void TryLeave_TallBlockAtItsDoor_LeavesThroughIt()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 2).Door(Red, Side.Bottom, 1).Build());

            Assert.IsTrue(board.TryLeave(0, out var exitDoor));
            Assert.AreEqual(0, exitDoor);
            Assert.IsTrue(board.Blocks[0].IsGone);
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(1, 2)), "its cells are freed");
            Assert.IsTrue(board.IsCleared);
        }

        [Test]
        public void TryLeave_BlockAwayFromItsDoor_Stays()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Door(Red, Side.Top, 1).Build());

            Assert.IsFalse(board.TryLeave(0, out var exitDoor));
            Assert.AreEqual(StepResult.NoDoor, exitDoor);
            Assert.IsFalse(board.Blocks[0].IsGone);
        }

        [Test]
        public void TryLeave_BlockThatHasLeft_ChangesNothing()
        {
            var board = Boards.Of(new LevelBuilder(3, 3)
                .Rect(Red, 1, 0, 1, 1)
                .Rect(Blue, 2, 1, 1, 1)
                .Door(Red, Side.Top, 1)
                .Build());
            board.TryLeave(0, out _);
            board.Step(1, Direction.Left);
            board.Step(1, Direction.Up);

            Assert.IsFalse(board.TryLeave(0, out var exitDoor));
            Assert.AreEqual(StepResult.NoDoor, exitDoor);
            Assert.AreEqual(1, board.RemainingBlocks);
            Assert.AreEqual(1, board.BlockAt(new Cell(1, 0)), "the blue block keeps the cell the red one left");
        }

        [Test]
        public void TryLeave_AfterTheBlockInTheWayMovedOff_Leaves()
        {
            var board = Boards.Of(new LevelBuilder(3, 4)
                .Block(Red, 0, 2, new Cell(0, 0), new Cell(1, 0), new Cell(0, 1))
                .Rect(Blue, 1, 3, 1, 1)
                .Door(Red, Side.Bottom, 0, 2)
                .Build());
            Assert.IsFalse(board.TryLeave(0, out _), "the blue block is under the red one's other part");

            board.Step(1, Direction.Right);

            Assert.IsTrue(board.TryLeave(0, out var exitDoor));
            Assert.AreEqual(0, exitDoor);
        }

        [Test]
        public void Step_ColorWithTwoDoors_LeavesThroughTheOneItTouches()
        {
            var level = new LevelBuilder(1, 3)
                .Rect(Red, 0, 1, 1, 1)
                .Door(Red, Side.Top, 0)
                .Door(Red, Side.Bottom, 0)
                .Build();
            var board = Boards.Of(level);

            var result = board.Step(0, Direction.Down);

            Assert.AreEqual(1, result.ExitDoor);
        }

        [Test]
        public void Step_BlockThatLeft_FreesItsCellsAndCannotMove()
        {
            var level = new LevelBuilder(3, 3)
                .Rect(Red, 1, 1, 1, 1)
                .Rect(Blue, 1, 2, 1, 1)
                .Door(Red, Side.Top, 1)
                .Build();
            var board = Boards.Of(level);

            board.Step(0, Direction.Up);

            Assert.IsFalse(board.CanStep(0, Direction.Down));
            Assert.IsTrue(board.CanStep(1, Direction.Up));
        }

        [Test]
        public void IsCleared_OnlyAfterTheLastBlockLeaves()
        {
            var level = new LevelBuilder(3, 3)
                .Rect(Red, 0, 1, 1, 1)
                .Rect(Blue, 2, 1, 1, 1)
                .Door(Red, Side.Top, 0)
                .Door(Blue, Side.Top, 2)
                .Build();
            var board = Boards.Of(level);

            board.Step(0, Direction.Up);
            Assert.AreEqual(1, board.RemainingBlocks);
            Assert.IsFalse(board.IsCleared);

            board.Step(1, Direction.Up);
            Assert.AreEqual(0, board.RemainingBlocks);
            Assert.IsTrue(board.IsCleared);
        }
    }
}
