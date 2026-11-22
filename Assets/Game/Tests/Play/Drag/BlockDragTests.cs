using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class BlockDragTests
    {
        private const int Red = 1;
        private const int Blue = 2;
        private const float Tolerance = 0.0001f;

        [Test]
        public void MoveTo_LessThanHalfACellTowardAFreeCell_FollowsTheFinger()
        {
            var drag = Grab(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Build(), 1.5f, 1.5f);

            var pose = drag.MoveTo(1.8f, 1.1f);

            Assert.AreEqual(new Cell(1, 1), pose.Position);
            Assert.AreEqual(0.3f, pose.OffsetX, Tolerance);
            Assert.AreEqual(-0.4f, pose.OffsetY, Tolerance);
        }

        [Test]
        public void MoveTo_TowardABlockedCell_DoesNotOffset()
        {
            var drag = Grab(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Blue, 1, 0, 1, 1).Build(), 0.5f, 0.5f);

            var pose = drag.MoveTo(0.9f, 0.5f);

            Assert.AreEqual(0f, pose.OffsetX, Tolerance);
        }

        [Test]
        public void MoveTo_PastHalfACell_StepsAndKeepsTheRest()
        {
            var drag = Grab(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Build(), 0.5f, 0.5f);

            var pose = drag.MoveTo(1.2f, 0.5f);

            Assert.AreEqual(new Cell(1, 0), pose.Position);
            Assert.AreEqual(-0.3f, pose.OffsetX, Tolerance);
        }

        [Test]
        public void MoveTo_SeveralCellsAtOnce_StepsEachCell()
        {
            var drag = Grab(new LevelBuilder(5, 1).Rect(Red, 0, 0, 1, 1).Build(), 0.5f, 0.5f);

            var pose = drag.MoveTo(3.7f, 0.5f);

            Assert.AreEqual(new Cell(3, 0), pose.Position);
            Assert.AreEqual(0.2f, pose.OffsetX, Tolerance);
        }

        [Test]
        public void MoveTo_PastAnotherBlock_StopsInFrontOfIt()
        {
            var drag = Grab(new LevelBuilder(5, 1).Rect(Red, 0, 0, 1, 1).Rect(Blue, 3, 0, 1, 1).Build(), 0.5f, 0.5f);

            var pose = drag.MoveTo(4.5f, 0.5f);

            Assert.AreEqual(new Cell(2, 0), pose.Position);
            Assert.AreEqual(0f, pose.OffsetX, Tolerance);
        }

        [Test]
        public void MoveTo_FingerPastAnObstacle_GoesAroundItOneAxisAtATime()
        {
            var drag = Grab(new LevelBuilder(3, 3).Rect(Red, 0, 1, 1, 1).Rect(Blue, 1, 1, 1, 1).Build(), 0.5f, 1.5f);

            var pose = drag.MoveTo(2.5f, 0.5f);

            Assert.AreEqual(new Cell(2, 0), pose.Position);
        }

        [Test]
        public void MoveTo_KeepsWhereTheBlockWasGrabbed()
        {
            var drag = Grab(new LevelBuilder(3, 1).Rect(Red, 0, 0, 2, 1).Build(), 1.7f, 0.5f);

            var pose = drag.MoveTo(2.0f, 0.5f);

            Assert.AreEqual(new Cell(0, 0), pose.Position);
            Assert.AreEqual(0.3f, pose.OffsetX, Tolerance);
        }

        [Test]
        public void MoveTo_ToItsDoor_LeavesAndEndsTheDrag()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 1, 2, 1, 1).Door(Red, Side.Top, 1).Build();
            var drag = Grab(level, 1.5f, 2.5f);

            var pose = drag.MoveTo(1.5f, 0.5f);

            Assert.IsTrue(pose.HasLeft);
            Assert.AreEqual(0, pose.ExitDoor);
            Assert.AreEqual(new Cell(1, 0), pose.Position);
            Assert.IsFalse(drag.IsActive);
        }

        private static BlockDrag Grab(LevelData level, float x, float y)
        {
            var board = Boards.Of(level);
            var drag = new BlockDrag(board);
            drag.Begin(board.BlockAt(new Cell((int)x, (int)y)), x, y);
            return drag;
        }
    }
}
