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

            var grabbed = attempt.Grab(0.5f, 1.5f, out _);
            attempt.Tick(5f);

            Assert.IsTrue(grabbed);
            Assert.AreEqual(0, attempt.Release());
            Assert.AreEqual(TimeLimit - 5f, attempt.RemainingSeconds, Tolerance);
        }

        [Test]
        public void HeldBlock_IsTheGrabbedBlockUntilRelease_WhileThePoseNamesNone()
        {
            var attempt = TwoBlocks();
            Assert.AreEqual(Board.NoBlock, attempt.HeldBlock);

            attempt.Grab(2.5f, 1.5f, out var pose);

            Assert.AreEqual(1, attempt.HeldBlock);
            Assert.AreEqual(Board.NoBlock, pose.Block);
            attempt.Release();
            Assert.AreEqual(Board.NoBlock, attempt.HeldBlock);
        }

        [Test]
        public void Grab_OnAnEmptyCell_HoldsNothingButStartsTheTimer()
        {
            var attempt = TwoBlocks();

            var grabbed = attempt.Grab(1.5f, 1.5f, out _);
            attempt.Tick(5f);

            Assert.IsFalse(grabbed);
            Assert.AreEqual(Board.NoBlock, attempt.Release());
            Assert.AreEqual(TimeLimit - 5f, attempt.RemainingSeconds, Tolerance);
        }

        [Test]
        public void Grab_OutsideTheBoard_DoesNotStartTheTimer()
        {
            var attempt = TwoBlocks();

            attempt.Grab(-0.5f, 1.5f, out _);
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
            attempt.Grab(0.5f, 1.5f, out _);

            var moved = attempt.MoveTo(1.7f, 1.5f, out var pose);

            Assert.IsTrue(moved);
            Assert.AreEqual(0, pose.Block);
            Assert.AreEqual(new Cell(1, 1), pose.Position);
        }

        [Test]
        public void MoveTo_ABlockOutWhileOthersRemain_KeepsPlaying()
        {
            var attempt = TwoBlocks();
            attempt.Grab(0.5f, 1.5f, out _);

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
            attempt.Grab(2.5f, 1.5f, out _);

            attempt.MoveTo(2.5f, 2.1f, out _);
            var outcome = attempt.Tick(TimeLimit);

            Assert.AreEqual(LevelOutcome.Won, outcome);
            Assert.AreEqual(TimeLimit, attempt.RemainingSeconds, Tolerance);
        }

        [Test]
        public void MoveTo_TheLastBlockOut_WinsBeforeTheNextTick()
        {
            var attempt = TwoBlocks();
            LeaveFirstBlock(attempt);
            attempt.Grab(2.5f, 1.5f, out _);

            attempt.MoveTo(2.5f, 2.1f, out _);

            Assert.AreEqual(LevelOutcome.Won, attempt.Outcome);
        }

        [Test]
        public void Tick_AfterTheLevelFailed_ReturnsFailedAgain()
        {
            var attempt = TwoBlocks();
            attempt.Grab(1.5f, 1.5f, out _);
            attempt.Tick(TimeLimit);

            Assert.AreEqual(LevelOutcome.Failed, attempt.Tick(1f));
        }

        [Test]
        public void Grab_OnABlockAtItsDoor_LeavesAtOnceWithoutHoldingIt()
        {
            var attempt = new LevelAttempt(new LevelBuilder(3, 3)
                .Rect(Red, 0, 0, 1, 1)
                .Rect(Blue, 2, 1, 1, 1)
                .Door(Red, Side.Top, 0)
                .Door(Blue, Side.Bottom, 2)
                .Build());

            var grabbed = attempt.Grab(0.5f, 0.5f, out var pose);

            Assert.IsTrue(grabbed);
            Assert.IsTrue(pose.HasLeft);
            Assert.AreEqual(0, pose.Block);
            Assert.AreEqual(0, pose.ExitDoor);
            Assert.AreEqual(Board.NoBlock, attempt.Release(), "a block that left is not held");
            Assert.AreEqual(LevelOutcome.Playing, attempt.Outcome);
        }

        [Test]
        public void Grab_OnTheLastBlockAtItsDoor_Wins()
        {
            var attempt = new LevelAttempt(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Door(Red, Side.Top, 0).Build());

            attempt.Grab(0.5f, 0.5f, out _);

            Assert.AreEqual(LevelOutcome.Won, attempt.Outcome);
        }

        // A block between the door and the L's other part keeps the L on the board.
        [Test]
        public void Grab_OnABlockAtItsDoorWithABlockInTheWay_HoldsIt()
        {
            var attempt = new LevelAttempt(new LevelBuilder(2, 3)
                .Block(Red, 0, 1, new Cell(0, 0), new Cell(1, 0), new Cell(1, 1))
                .Rect(Blue, 0, 2, 1, 1)
                .Door(Red, Side.Bottom, 0, 2)
                .Build());

            var grabbed = attempt.Grab(1.5f, 2.5f, out var pose);

            Assert.IsTrue(grabbed);
            Assert.IsFalse(pose.HasLeft);
            Assert.AreEqual(0, attempt.Release(), "the red block is held");
        }

        [Test]
        public void Grab_OnABlockAwayFromItsDoor_LeavesNothing()
        {
            var attempt = TwoBlocks();

            attempt.Grab(0.5f, 1.5f, out var pose);

            Assert.IsFalse(pose.HasLeft);
        }

        [Test]
        public void Tick_ToZero_Fails()
        {
            var attempt = TwoBlocks();
            attempt.Grab(1.5f, 1.5f, out _);

            var outcome = attempt.Tick(TimeLimit);

            Assert.AreEqual(LevelOutcome.Failed, outcome);
            Assert.AreEqual(LevelOutcome.Failed, attempt.Outcome);
        }

        [Test]
        public void Tick_ToZeroWhileHolding_KeepsTheBlockUntilReleaseButStopsItsMoves()
        {
            var attempt = TwoBlocks();
            attempt.Grab(0.5f, 1.5f, out _);
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
            attempt.Grab(1.5f, 1.5f, out _);
            attempt.Tick(TimeLimit);

            Assert.IsFalse(attempt.Grab(0.5f, 1.5f, out _));
        }

        [Test]
        public void Pause_StopsTheTimerAndTheBoard()
        {
            var attempt = TwoBlocks();
            attempt.Grab(0.5f, 1.5f, out _);
            attempt.Pause();

            attempt.Tick(5f);
            var moved = attempt.MoveTo(1.7f, 1.5f, out _);

            Assert.AreEqual(TimeLimit, attempt.RemainingSeconds, Tolerance);
            Assert.IsFalse(moved);
            Assert.AreEqual(0, attempt.Release());
            Assert.IsFalse(attempt.Grab(0.5f, 1.5f, out _));
        }

        [Test]
        public void Resume_StartsTheTimerAndTheBoardAgain()
        {
            var attempt = TwoBlocks();
            attempt.Grab(1.5f, 1.5f, out _);
            attempt.Pause();
            attempt.Resume();

            attempt.Tick(5f);

            Assert.AreEqual(TimeLimit - 5f, attempt.RemainingSeconds, Tolerance);
            Assert.IsTrue(attempt.Grab(0.5f, 1.5f, out _));
        }

        [Test]
        public void IsPaused_FollowsPauseAndResume()
        {
            var attempt = TwoBlocks();

            attempt.Pause();
            var paused = attempt.IsPaused;
            attempt.Resume();

            Assert.IsTrue(paused);
            Assert.IsFalse(attempt.IsPaused);
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
            attempt.Grab(0.5f, 1.5f, out _);
            attempt.MoveTo(0.5f, 0.9f, out _);
        }
    }
}
