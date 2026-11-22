using Game.Play;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class CountdownTextTests
    {
        [TestCase(300f, 300)]
        [TestCase(299.01f, 300)]
        [TestCase(0.01f, 1)]
        [TestCase(0f, 0)]
        [TestCase(-0.5f, 0)]
        public void WholeSeconds_RoundsUpAndStopsAtZero(float remaining, int expected)
        {
            Assert.AreEqual(expected, CountdownText.WholeSeconds(remaining));
        }

        [TestCase(300, "05:00")]
        [TestCase(61, "01:01")]
        [TestCase(9, "00:09")]
        [TestCase(0, "00:00")]
        [TestCase(5999, "99:59")]
        public void Write_ShowsMinutesAndSeconds(int wholeSeconds, string expected)
        {
            var text = new char[CountdownText.Length];

            CountdownText.Write(wholeSeconds, text);

            Assert.AreEqual(expected, new string(text));
        }
    }
}
