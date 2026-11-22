using System;
using NUnit.Framework;

namespace Core.Haptics.Tests
{
    public sealed class AndroidHapticBackendTests
    {
        [Test]
        public void TheEffectTable_HasOneEffectPerPreset()
        {
            Assert.That(AndroidHapticBackend.Effects.Length, Is.EqualTo(Enum.GetValues(typeof(HapticPreset)).Length));
        }
    }
}
