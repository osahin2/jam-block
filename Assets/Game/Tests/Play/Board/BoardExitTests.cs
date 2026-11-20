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
            var board = new Board(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Door(Red, Side.Top, 1).Build());

            var result = board.Step(0, Direction.Up);

            Assert.IsTrue(result.HasLeft);
            Assert.AreEqual(0, result.ExitDoor);
            Assert.IsTrue(board.Blocks[0].HasLeft);
        }

        // Also checks the Side-indexed table in Direction: a wrong entry steps the block the wrong way.
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        [TestCase(Side.Left)]
        [TestCase(Side.Right)]
        public void Step_OutOfTheMiddleTowardADoorOnEachSide_LeavesThroughIt(Side side)
        {
            var board = new Board(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Door(Red, side, 1).Build());

            var result = board.Step(0, Direction.OutOf(side));

            Assert.IsTrue(result.HasLeft, side.ToString());
        }

        [Test]
        public void Step_AlongTheEdgeIntoTheDoor_Leaves()
        {
            var board = new Board(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Door(Red, Side.Top, 1).Build());

            var result = board.Step(0, Direction.Right);

            Assert.IsTrue(result.HasLeft);
        }

        [Test]
        public void Step_ToADoorOfAnotherColor_StaysAndTheDoorIsWall()
        {
            var board = new Board(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Door(Blue, Side.Top, 1).Build());

            var result = board.Step(0, Direction.Up);

            Assert.IsFalse(result.HasLeft);
            Assert.AreEqual(StepResult.NoDoor, result.ExitDoor);
            Assert.IsFalse(board.CanStep(0, Direction.Up));
        }

        [Test]
        public void Step_ToADoorCoveringPartOfTheBlock_Stays()
        {
            var board = new Board(new LevelBuilder(4, 3).Rect(Red, 1, 1, 2, 1).Door(Red, Side.Top, 0, 2).Build());

            var result = board.Step(0, Direction.Up);

            Assert.IsFalse(result.HasLeft);
        }

        [Test]
        public void Step_ExtentCountsAlongTheDoorSideOnly()
        {
            var board = new Board(new LevelBuilder(3, 3).Rect(Red, 0, 0, 3, 1).Door(Red, Side.Left, 1).Build());

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

            Assert.IsFalse(new Board(narrow).Step(0, Direction.Up).HasLeft);
            Assert.IsTrue(new Board(wide).Step(0, Direction.Up).HasLeft);
        }

        // The block slides out straight through the door, so a block between any of its parts and the door holds it:
        // here the L's arm touches the bottom while its other column still has a block below it.
        [Test]
        public void Step_ToTouchItsDoor_WithABlockBetweenAnotherPartAndTheDoor_Stays()
        {
            var board = new Board(new LevelBuilder(2, 3)
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
            var board = new Board(new LevelBuilder(2, 3)
                .Block(Red, 0, 0, new Cell(0, 0), new Cell(1, 0), new Cell(1, 1))
                .Door(Red, Side.Bottom, 0, 2)
                .Build());

            var result = board.Step(0, Direction.Down);

            Assert.IsTrue(result.HasLeft);
        }

        // Its own cells between a part and the door count as free.
        [Test]
        public void TryLeave_TallBlockAtItsDoor_LeavesThroughIt()
        {
            var board = new Board(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 2).Door(Red, Side.Bottom, 1).Build());

            Assert.IsTrue(board.TryLeave(0, out var exitDoor));
            Assert.AreEqual(0, exitDoor);
            Assert.IsTrue(board.Blocks[0].HasLeft);
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(1, 2)), "its cells are freed");
            Assert.IsTrue(board.IsCleared);
        }

        [Test]
        public void TryLeave_BlockAwayFromItsDoor_Stays()
        {
            var board = new Board(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Door(Red, Side.Top, 1).Build());

            Assert.IsFalse(board.TryLeave(0, out var exitDoor));
            Assert.AreEqual(StepResult.NoDoor, exitDoor);
            Assert.IsFalse(board.Blocks[0].HasLeft);
        }

        // A block that left still touches its door where it stood; a second call must not take its old cells, now
        // another block's, off the board or count it out again.
        [Test]
        public void TryLeave_BlockThatHasLeft_ChangesNothing()
        {
            var board = new Board(new LevelBuilder(3, 3)
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
            var board = new Board(new LevelBuilder(3, 4)
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
            var board = new Board(level);

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
            var board = new Board(level);

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
            var board = new Board(level);

            board.Step(0, Direction.Up);
            Assert.AreEqual(1, board.RemainingBlocks);
            Assert.IsFalse(board.IsCleared);

            board.Step(1, Direction.Up);
            Assert.AreEqual(0, board.RemainingBlocks);
            Assert.IsTrue(board.IsCleared);
        }
    }
}
