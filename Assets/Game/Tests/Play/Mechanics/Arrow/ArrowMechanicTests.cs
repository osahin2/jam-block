using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class ArrowMechanicTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void AHorizontalArrow_StepsLeftAndRightOnly()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Arrow(ArrowAxis.Horizontal).Build());

            Assert.IsTrue(board.CanStep(0, Direction.Left));
            Assert.IsTrue(board.CanStep(0, Direction.Right));
            Assert.IsFalse(board.CanStep(0, Direction.Up));
            Assert.IsFalse(board.CanStep(0, Direction.Down));
            Assert.IsFalse(board.IsLocked(0));
        }

        [Test]
        public void AVerticalArrow_StepsUpAndDownOnly()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Arrow(ArrowAxis.Vertical).Build());

            Assert.IsTrue(board.CanStep(0, Direction.Up));
            Assert.IsTrue(board.CanStep(0, Direction.Down));
            Assert.IsFalse(board.CanStep(0, Direction.Left));
            Assert.IsFalse(board.CanStep(0, Direction.Right));
        }

        [Test]
        public void ABlockWithoutAnArrow_StepsAnyWay()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Build());

            Assert.IsTrue(board.CanStep(0, Direction.Up));
            Assert.IsTrue(board.CanStep(0, Direction.Left));
        }

        [Test]
        public void AHorizontalArrow_LeavesThroughADoorOnItsAxisOnly()
        {
            var onTop = Boards.Of(new LevelBuilder(3, 3)
                .Rect(Red, 2, 0, 1, 1).Arrow(ArrowAxis.Horizontal)
                .Door(Red, Side.Top, 2)
                .Build());
            var onRight = Boards.Of(new LevelBuilder(3, 3)
                .Rect(Red, 2, 0, 1, 1).Arrow(ArrowAxis.Horizontal)
                .Door(Red, Side.Right, 0)
                .Build());

            Assert.IsFalse(onTop.TryLeave(0, out _));
            Assert.IsTrue(onRight.TryLeave(0, out _));
        }

        [Test]
        public void AnIcedArrowBlock_IsLockedUntilItsIceMelts()
        {
            var board = Boards.Of(new LevelBuilder(3, 3)
                .Rect(Red, 1, 1, 1, 1).Arrow(ArrowAxis.Vertical).Ice(1)
                .Rect(Blue, 0, 0, 1, 1)
                .Build());
            Assert.IsTrue(board.IsLocked(0));

            board.Hit(1);

            Assert.IsFalse(board.IsLocked(0));
            Assert.IsTrue(board.CanStep(0, Direction.Down));
            Assert.IsFalse(board.CanStep(0, Direction.Right));
        }
    }
}
