using Game.Data.Levels;
using Game.Editor.LevelEditor;
using Game.Logic.Levels;
using Game.Tests.Levels;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.LevelEditor
{
    public sealed class SolvabilityCheckTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        private SolverSettings _settings;

        [SetUp]
        public void SetUp() => _settings = ScriptableObject.CreateInstance<SolverSettings>();

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_settings);

        [Test]
        public void Solve_TheSameLevelAgain_GivesTheSameSolution()
        {
            var check = new SolvabilityCheck(_settings);
            var level = OneBlock();

            var first = check.Solve(level);
            var second = check.Solve(OneBlock());

            Assert.AreSame(first, second);
            Assert.AreEqual(Solvability.Solvable, first.Verdict);
        }

        [Test]
        public void Solve_AChangedLevel_SolvesItAgain()
        {
            var check = new SolvabilityCheck(_settings);
            var level = OneBlock();
            var first = check.Solve(level);

            level.Doors.Clear();
            var second = check.Solve(level);

            Assert.AreNotSame(first, second);
            Assert.AreEqual(Solvability.Unsolvable, second.Verdict);
        }

        [Test]
        public void SearchFully_ALevelTheQuickCheckLeftUndecided_DecidesIt()
        {
            var settings = new SerializedObject(_settings);
            settings.FindProperty("_quickCheckMilliseconds").intValue = 0;
            settings.ApplyModifiedPropertiesWithoutUndo();
            var check = new SolvabilityCheck(_settings);
            var level = TwoBlocksThatCannotPassEachOther();

            var quick = check.Solve(level);
            var searchedQuickly = check.SearchedFully;
            check.SearchFully(level);
            var full = check.Solve(level);

            Assert.AreEqual(Solvability.Unknown, quick.Verdict);
            Assert.IsFalse(searchedQuickly);
            Assert.AreEqual(Solvability.Unsolvable, full.Verdict);
            Assert.IsTrue(check.SearchedFully);
        }

        [Test]
        public void TheProject_HasOneSolverSettingsAsset()
        {
            Assert.AreEqual(1, AssetDatabase.FindAssets($"t:{nameof(SolverSettings)}").Length);
        }

        private static LevelData OneBlock() =>
            new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Door(Red, Side.Top, 1).Build();

        private static LevelData TwoBlocksThatCannotPassEachOther() =>
            new LevelBuilder(3, 1)
                .Rect(Blue, 0, 0, 1, 1)
                .Rect(Red, 2, 0, 1, 1)
                .Door(Blue, Side.Right, 0)
                .Door(Red, Side.Left, 0)
                .Build();
    }
}
