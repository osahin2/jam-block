using Core.Levels;
using Game.Levels;
using Game.Levels.Settings;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Levels
{
    public sealed class LevelSequenceTests
    {
        private LevelAsset[] _levels;
        private LevelCatalog _catalog;
        private FakeProgression _progression;
        private LevelSequence _sequence;

        [SetUp]
        public void CreateSequence()
        {
            _levels = new LevelAsset[3];
            for (var i = 0; i < _levels.Length; i++)
                _levels[i] = ScriptableObject.CreateInstance<LevelAsset>();
            _catalog = ScriptableObject.CreateInstance<LevelCatalog>();
            var levels = new SerializedObject(_catalog).FindProperty("_levels");
            levels.arraySize = _levels.Length;
            for (var i = 0; i < _levels.Length; i++)
                levels.GetArrayElementAtIndex(i).objectReferenceValue = _levels[i];
            levels.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            _progression = new FakeProgression();
            _sequence = new LevelSequence(_catalog, _progression);
        }

        [TearDown]
        public void DestroyAssets()
        {
            foreach (var level in _levels)
                Object.DestroyImmediate(level);
            Object.DestroyImmediate(_catalog);
        }

        [Test]
        public void Current_IsTheCatalogLevelAtTheProgressionsIndex()
        {
            _progression.Index = 1;

            Assert.AreSame(_levels[1], _sequence.Current);
        }

        [Test]
        public void CompleteCurrent_CompletesTheCurrentLevelAndGivesTheNextOne()
        {
            var next = _sequence.CompleteCurrent();

            Assert.AreEqual(1, _progression.Completed);
            Assert.AreSame(_levels[1], next);
        }

        // A designer testing the player's current level from the level editor does not move the player's progress.
        [Test]
        public void After_TheCurrentLevel_GivesTheCatalogsNextLevelWithoutProgress()
        {
            var next = _sequence.After(_levels[0]);

            Assert.AreEqual(0, _progression.Completed);
            Assert.AreSame(_levels[1], next);
        }

        [Test]
        public void After_AnotherCatalogLevel_GivesTheCatalogsNextLevelWithoutProgress()
        {
            var next = _sequence.After(_levels[1]);

            Assert.AreEqual(0, _progression.Completed);
            Assert.AreSame(_levels[2], next);
        }

        [Test]
        public void After_TheCatalogsLastLevel_GivesTheCatalogsFirstLevel()
        {
            Assert.AreSame(_levels[0], _sequence.After(_levels[2]));
        }

        [Test]
        public void After_ALevelOutsideTheCatalog_GivesTheCurrentLevelWithoutProgress()
        {
            _progression.Index = 2;
            var draft = ScriptableObject.CreateInstance<LevelAsset>();

            var next = _sequence.After(draft);

            Assert.AreEqual(0, _progression.Completed);
            Assert.AreSame(_levels[2], next);
            Object.DestroyImmediate(draft);
        }

        // Moves one index per completed level; the loop after the last level is Core's and tested there.
        private sealed class FakeProgression : ILevelProgression
        {
            public int Index { get; set; }
            public int Completed { get; private set; }
            public int CurrentLevelNumber => Index + 1;
            public int CurrentLevelIndex => Index;

            public void CompleteCurrentLevel()
            {
                Completed++;
                Index++;
            }
        }
    }
}
