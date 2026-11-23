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
        private EditorPlayRequest _request;
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
            _request = new EditorPlayRequest();
            _request.TryTake(out _);
            _runs = new LevelRuns(_catalog, _progression, _request);
        }

        [TearDown]
        public void DestroyAssets()
        {
            _request.TryTake(out _);
            foreach (var level in _levels)
                Object.DestroyImmediate(level);
            Object.DestroyImmediate(_catalog);
        }

        [Test]
        public void TakeRun_WithoutARequest_PlaysThePlayersLevel()
        {
            _progression.Index = 1;

            var run = _runs.TakeRun();

            Assert.IsFalse(_runs.HasTestRun);
            Assert.AreSame(_levels[1], run.Level);
        }

        [Test]
        public void TakeRun_WithARequest_PlaysTheRequestedLevelOnce()
        {
            var requested = AnyLevelAsset();
            _request.Request(requested);

            Assert.IsTrue(_runs.HasTestRun);
            Assert.AreSame(requested, _runs.TakeRun().Level);
            Assert.IsFalse(_runs.HasTestRun);
        }

        [Test]
        public void PlayerRun_PlaysTheCatalogLevelAtTheProgressionsIndex()
        {
            _progression.Index = 1;

            var run = new PlayerRun(_catalog, _progression);

            Assert.AreSame(_levels[1], run.Level);
            Assert.AreEqual(2, run.Number);
        }

        [Test]
        public void PlayerRun_Advance_CompletesTheLevelAndPlaysTheNextOne()
        {
            var run = new PlayerRun(_catalog, _progression);

            run.Advance();

            Assert.AreEqual(1, _progression.Completed);
            Assert.AreSame(_levels[1], run.Level);
            Assert.AreEqual(2, run.Number);
        }

        [Test]
        public void TestRun_PlaysTheTestedLevelWithItsPlaceInTheCatalog()
        {
            var run = new TestRun(_catalog, _progression, _levels[2]);

            Assert.AreSame(_levels[2], run.Level);
            Assert.AreEqual(3, run.Number);
        }

        [Test]
        public void TestRun_Advance_FromTheCurrentLevel_PlaysTheCatalogsNextLevelWithoutProgress()
        {
            var run = new TestRun(_catalog, _progression, _levels[0]);

            run.Advance();

            Assert.AreEqual(0, _progression.Completed);
            Assert.AreSame(_levels[1], run.Level);
            Assert.AreEqual(2, run.Number);
        }

        [Test]
        public void TestRun_Advance_FromAnotherCatalogLevel_PlaysTheCatalogsNextLevelWithoutProgress()
        {
            var run = new TestRun(_catalog, _progression, _levels[1]);

            run.Advance();

            Assert.AreEqual(0, _progression.Completed);
            Assert.AreSame(_levels[2], run.Level);
        }

        [Test]
        public void TestRun_Advance_FromTheCatalogsLastLevel_PlaysTheCatalogsFirstLevel()
        {
            var run = new TestRun(_catalog, _progression, _levels[2]);

            run.Advance();

            Assert.AreSame(_levels[0], run.Level);
            Assert.AreEqual(1, run.Number);
        }

        [Test]
        public void TestRun_ALevelOutsideTheCatalog_ShowsThePlayersNumberAndAdvancesToTheirLevelWithoutProgress()
        {
            _progression.Index = 2;
            var draft = ScriptableObject.CreateInstance<LevelAsset>();
            var run = new TestRun(_catalog, _progression, draft);

            var number = run.Number;
            run.Advance();

            Assert.AreEqual(3, number);
            Assert.AreEqual(0, _progression.Completed);
            Assert.AreSame(_levels[2], run.Level);
            Object.DestroyImmediate(draft);
        }

        private static LevelAsset AnyLevelAsset()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(LevelAsset)}");
            Assert.IsNotEmpty(guids, "No level assets found.");
            return AssetDatabase.LoadAssetAtPath<LevelAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
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
