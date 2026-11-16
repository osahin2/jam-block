using NUnit.Framework;

namespace Core.Haptics.Tests
{
    public sealed class HapticServiceTests
    {
        private MemorySave _save;
        private RecordingBackend _backend;

        [SetUp]
        public void SetUp()
        {
            _save = new MemorySave();
            _backend = new RecordingBackend();
        }

        [Test]
        public void AFreshService_IsEnabled()
        {
            Assert.That(Create().Enabled, Is.True);
        }

        [Test]
        public void WhileEnabled_PresetsReachTheBackend()
        {
            HapticService service = Create();

            service.Play(HapticPreset.Light);
            service.Play(HapticPreset.Failure);

            Assert.That(_backend.Played, Is.EqualTo(new[] { HapticPreset.Light, HapticPreset.Failure }));
        }

        [Test]
        public void WhileDisabled_NothingReachesTheBackend()
        {
            HapticService service = Create();
            service.Enabled = false;

            service.Play(HapticPreset.Heavy);

            Assert.That(_backend.Played, Is.Empty);
        }

        [Test]
        public void Disabling_IsSavedAndRestored()
        {
            Create().Enabled = false;

            HapticService restored = Create();
            restored.Play(HapticPreset.Success);

            Assert.That(restored.Enabled, Is.False);
            Assert.That(_backend.Played, Is.Empty);
        }

        [Test]
        public void SettingTheSameValue_DoesNotWrite()
        {
            Create().Enabled = true;

            Assert.That(_save.SaveCount, Is.EqualTo(0));
        }

        // Each call builds a fresh service over the same save, as a restart of the app would.
        private HapticService Create() => new HapticService(_save, _backend);
    }
}
