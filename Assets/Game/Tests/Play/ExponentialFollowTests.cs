using Game.Play.Presentation;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class ExponentialFollowTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void Fraction_AfterOneHalfLife_CoversHalfTheDistance()
        {
            Assert.AreEqual(0.5f, ExponentialFollow.Fraction(0.06f, 0.06f), Tolerance);
        }

        [Test]
        public void Fraction_TwoShortFramesEqualOneLongFrame()
        {
            var shortFrame = ExponentialFollow.Fraction(0.05f, 1f / 60f);
            var remainingAfterTwo = (1f - shortFrame) * (1f - shortFrame);

            Assert.AreEqual(1f - ExponentialFollow.Fraction(0.05f, 1f / 30f), remainingAfterTwo, Tolerance);
        }

        [Test]
        public void Fraction_LongFrame_NeverPassesTheTarget()
        {
            var fraction = ExponentialFollow.Fraction(0.03f, 1f);

            Assert.LessOrEqual(fraction, 1f);
            Assert.Greater(fraction, 0.99f);
        }
    }
}
