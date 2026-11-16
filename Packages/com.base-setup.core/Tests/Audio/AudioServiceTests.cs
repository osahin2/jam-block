using Core.Pooling;
using NUnit.Framework;
using UnityEngine;

namespace Core.Audio.Tests
{
    public sealed class AudioServiceTests
    {
        private AudioTestObjects _objects;
        private MemorySave _save;
        private SfxPlayer _sfx;
        private MusicPlayer _music;
        private AudioSource _musicA;
        private AudioSource _musicB;

        [SetUp]
        public void SetUp()
        {
            _objects = new AudioTestObjects();
            _save = new MemorySave();
        }

        [TearDown]
        public void TearDown() => _objects.DestroyAll();

        [Test]
        public void AFreshService_IsAtFullVolumeAndNotMuted()
        {
            AudioService service = Create();

            Assert.That(service.SfxVolume, Is.EqualTo(1f));
            Assert.That(service.MusicVolume, Is.EqualTo(1f));
            Assert.That(service.SfxMuted, Is.False);
            Assert.That(service.MusicMuted, Is.False);
        }

        [Test]
        public void Changes_AreSavedAndRestored()
        {
            AudioService service = Create();
            service.SfxVolume = 0.3f;
            service.MusicVolume = 0.6f;
            service.SfxMuted = true;
            service.MusicMuted = true;

            AudioService restored = Create();

            Assert.That(restored.SfxVolume, Is.EqualTo(0.3f).Within(1e-5f));
            Assert.That(restored.MusicVolume, Is.EqualTo(0.6f).Within(1e-5f));
            Assert.That(restored.SfxMuted, Is.True);
            Assert.That(restored.MusicMuted, Is.True);
        }

        [Test]
        public void SettingTheSameValue_DoesNotWrite()
        {
            AudioService service = Create();

            service.SfxMuted = false;
            service.MusicVolume = 1f;

            Assert.That(_save.SaveCount, Is.EqualTo(0));
        }

        [Test]
        public void Volume_IsClampedToZeroOne()
        {
            AudioService service = Create();

            service.SfxVolume = 2f;
            service.MusicVolume = -1f;

            Assert.That(service.SfxVolume, Is.EqualTo(1f));
            Assert.That(service.MusicVolume, Is.EqualTo(0f));
        }

        [Test]
        public void WhileSfxIsMuted_CuesPlayNothing()
        {
            AudioService service = Create();
            service.SfxMuted = true;

            service.Play(_objects.Cue(1f, 1f, 1f, _objects.Clip(0.1f)));

            Assert.That(_sfx.PlayingCount, Is.EqualTo(0));
        }

        [Test]
        public void ARestoredMusicMute_SilencesMusicFromTheStart_AndUnmutingRestoresIt()
        {
            Create().MusicMuted = true;
            AudioService service = Create();
            AudioClip clip = _objects.Clip(1f);

            service.PlayMusic(clip);
            _music.Tick(0.1f);
            AudioSource source = MusicSourceOf(clip);
            Assert.That(source.clip, Is.EqualTo(clip));
            Assert.That(source.volume, Is.EqualTo(0f));

            service.MusicMuted = false;
            Assert.That(source.volume, Is.EqualTo(1f));
        }

        [Test]
        public void MusicVolume_ScalesThePlayingTrack()
        {
            AudioService service = Create();
            AudioClip clip = _objects.Clip(1f);
            service.PlayMusic(clip);
            _music.Tick(0.1f);

            service.MusicVolume = 0.25f;

            Assert.That(MusicSourceOf(clip).volume, Is.EqualTo(0.25f).Within(1e-5f));
        }

        // Each call builds fresh players over the same save, as a restart of the app would.
        private AudioService Create()
        {
            Transform sfxRoot = _objects.Root("Sfx Root");
            AudioSource template = _objects.Source("Template");
            template.gameObject.SetActive(false);
            _sfx = new SfxPlayer(new PrefabPool<AudioSource>(template, sfxRoot, 0, 4), sfxRoot);
            _musicA = _objects.Source("Music A");
            _musicB = _objects.Source("Music B");
            _music = new MusicPlayer(_musicA, _musicB, 0f);
            return new AudioService(_save, _sfx, _music);
        }

        private AudioSource MusicSourceOf(AudioClip clip) => _musicA.clip == clip ? _musicA : _musicB;
    }
}
