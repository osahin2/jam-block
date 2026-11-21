using Core.Levels;
using Game.Levels;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Levels
{
    public sealed class LevelRunsTests
    {
        private LevelAsset[] _levels;
        private LevelCatalog _catalog;
        private FakeProgression _progression;
        private LevelRuns _runs;

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
            _runs = new LevelRuns(_catalog, _progression);
        }

        [TearDown]
        public void DestroyAssets()
        {
            foreach (var level in _levels)
                Object.DestroyImmediate(level);
            Object.DestroyImmediate(_catalog);
        }

        [Test]
        public void RunForPlayer_PlaysTheCatalogLevelAtTheProgressionsIndex()
        {
            _progression.Index = 1;

            var run = _runs.RunForPlayer();

            Assert.AreSame(_levels[1], run.Level);
            Assert.AreEqual(2, run.Number);
        }

        [Test]
        public void RunForPlayer_Advance_CompletesTheLevelAndPlaysTheNextOne()
        {
            var run = _runs.RunForPlayer();

            run.Advance();

            Assert.AreEqual(1, _progression.Completed);
            Assert.AreSame(_levels[1], run.Level);
            Assert.AreEqual(2, run.Number);
        }

        [Test]
        public void RunForTest_PlaysTheTestedLevelWithItsPlaceInTheCatalog()
        {
            var run = _runs.RunForTest(_levels[2]);

            Assert.AreSame(_levels[2], run.Level);
            Assert.AreEqual(3, run.Number);
        }

        [Test]
        public void RunForTest_Advance_FromTheCurrentLevel_PlaysTheCatalogsNextLevelWithoutProgress()
        {
            var run = _runs.RunForTest(_levels[0]);

            run.Advance();

            Assert.AreEqual(0, _progression.Completed);
            Assert.AreSame(_levels[1], run.Level);
            Assert.AreEqual(2, run.Number);
        }

        [Test]
        public void RunForTest_Advance_FromAnotherCatalogLevel_PlaysTheCatalogsNextLevelWithoutProgress()
        {
            var run = _runs.RunForTest(_levels[1]);

            run.Advance();

            Assert.AreEqual(0, _progression.Completed);
            Assert.AreSame(_levels[2], run.Level);
        }

        [Test]
        public void RunForTest_Advance_FromTheCatalogsLastLevel_PlaysTheCatalogsFirstLevel()
        {
            var run = _runs.RunForTest(_levels[2]);

            run.Advance();

            Assert.AreSame(_levels[0], run.Level);
            Assert.AreEqual(1, run.Number);
        }

        [Test]
        public void RunForTest_ALevelOutsideTheCatalog_ShowsThePlayersNumberAndAdvancesToTheirLevelWithoutProgress()
        {
            _progression.Index = 2;
            var draft = ScriptableObject.CreateInstance<LevelAsset>();
            var run = _runs.RunForTest(draft);

            var number = run.Number;
            run.Advance();

            Assert.AreEqual(3, number);
            Assert.AreEqual(0, _progression.Completed);
            Assert.AreSame(_levels[2], run.Level);
            Object.DestroyImmediate(draft);
        }

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
