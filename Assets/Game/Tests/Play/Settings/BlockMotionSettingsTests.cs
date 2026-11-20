using Game.Play;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Play
{
    public sealed class BlockMotionSettingsTests
    {
        private BlockMotionSettings _motion;

        [SetUp]
        public void SetUp() => _motion = ScriptableObject.CreateInstance<BlockMotionSettings>();

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_motion);

        [Test]
        public void SlideDuration_GrowsWithTheRootOfTheLength_AtAConstantAcceleration()
        {
            Assert.Greater(_motion.SlideDuration(3), _motion.SlideDuration(2));
            Assert.AreEqual(2f, _motion.SlideDuration(4) / _motion.SlideDuration(1), 1e-5f);
        }
    }
}
