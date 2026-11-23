using Game.Logic.Play;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class LevelTimerTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void Tick_BeforeStart_DoesNotCount()
        {
            var timer = new LevelTimer(60);

            timer.Tick(5f);

            Assert.AreEqual(60f, timer.RemainingSeconds, Tolerance);
        }

        [Test]
        public void Tick_AfterStart_CountsDown()
        {
            var timer = Started(60);

            timer.Tick(1.5f);

            Assert.AreEqual(58.5f, timer.RemainingSeconds, Tolerance);
        }

        [Test]
        public void Tick_WhilePaused_DoesNotCount()
        {
            var timer = Started(60);
            timer.Pause();

            timer.Tick(5f);

            Assert.AreEqual(60f, timer.RemainingSeconds, Tolerance);
        }

        [Test]
        public void Tick_AfterResume_CountsAgain()
        {
            var timer = Started(60);
            timer.Pause();
            timer.Resume();

            timer.Tick(5f);

            Assert.AreEqual(55f, timer.RemainingSeconds, Tolerance);
        }

        [Test]
        public void Start_WhilePaused_StaysPaused()
        {
            var timer = new LevelTimer(60);
            timer.Pause();
            timer.Start();

            timer.Tick(5f);

            Assert.AreEqual(60f, timer.RemainingSeconds, Tolerance);
        }

        [Test]
        public void Tick_PastZero_StopsAtZeroAndExpires()
        {
            var timer = Started(2);

            var expired = timer.Tick(3f);

            Assert.IsTrue(expired);
            Assert.IsTrue(timer.HasExpired);
            Assert.AreEqual(0f, timer.RemainingSeconds);
        }

        [Test]
        public void Tick_ExactlyToZero_Expires()
        {
            var timer = Started(2);

            Assert.IsFalse(timer.Tick(1f));
            Assert.IsTrue(timer.Tick(1f));
        }

        [Test]
        public void Tick_AfterExpiring_ReportsItOnlyOnce()
        {
            var timer = Started(1);
            timer.Tick(1f);

            var expired = timer.Tick(1f);

            Assert.IsFalse(expired);
            Assert.IsTrue(timer.HasExpired);
        }

        [Test]
        public void Tick_WhileFrozen_DoesNotCount()
        {
            var timer = Started(60);
            timer.Freeze(10f);

            timer.Tick(4f);

            Assert.AreEqual(60f, timer.RemainingSeconds, Tolerance);
            Assert.Greater(timer.FrozenSeconds, 0f);
        }

        [Test]
        public void Tick_PastTheFreeze_CountsOnlyTheTimeAfterIt()
        {
            var timer = Started(60);
            timer.Freeze(10f);

            timer.Tick(8f);
            timer.Tick(5f);

            Assert.AreEqual(57f, timer.RemainingSeconds, Tolerance);
            Assert.AreEqual(0f, timer.FrozenSeconds);
        }

        [Test]
        public void FrozenSeconds_DrainWithCountedTime()
        {
            var timer = Started(60);
            timer.Freeze(10f);

            timer.Tick(4f);

            Assert.AreEqual(6f, timer.FrozenSeconds, Tolerance);
        }

        [Test]
        public void FrozenSeconds_PastTheFreeze_AreZero()
        {
            var timer = Started(60);
            timer.Freeze(10f);

            timer.Tick(12f);

            Assert.AreEqual(0f, timer.FrozenSeconds);
        }

        [Test]
        public void FrozenSeconds_WhilePaused_StayTheSame()
        {
            var timer = Started(60);
            timer.Freeze(10f);
            timer.Tick(3f);
            timer.Pause();

            timer.Tick(5f);

            Assert.AreEqual(7f, timer.FrozenSeconds, Tolerance);
        }

        [Test]
        public void Freeze_IsNotUsedUpWhilePaused()
        {
            var timer = Started(60);
            timer.Freeze(10f);
            timer.Pause();

            timer.Tick(20f);
            timer.Resume();
            timer.Tick(9f);

            Assert.Greater(timer.FrozenSeconds, 0f);
            Assert.AreEqual(60f, timer.RemainingSeconds, Tolerance);
        }

        [Test]
        public void Freeze_IsNotUsedUpBeforeStart()
        {
            var timer = new LevelTimer(60);
            timer.Freeze(10f);

            timer.Tick(20f);

            Assert.Greater(timer.FrozenSeconds, 0f);
        }

        private static LevelTimer Started(int limitSeconds)
        {
            var timer = new LevelTimer(limitSeconds);
            timer.Start();
            return timer;
        }
    }
}
