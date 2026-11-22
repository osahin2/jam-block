using Game.Play;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class TimeWarningTests
    {
        [TestCase(30, true)]
        [TestCase(1, true)]
        [TestCase(31, false)]
        [TestCase(0, false)]
        public void Flashes_OnlyInTheLastSecondsBeforeZero(int wholeSeconds, bool expected)
        {
            Assert.AreEqual(expected, TimeWarning.Flashes(wholeSeconds, 30));
        }
    }
}
