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
