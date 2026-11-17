using Game.Data.Levels;
using Game.Logic.Levels;
using NUnit.Framework;

namespace Game.Tests.Levels
{
    public sealed class LevelValidatorTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void Validate_PlayableLevel_HasNoProblems()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 1, 2, 1).Door(Red, Side.Top, 0, 2).Build();

            var report = LevelValidator.Validate(level);

            Assert.IsEmpty(report.Problems);
            Assert.IsFalse(report.HasErrors);
        }

        [Test]
        public void Validate_BoardWithoutCells_IsAnError()
        {
            var report = LevelValidator.Validate(new LevelBuilder(0, 3).Build());

            AssertSingle(report, LevelProblemKind.InvalidSize, Severity.Error, LevelProblem.NoIndex);
        }

        [Test]
        public void Validate_TimeLimitBelowOneSecond_IsAnError()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 1, 1, 1).Door(Red, Side.Top, 0).Build();
            level.TimeLimitSeconds = 0;

            AssertSingle(LevelValidator.Validate(level), LevelProblemKind.InvalidTimeLimit, Severity.Error, LevelProblem.NoIndex);
        }

        [Test]
        public void Validate_LevelWithoutBlocks_IsAnError()
        {
            var level = new LevelBuilder(3, 3).Door(Red, Side.Top, 0).Build();

            AssertSingle(LevelValidator.Validate(level), LevelProblemKind.NoBlocks, Severity.Error, LevelProblem.NoIndex);
        }

        [Test]
        public void Validate_BlockWithoutCells_IsAnError()
        {
            var level = new LevelBuilder(3, 3).Block(Red, 0, 0).Build();

            AssertSingle(LevelValidator.Validate(level), LevelProblemKind.EmptyBlock, Severity.Error, 0);
        }

        [Test]
        public void Validate_BlockCellOutsideTheBoard_IsAnErrorAtThatCell()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 2, 0, 2, 1).Door(Red, Side.Top, 0, 2).Build();

            var problem = AssertSingle(LevelValidator.Validate(level), LevelProblemKind.BlockOutsideBoard, Severity.Error, 0);
            Assert.AreEqual(new Cell(3, 0), problem.Cell);
        }

        [Test]
        public void Validate_OverlappingBlocks_IsAnErrorAtTheSharedCell()
        {
            var level = new LevelBuilder(3, 3)
                .Rect(Red, 0, 0, 2, 1)
                .Rect(Red, 1, 0, 1, 1)
                .Door(Red, Side.Top, 0, 2)
                .Build();

            var problem = AssertSingle(LevelValidator.Validate(level), LevelProblemKind.BlocksOverlap, Severity.Error, 1);
            Assert.AreEqual(new Cell(1, 0), problem.Cell);
        }

        [Test]
        public void Validate_DoorPastTheEndOfItsSide_IsAnError()
        {
            var level = new LevelBuilder(3, 4).Rect(Red, 0, 0, 1, 1).Door(Red, Side.Top, 0).Door(Red, Side.Top, 2, 2).Build();

            AssertSingle(LevelValidator.Validate(level), LevelProblemKind.DoorOutsideSide, Severity.Error, 1);
        }

        [Test]
        public void Validate_DoorAlongTheLongerSide_UsesThatSidesLength()
        {
            var level = new LevelBuilder(3, 4).Rect(Red, 0, 0, 1, 1).Door(Red, Side.Left, 2, 2).Build();

            Assert.IsEmpty(LevelValidator.Validate(level).Problems);
        }

        [Test]
        public void Validate_BlockWiderThanEveryDoorOfItsColor_IsAWarning()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 1, 2, 1).Door(Red, Side.Top, 0).Build();

            var report = LevelValidator.Validate(level);

            AssertSingle(report, LevelProblemKind.BlockCannotLeave, Severity.Warning, 0);
            Assert.IsFalse(report.HasErrors);
        }

        [Test]
        public void Validate_BlockFitsADoorOnTheSideAcrossItsShortExtent()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 1, 2, 1).Door(Red, Side.Left, 1).Build();

            Assert.IsEmpty(LevelValidator.Validate(level).Problems);
        }

        [Test]
        public void Validate_BlockWithDoorsOfOtherColorsOnly_IsAWarning()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 1, 1, 1).Door(Blue, Side.Top, 0).Build();

            AssertSingle(LevelValidator.Validate(level), LevelProblemKind.BlockCannotLeave, Severity.Warning, 0);
        }

        [Test]
        public void Validate_ReportsEveryProblem()
        {
            var level = new LevelBuilder(3, 3)
                .Rect(Red, 2, 0, 2, 1)
                .Rect(Blue, 0, 2, 1, 1)
                .Door(Red, Side.Top, 0, 2)
                .Build();
            level.TimeLimitSeconds = 0;

            var report = LevelValidator.Validate(level);

            Assert.AreEqual(3, report.Problems.Count);
            Assert.IsTrue(report.HasErrors);
        }

        private static LevelProblem AssertSingle(LevelReport report, LevelProblemKind kind, Severity severity, int index)
        {
            Assert.AreEqual(1, report.Problems.Count, report.ToString());
            var problem = report.Problems[0];
            Assert.AreEqual(kind, problem.Kind);
            Assert.AreEqual(severity, problem.Severity);
            Assert.AreEqual(index, problem.Index);
            return problem;
        }
    }
}
