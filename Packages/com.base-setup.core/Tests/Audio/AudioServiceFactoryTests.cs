using NUnit.Framework;
using UnityEngine;

namespace Core.Audio.Tests
{
    public sealed class AudioServiceFactoryTests
    {
        private AudioTestObjects _objects;

        [SetUp]
        public void SetUp() => _objects = new AudioTestObjects();

        [TearDown]
        public void TearDown() => _objects.DestroyAll();

        [Test]
        public void Create_BuildsOneRootWithMusicSourcesTheSfxPoolAndTheTicker()
        {
            var settings = ScriptableObject.CreateInstance<AudioServiceSettings>();
            _objects.Track(settings);

            AudioService service = AudioServiceFactory.Create(settings, new MemorySave());
            var root = GameObject.Find("Core.Audio");
            _objects.Track(root);

            Assert.That(service, Is.Not.Null);
            Assert.That(root.GetComponent<AudioTicker>(), Is.Not.Null);
            Assert.That(root.transform.Find("Music A"), Is.Not.Null);
            Assert.That(root.transform.Find("Music B"), Is.Not.Null);
            // Template plus the default prewarm of four, all inactive until a cue plays.
            Assert.That(root.GetComponentsInChildren<AudioSource>(true).Length, Is.EqualTo(2 + 1 + 4));
            Assert.That(root.GetComponentsInChildren<AudioSource>(false).Length, Is.EqualTo(2));
        }
    }
}
