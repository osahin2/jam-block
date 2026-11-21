using System.Collections.Generic;
using Core.Save;
using NUnit.Framework;
using UnityEngine;

namespace Core.Levels.Tests
{
    public sealed class LevelProgressionTests
    {
        private sealed class MemorySave : ISaveService
        {
            public readonly Dictionary<string, string> Files = new Dictionary<string, string>();

            public T Load<T>(string key) where T : new() =>
                Files.TryGetValue(key, out string json) ? JsonUtility.FromJson<T>(json) : new T();

            public void Save<T>(string key, T document) => Files[key] = JsonUtility.ToJson(document);
        }

        [Test]
        public void FreshProgress_StartsAtFirstLevel()
        {
            var progression = Create(new MemorySave(), 5);

            Assert.That(progression.CurrentLevelNumber, Is.EqualTo(1));
            Assert.That(progression.CurrentLevelIndex, Is.EqualTo(0));
        }

        [Test]
        public void CompletingALevel_AdvancesNumberAndIndex()
        {
            var progression = Create(new MemorySave(), 5);

            Complete(progression, 2);

            Assert.That(progression.CurrentLevelNumber, Is.EqualTo(3));
            Assert.That(progression.CurrentLevelIndex, Is.EqualTo(2));
        }

        [Test]
        public void Progress_IsSavedAndRestored()
        {
            var save = new MemorySave();
            Complete(Create(save, 5), 3);

            var restored = Create(save, 5);

            Assert.That(restored.CurrentLevelNumber, Is.EqualTo(4));
            Assert.That(restored.CurrentLevelIndex, Is.EqualTo(3));
        }

        [TestCase(5, 0, 5, 0)]
        [TestCase(5, 0, 7, 2)]
        [TestCase(5, 2, 5, 2)]
        [TestCase(5, 2, 7, 4)]
        [TestCase(5, 2, 8, 2)]
        public void AfterTheLastLevel_IndexLoopsFromTheLoopStart(int levelCount, int loopStart, int completed, int expectedIndex)
        {
            var progression = Create(new MemorySave(), levelCount, loopStart);

            Complete(progression, completed);

            Assert.That(progression.CurrentLevelIndex, Is.EqualTo(expectedIndex));
            Assert.That(progression.CurrentLevelNumber, Is.EqualTo(completed + 1));
        }

        private static LevelProgression Create(MemorySave save, int levelCount, int loopStart = 0) =>
            new LevelProgression(save, new LevelLoop(levelCount, loopStart));

        private static void Complete(ILevelProgression progression, int times)
        {
            for (int i = 0; i < times; i++) progression.CompleteCurrentLevel();
        }
    }
}
