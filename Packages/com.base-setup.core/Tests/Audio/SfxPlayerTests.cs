using System.Linq;
using Core.Pooling;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Core.Audio.Tests
{
    public sealed class SfxPlayerTests
    {
        private AudioTestObjects _objects;
        private Transform _root;
        private SfxPlayer _player;

        [SetUp]
        public void SetUp()
        {
            _objects = new AudioTestObjects();
            _root = _objects.Root("Sfx Root");
            AudioSource template = _objects.Source("Template");
            template.gameObject.SetActive(false);
            _player = new SfxPlayer(new PrefabPool<AudioSource>(template, _root, 0, 4), _root);
        }

        [TearDown]
        public void TearDown() => _objects.DestroyAll();

        [Test]
        public void Play_TakesAnActiveSourceWithOneOfTheClipsAndAPitchInRange()
        {
            AudioClip first = _objects.Clip(0.1f);
            AudioClip second = _objects.Clip(0.1f);

            _player.Play(_objects.Cue(0.5f, 0.8f, 1.2f, first, second));

            AudioSource source = ActiveSources().Single();
            Assert.That(source.clip, Is.EqualTo(first).Or.EqualTo(second));
            Assert.That(source.pitch, Is.InRange(0.8f, 1.2f));
            Assert.That(source.volume, Is.EqualTo(0.5f).Within(1e-5f));
            Assert.That(_player.PlayingCount, Is.EqualTo(1));
        }

        [Test]
        public void WhenTheClipHasPlayedOut_TheSourceGoesBackToThePool()
        {
            SfxCue cue = _objects.Cue(1f, 1f, 1f, _objects.Clip(0.1f));
            _player.Play(cue);
            AudioSource source = ActiveSources().Single();

            _player.Tick(0.05f);
            Assert.That(source.gameObject.activeSelf, Is.True);

            _player.Tick(0.06f);
            Assert.That(source.gameObject.activeSelf, Is.False);
            Assert.That(source.clip, Is.Null);
            Assert.That(_player.PlayingCount, Is.EqualTo(0));

            _player.Play(cue);
            Assert.That(ActiveSources().Single(), Is.SameAs(source));
        }

        [Test]
        public void AHigherPitch_EndsTheSoundSooner()
        {
            _player.Play(_objects.Cue(1f, 2f, 2f, _objects.Clip(0.1f)));

            _player.Tick(0.06f);

            Assert.That(_player.PlayingCount, Is.EqualTo(0));
        }

        [Test]
        public void SetVolume_ScalesSoundsAlreadyPlayingAndNewOnes()
        {
            SfxCue cue = _objects.Cue(0.5f, 1f, 1f, _objects.Clip(1f));
            _player.Play(cue);

            _player.SetVolume(0.5f);
            _player.Play(cue);

            foreach (AudioSource source in ActiveSources())
                Assert.That(source.volume, Is.EqualTo(0.25f).Within(1e-5f));
        }

        [Test]
        public void AtZeroVolume_NothingPlays()
        {
            _player.SetVolume(0f);

            _player.Play(_objects.Cue(1f, 1f, 1f, _objects.Clip(0.1f)));

            Assert.That(ActiveSources(), Is.Empty);
            Assert.That(_player.PlayingCount, Is.EqualTo(0));
        }

        [Test]
        public void ACueWithoutClips_WarnsAndPlaysNothing()
        {
            LogAssert.Expect(LogType.Warning, "[Core.Audio] Sfx cue 'Cue' has no clips.");

            _player.Play(_objects.Cue(1f, 1f, 1f));

            Assert.That(ActiveSources(), Is.Empty);
        }

        private AudioSource[] ActiveSources() => _root.GetComponentsInChildren<AudioSource>(false);
    }
}
