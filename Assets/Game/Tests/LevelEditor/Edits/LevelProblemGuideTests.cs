using System;
using Game.Data.Levels;
using Game.Editor.LevelEditor.Editing;
using Game.Logic.Levels;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.LevelEditor
{
    public sealed class LevelProblemGuideTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void EveryProblemKind_HasAnExplanation()
        {
            foreach (LevelProblemKind kind in Enum.GetValues(typeof(LevelProblemKind)))
                Assert.AreNotEqual(kind.ToString(), LevelProblemGuide.Explain(kind), kind.ToString());
        }

        [Test]
        public void SubjectOf_IsTheBlockOrDoorTheProblemIsAbout()
        {
            var level = new LevelBuilder(4, 3)
                .Block(Red, 0, 0, new Cell(0, 0), new Cell(2, 0))
                .Door(Red, Side.Top, 0, 2)
                .Door(Blue, Side.Top, 1, 2)
                .Build();
            var report = LevelValidator.Validate(level);

            var block = LevelProblemGuide.SubjectOf(Find(report, LevelProblemKind.DisconnectedBlock), level);
            var door = LevelProblemGuide.SubjectOf(Find(report, LevelProblemKind.DoorsOverlap), level);

            CollectionAssert.AreEquivalent(new[] { new Cell(0, 0), new Cell(2, 0) }, block.Cells(level));
            Assert.AreEqual(Blue, door.ColorId(level));
        }

        [Test]
        public void SubjectOf_AProblemWithTheWholeLevel_IsNothing()
        {
            var level = new LevelBuilder(3, 3).Build();
            var report = LevelValidator.Validate(level);

            Assert.IsNull(LevelProblemGuide.SubjectOf(Find(report, LevelProblemKind.NoBlocks), level));
        }

        private static LevelProblem Find(LevelReport report, LevelProblemKind kind)
        {
            foreach (var problem in report.Problems)
            {
                if (problem.Kind == kind)
                    return problem;
            }
            Assert.Fail($"No {kind} in {report}");
            return default;
        }
    }
}
