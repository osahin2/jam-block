using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class LevelAttemptTests
    {
        private const int Red = 1;
        private const int Blue = 2;
        private const int TimeLimit = 60;
        private const float Tolerance = 0.0001f;

        [Test]
        public void Tick_BeforeTheFirstTouch_DoesNotCount()
        {
            var attempt = TwoBlocks();

            attempt.Tick(5f);

            Assert.AreEqual(TimeLimit, attempt.RemainingSeconds, Tolerance);
        }

        [Test]
        public void Grab_OnABlock_HoldsItAndStartsTheTimer()
        {
            var attempt = TwoBlocks();

            var grabbed = attempt.Grab(0.5f, 1.5f);
            attempt.Tick(5f);

            Assert.IsTrue(grabbed);
            Assert.AreEqual(0, attempt.Release());
            Assert.AreEqual(TimeLimit - 5f, attempt.RemainingSeconds, Tolerance);
        }

        [Test]
        public void Grab_OnAnEmptyCell_HoldsNothingButStartsTheTimer()
        {
            var attempt = TwoBlocks();

            var grabbed = attempt.Grab(1.5f, 1.5f);
            attempt.Tick(5f);

            Assert.IsFalse(grabbed);
            Assert.AreEqual(Board.NoBlock, attempt.Release());
            Assert.AreEqual(TimeLimit - 5f, attempt.RemainingSeconds, Tolerance);
        }

        [Test]
        public void Grab_OutsideTheBoard_DoesNotStartTheTimer()
        {
            var attempt = TwoBlocks();

            attempt.Grab(-0.5f, 1.5f);
            attempt.Tick(5f);

            Assert.AreEqual(TimeLimit, attempt.RemainingSeconds, Tolerance);
        }

        [Test]
        public void MoveTo_WithNothingHeld_ReportsNoPose()
        {
            var attempt = TwoBlocks();

            Assert.IsFalse(attempt.MoveTo(1.5f, 1.5f, out var pose));
            Assert.IsFalse(pose.HasLeft, "an empty pose must not read as having left through door 0");
            Assert.AreEqual(LevelAttempt.NoBlock, pose.Block);
        }

        [Test]
        public void MoveTo_WithABlockHeld_ReturnsItsPose()
        {
            var attempt = TwoBlocks();
            attempt.Grab(0.5f, 1.5f);

            var moved = attempt.MoveTo(1.7f, 1.5f, out var pose);

            Assert.IsTrue(moved);
            Assert.AreEqual(0, pose.Block);
            Assert.AreEqual(new Cell(1, 1), pose.Position);
        }

        [Test]
        public void MoveTo_ABlockOutWhileOthersRemain_KeepsPlaying()
        {
            var attempt = TwoBlocks();
            attempt.Grab(0.5f, 1.5f);

            attempt.MoveTo(0.5f, 0.9f, out var pose);

            Assert.IsTrue(pose.HasLeft);
            Assert.AreEqual(0, pose.Block);
            Assert.AreEqual(LevelOutcome.Playing, attempt.Outcome);
            Assert.AreEqual(Board.NoBlock, attempt.Release());
        }

        [Test]
        public void MoveTo_TheLastBlockOut_WinsAndStopsTheTimer()
        {
            var attempt = TwoBlocks();
            LeaveFirstBlock(attempt);
            attempt.Grab(2.5f, 1.5f);

            attempt.MoveTo(2.5f, 2.1f, out _);
            var outcome = attempt.Tick(TimeLimit);

            Assert.AreEqual(LevelOutcome.Won, outcome);
            Assert.AreEqual(TimeLimit, attempt.RemainingSeconds, Tolerance);
        }

        [Test]
        public void Tick_ToZero_Fails()
        {
            var attempt = TwoBlocks();
            attempt.Grab(1.5f, 1.5f);

            var outcome = attempt.Tick(TimeLimit);

            Assert.AreEqual(LevelOutcome.Failed, outcome);
            Assert.AreEqual(LevelOutcome.Failed, attempt.Outcome);
        }

        [Test]
        public void Tick_ToZeroWhileHolding_KeepsTheBlockUntilReleaseButStopsItsMoves()
        {
            var attempt = TwoBlocks();
            attempt.Grab(0.5f, 1.5f);
            attempt.Tick(TimeLimit);

            var moved = attempt.MoveTo(1.7f, 1.5f, out _);
            var released = attempt.Release();

            Assert.IsFalse(moved);
            Assert.AreEqual(0, released);
            Assert.AreEqual(new Cell(0, 1), attempt.PositionOf(0));
        }

        [Test]
        public void Grab_AfterTheLevelIsOver_HoldsNothing()
        {
            var attempt = TwoBlocks();
            attempt.Grab(1.5f, 1.5f);
            attempt.Tick(TimeLimit);

            Assert.IsFalse(attempt.Grab(0.5f, 1.5f));
        }

        [Test]
        public void Pause_StopsTheTimerAndTheBoard()
        {
            var attempt = TwoBlocks();
            attempt.Grab(0.5f, 1.5f);
            attempt.Pause();

            attempt.Tick(5f);
            var moved = attempt.MoveTo(1.7f, 1.5f, out _);

            Assert.AreEqual(TimeLimit, attempt.RemainingSeconds, Tolerance);
            Assert.IsFalse(moved);
            Assert.AreEqual(0, attempt.Release());
            Assert.IsFalse(attempt.Grab(0.5f, 1.5f));
        }

        [Test]
        public void Resume_StartsTheTimerAndTheBoardAgain()
        {
            var attempt = TwoBlocks();
            attempt.Grab(1.5f, 1.5f);
            attempt.Pause();
            attempt.Resume();

            attempt.Tick(5f);

            Assert.AreEqual(TimeLimit - 5f, attempt.RemainingSeconds, Tolerance);
            Assert.IsTrue(attempt.Grab(0.5f, 1.5f));
        }

        [Test]
        public void Pause_BeforeTheFirstTouch_KeepsTheTimerStoppedAfterResume()
        {
            var attempt = TwoBlocks();
            attempt.Pause();
            attempt.Resume();

            attempt.Tick(5f);

            Assert.AreEqual(TimeLimit, attempt.RemainingSeconds, Tolerance);
        }

        [Test]
        public void Release_WithNothingHeld_ReturnsNoBlock()
        {
            Assert.AreEqual(Board.NoBlock, TwoBlocks().Release());
        }

        // A red block on the middle row one step below a red door, a blue one one step above a blue door.
        private static LevelAttempt TwoBlocks()
        {
            var level = new LevelBuilder(3, 3)
                .Rect(Red, 0, 1, 1, 1)
                .Rect(Blue, 2, 1, 1, 1)
                .Door(Red, Side.Top, 0)
                .Door(Blue, Side.Bottom, 2)
                .Build();
            level.TimeLimitSeconds = TimeLimit;
            return new LevelAttempt(level);
        }

        private static void LeaveFirstBlock(LevelAttempt attempt)
        {
            attempt.Grab(0.5f, 1.5f);
            attempt.MoveTo(0.5f, 0.9f, out _);
        }
    }
}
