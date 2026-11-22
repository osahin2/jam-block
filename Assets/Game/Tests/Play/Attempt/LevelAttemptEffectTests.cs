using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class LevelAttemptEffectTests
    {
        private const int Red = 1;
        private const int Blue = 2;
        private const int TimeLimit = 60;
        private const float Tolerance = 0.0001f;

        private BreakTargetEffect _effect;

        [SetUp]
        public void SetUp() => _effect = new BreakTargetEffect();

        [Test]
        public void CanUseBoosters_BeforeTheFirstTouch_IsFalse_AndAimIsRefused()
        {
            var attempt = TwoBlocks();

            Assert.IsFalse(attempt.CanUseBoosters);
            Assert.IsFalse(attempt.Aim());
            Assert.IsFalse(attempt.IsAiming);
        }

        [Test]
        public void CanUseBoosters_AfterTheFirstTouch_IsTrue()
        {
            var attempt = Touched(TwoBlocks());

            Assert.IsTrue(attempt.CanUseBoosters);
            Assert.IsTrue(attempt.Aim());
            Assert.IsTrue(attempt.IsAiming);
        }

        [Test]
        public void TryApply_WithoutAiming_AppliesNothing()
        {
            var attempt = Touched(TwoBlocks());

            Assert.IsFalse(attempt.TryApply(_effect, 0.5f, 1.5f));
            Assert.AreEqual(0, _effect.Applied);
        }

        [Test]
        public void TryApply_OnAValidTarget_AppliesTheEffectAndEndsTheAim()
        {
            var attempt = Touched(TwoBlocks());
            attempt.Aim();

            Assert.IsTrue(attempt.TryApply(_effect, 0.5f, 1.5f));

            Assert.AreEqual(1, _effect.Applied);
            Assert.IsTrue(attempt.Board.Blocks[0].IsGone);
            Assert.IsFalse(attempt.IsAiming);
            Assert.AreEqual(LevelOutcome.Playing, attempt.Outcome);
        }

        [TestCase(1.5f, 0.5f)]
        [TestCase(-0.5f, 1.5f)]
        public void TryApply_OnACellThatIsNoTarget_AppliesNothingAndKeepsAiming(float x, float y)
        {
            var attempt = Touched(TwoBlocks());
            attempt.Aim();

            Assert.IsFalse(attempt.TryApply(_effect, x, y));

            Assert.AreEqual(0, _effect.Applied);
            Assert.IsTrue(attempt.IsAiming);
        }

        [Test]
        public void TryApply_TakingTheLastBlock_WinsBeforeTheNextTick()
        {
            var attempt = Touched(AttemptAt(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Build()));
            attempt.Aim();

            attempt.TryApply(_effect, 0.5f, 0.5f);

            Assert.AreEqual(LevelOutcome.Won, attempt.Outcome);
            Assert.IsFalse(attempt.CanUseBoosters);
        }

        [Test]
        public void Aim_ReleasesTheHeldBlock_AndGrabsDoNothingWhileAiming()
        {
            var attempt = TwoBlocks();
            attempt.Grab(0.5f, 1.5f, out _);

            attempt.Aim();

            Assert.AreEqual(Board.NoBlock, attempt.HeldBlock);
            Assert.IsFalse(attempt.Grab(2.5f, 1.5f, out _));
            Assert.AreEqual(Board.NoBlock, attempt.HeldBlock);
        }

        [Test]
        public void CancelAim_TurnsTheDragBackOn()
        {
            var attempt = Touched(TwoBlocks());
            attempt.Aim();

            attempt.CancelAim();

            Assert.IsFalse(attempt.IsAiming);
            Assert.IsTrue(attempt.Grab(2.5f, 1.5f, out _));
            Assert.IsFalse(attempt.TryApply(_effect, 2.5f, 1.5f));
        }

        [Test]
        public void Aiming_KeepsTheTimeRunning()
        {
            var attempt = Touched(TwoBlocks());
            attempt.Aim();

            attempt.Tick(5f);
            attempt.CancelAim();
            attempt.Tick(5f);

            Assert.AreEqual(TimeLimit - 10f, attempt.RemainingSeconds, Tolerance);
        }

        [Test]
        public void TryApply_AfterTheLevelFailed_AppliesNothing()
        {
            var attempt = Touched(TwoBlocks());
            attempt.Aim();

            attempt.Tick(TimeLimit);

            Assert.IsFalse(attempt.TryApply(_effect, 0.5f, 1.5f));
            Assert.AreEqual(0, _effect.Applied);
            Assert.IsFalse(attempt.CanUseBoosters);
        }

        [Test]
        public void TryApply_WhilePaused_AppliesNothing()
        {
            var attempt = Touched(TwoBlocks());
            attempt.Aim();
            attempt.Pause();

            Assert.IsFalse(attempt.TryApply(_effect, 0.5f, 1.5f));
            Assert.AreEqual(0, _effect.Applied);
        }

        [Test]
        public void Freeze_BeforeTheFirstTouch_IsRefused()
        {
            var attempt = TwoBlocks();

            Assert.IsFalse(attempt.Freeze(10f));
            Assert.IsFalse(attempt.IsFrozen);
        }

        [Test]
        public void Freeze_StopsTheTimeForItsSeconds()
        {
            var attempt = Touched(TwoBlocks());

            Assert.IsTrue(attempt.Freeze(10f));
            attempt.Tick(12f);

            Assert.AreEqual(TimeLimit - 2f, attempt.RemainingSeconds, Tolerance);
            Assert.IsFalse(attempt.IsFrozen);
        }

        [Test]
        public void FrozenSeconds_OnceTheLevelIsWon_AreZero()
        {
            var attempt = Touched(AttemptAt(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Build()));
            attempt.Freeze(10f);
            attempt.Aim();

            attempt.TryApply(_effect, 0.5f, 0.5f);

            Assert.AreEqual(0f, attempt.FrozenSeconds);
            Assert.IsFalse(attempt.IsFrozen);
        }

        [Test]
        public void Freeze_WhileFrozen_IsRefused()
        {
            var attempt = Touched(TwoBlocks());
            attempt.Freeze(10f);
            attempt.Tick(4f);

            Assert.IsFalse(attempt.CanFreeze);
            Assert.IsFalse(attempt.Freeze(10f));
            attempt.Tick(7f);
            Assert.IsTrue(attempt.CanFreeze);

            Assert.AreEqual(TimeLimit - 1f, attempt.RemainingSeconds, Tolerance);
        }

        [Test]
        public void Freeze_AfterTheLevelFailed_IsRefused()
        {
            var attempt = Touched(TwoBlocks());
            attempt.Tick(TimeLimit);

            Assert.IsFalse(attempt.Freeze(10f));
        }

        private static LevelAttempt Touched(LevelAttempt attempt)
        {
            attempt.Grab(1.5f, 0.5f, out _);
            attempt.Release();
            return attempt;
        }

        private static LevelAttempt TwoBlocks() =>
            AttemptAt(new LevelBuilder(3, 3).Rect(Red, 0, 1, 1, 1).Rect(Blue, 2, 1, 1, 1).Door(Red, Side.Top, 0).Build());

        private static LevelAttempt AttemptAt(LevelData level)
        {
            level.TimeLimitSeconds = TimeLimit;
            return new LevelAttempt(level);
        }
    }
}
