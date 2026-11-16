using NUnit.Framework;
using UnityEngine;

namespace Core.Audio.Tests
{
    public sealed class MusicPlayerTests
    {
        private const float Tolerance = 1e-4f;

        private AudioTestObjects _objects;
        private AudioSource _first;
        private AudioSource _second;
        private AudioClip _a;
        private AudioClip _b;

        [SetUp]
        public void SetUp()
        {
            _objects = new AudioTestObjects();
            _first = _objects.Source("Music A");
            _second = _objects.Source("Music B");
            _a = _objects.Clip(2f);
            _b = _objects.Clip(2f);
        }

        [TearDown]
        public void TearDown() => _objects.DestroyAll();

        [Test]
        public void Play_LoopsTheClipAndFadesItIn()
        {
            MusicPlayer player = Create();

            player.Play(_a);
            AudioSource source = SourceOf(_a);
            Assert.That(source, Is.Not.Null);
            Assert.That(source.loop, Is.True);
            Assert.That(source.volume, Is.EqualTo(0f));

            player.Tick(0.5f);
            Assert.That(source.volume, Is.EqualTo(0.5f).Within(Tolerance));

            player.Tick(0.6f);
            Assert.That(source.volume, Is.EqualTo(1f).Within(Tolerance));
        }

        [Test]
        public void PlayingAnotherClip_Crossfades()
        {
            MusicPlayer player = Create();
            player.Play(_a);
            player.Tick(1f);
            AudioSource oldSource = SourceOf(_a);

            player.Play(_b);
            player.Tick(0.5f);
            Assert.That(oldSource.volume, Is.EqualTo(0.5f).Within(Tolerance));
            Assert.That(SourceOf(_b).volume, Is.EqualTo(0.5f).Within(Tolerance));

            player.Tick(0.5f);
            Assert.That(oldSource.clip, Is.Null);
            Assert.That(SourceOf(_b).volume, Is.EqualTo(1f).Within(Tolerance));
        }

        [Test]
        public void PlayingTheSameClip_KeepsItPlaying()
        {
            MusicPlayer player = Create();
            player.Play(_a);
            player.Tick(1f);
            AudioSource source = SourceOf(_a);

            player.Play(_a);

            Assert.That(SourceOf(_a), Is.SameAs(source));
            Assert.That(source.volume, Is.EqualTo(1f).Within(Tolerance));
        }

        [Test]
        public void ReturningToAClipThatIsFadingOut_FadesItBackIn()
        {
            MusicPlayer player = Create();
            player.Play(_a);
            player.Tick(1f);
            AudioSource sourceA = SourceOf(_a);
            player.Play(_b);
            player.Tick(0.25f);

            player.Play(_a);
            player.Tick(0.25f);

            Assert.That(SourceOf(_a), Is.SameAs(sourceA));
            Assert.That(sourceA.volume, Is.EqualTo(1f).Within(Tolerance));
            Assert.That(SourceOf(_b), Is.Null, "B faded out and released its clip");
        }

        [Test]
        public void Stop_FadesOutAndReleasesTheClip()
        {
            MusicPlayer player = Create();
            player.Play(_a);
            player.Tick(1f);
            AudioSource source = SourceOf(_a);

            player.Stop();
            player.Tick(0.5f);
            Assert.That(source.volume, Is.EqualTo(0.5f).Within(Tolerance));

            player.Tick(0.5f);
            Assert.That(source.clip, Is.Null);
        }

        [Test]
        public void SetVolume_ScalesTheFadeLevel()
        {
            MusicPlayer player = Create();
            player.Play(_a);
            player.Tick(1f);

            player.SetVolume(0.4f);

            Assert.That(SourceOf(_a).volume, Is.EqualTo(0.4f).Within(Tolerance));
        }

        [Test]
        public void WithoutAFadeDuration_TheClipReachesFullLevelOnTheNextTick()
        {
            MusicPlayer player = Create(0f);

            player.Play(_a);
            player.Tick(0.016f);

            Assert.That(SourceOf(_a).volume, Is.EqualTo(1f).Within(Tolerance));
        }

        private MusicPlayer Create(float fadeDuration = 1f) => new MusicPlayer(_first, _second, fadeDuration);

        private AudioSource SourceOf(AudioClip clip) => _first.clip == clip ? _first : _second.clip == clip ? _second : null;
    }
}
