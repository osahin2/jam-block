using System.Collections.Generic;
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

            var problem = AssertSingle(LevelValidator.Validate(level), LevelProblemKind.InvalidTimeLimit, Severity.Error, LevelProblem.NoIndex);
            Assert.AreEqual(LevelPart.Level, problem.Part);
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
        public void Validate_BlockInTwoPieces_IsAnError()
        {
            var level = new LevelBuilder(3, 3)
                .Block(Red, 0, 0, new Cell(0, 0), new Cell(2, 0))
                .Door(Red, Side.Top, 0, 3)
                .Build();

            var problem = AssertSingle(LevelValidator.Validate(level), LevelProblemKind.DisconnectedBlock, Severity.Error, 0);
            Assert.AreEqual(LevelPart.Block, problem.Part);
            Assert.IsNull(problem.Cell);
        }

        [Test]
        public void Validate_BlockCellsTouchingOnlyAtACorner_IsAnError()
        {
            var level = new LevelBuilder(3, 3)
                .Block(Red, 0, 0, new Cell(0, 0), new Cell(1, 1))
                .Door(Red, Side.Top, 0, 2)
                .Build();

            AssertSingle(LevelValidator.Validate(level), LevelProblemKind.DisconnectedBlock, Severity.Error, 0);
        }

        [Test]
        public void Validate_LShapedBlock_IsConnected()
        {
            var level = new LevelBuilder(3, 3)
                .Block(Red, 0, 0, new Cell(0, 0), new Cell(0, 1), new Cell(1, 1))
                .Door(Red, Side.Bottom, 0, 2)
                .Build();

            Assert.IsEmpty(LevelValidator.Validate(level).Problems);
        }

        [Test]
        public void Validate_DoorsOverlappingOnOneSide_IsAnErrorOnTheLaterDoor()
        {
            var level = new LevelBuilder(4, 3)
                .Rect(Red, 0, 1, 1, 1)
                .Door(Red, Side.Top, 0, 2)
                .Door(Blue, Side.Top, 1, 2)
                .Build();

            var problem = AssertSingle(LevelValidator.Validate(level), LevelProblemKind.DoorsOverlap, Severity.Error, 1);
            Assert.AreEqual(LevelPart.Door, problem.Part);
        }

        [Test]
        public void Validate_DoorsSideBySideOrOnDifferentSides_DoNotOverlap()
        {
            var level = new LevelBuilder(4, 3)
                .Rect(Red, 0, 1, 1, 1)
                .Door(Red, Side.Top, 0, 2)
                .Door(Red, Side.Top, 2, 2)
                .Door(Blue, Side.Left, 0, 2)
                .Build();

            Assert.IsEmpty(LevelValidator.Validate(level).Problems);
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
            var level = new LevelBuilder(3, 3).Rect(Red, 1, 1, 2, 1).Door(Red, Side.Left, 1).Build();

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

            var kinds = new LevelProblemKind[report.Problems.Count];
            for (var i = 0; i < kinds.Length; i++)
                kinds[i] = report.Problems[i].Kind;
            CollectionAssert.AreEquivalent(
                new[] { LevelProblemKind.InvalidTimeLimit, LevelProblemKind.BlockOutsideBoard, LevelProblemKind.BlockCannotLeave },
                kinds);
            Assert.IsTrue(report.HasErrors);
        }

        [Test]
        public void Validate_ArrowBlockWhoseDoorsAreAllOffItsAxis_IsAWarning()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 1, 0, 1, 1).Arrow(ArrowAxis.Horizontal).Door(Red, Side.Top, 1).Build();

            AssertSingle(LevelValidator.Validate(level), LevelProblemKind.BlockCannotLeave, Severity.Warning, 0);
        }

        [Test]
        public void Validate_ArrowBlockWithADoorOnItsAxis_HasNoProblems()
        {
            var level = new LevelBuilder(3, 3)
                .Rect(Red, 1, 0, 1, 1).Arrow(ArrowAxis.Vertical)
                .Door(Red, Side.Left, 0)
                .Door(Red, Side.Bottom, 1)
                .Build();

            Assert.IsEmpty(LevelValidator.Validate(level).Problems);
        }

        [Test]
        public void Validate_ArrowAlongTheBlocksOwnCells_HasNoProblems()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 3, 1).Arrow(ArrowAxis.Horizontal, 1, 0, 2).Door(Red, Side.Left, 0).Build();

            Assert.IsEmpty(LevelValidator.Validate(level).Problems);
        }

        [Test]
        public void Validate_ArrowRunningPastTheBlock_IsAnError()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 2, 1).Arrow(ArrowAxis.Horizontal, 1, 0, 2).Door(Red, Side.Left, 0).Build();

            AssertSingle(LevelValidator.Validate(level), LevelProblemKind.ArrowOffBlock, Severity.Error, 0);
        }

        [Test]
        public void Validate_ArrowWithoutCells_IsAnError()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Arrow(ArrowAxis.Vertical, 0, 0, 0).Door(Red, Side.Top, 0).Build();

            AssertSingle(LevelValidator.Validate(level), LevelProblemKind.ArrowOffBlock, Severity.Error, 0);
        }

        [Test]
        public void Validate_IceThatMeltsAsBlocksLeave_HasNoProblems()
        {
            var level = new LevelBuilder(3, 1)
                .Rect(Red, 0, 0, 1, 1)
                .Rect(Red, 1, 0, 1, 1).Ice(1)
                .Rect(Red, 2, 0, 1, 1).Ice(2)
                .Door(Red, Side.Top, 0, 3)
                .Build();

            Assert.IsEmpty(LevelValidator.Validate(level).Problems);
        }

        [Test]
        public void Validate_IceOnTheOnlyBlock_IsAnErrorOnIt()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 1, 1, 1).Ice(1).Door(Red, Side.Top, 0).Build();

            var problem = AssertSingle(LevelValidator.Validate(level), LevelProblemKind.IceNeverMelts, Severity.Error, 0);
            Assert.AreEqual(LevelPart.Block, problem.Part);
        }

        [Test]
        public void Validate_IceMoreThanTheBlocksThatCanLeaveFirst_IsAnErrorOnEveryBlockItLocks()
        {
            var level = new LevelBuilder(4, 1)
                .Rect(Red, 0, 0, 1, 1).Ice(2)
                .Rect(Red, 1, 0, 1, 1)
                .Rect(Red, 2, 0, 1, 1).Ice(2)
                .Rect(Red, 3, 0, 1, 1).Ice(3)
                .Door(Red, Side.Top, 0, 4)
                .Build();

            var report = LevelValidator.Validate(level);

            CollectionAssert.AreEqual(new[] { 0, 2, 3 }, IndicesOf(report, LevelProblemKind.IceNeverMelts));
            Assert.IsTrue(report.HasErrors);
        }

        [Test]
        public void Validate_TwoBlocksIcedOnlyByEachOther_AreBothErrors()
        {
            var level = new LevelBuilder(2, 1)
                .Rect(Red, 0, 0, 1, 1).Ice(1)
                .Rect(Red, 1, 0, 1, 1).Ice(1)
                .Door(Red, Side.Top, 0, 2)
                .Build();

            CollectionAssert.AreEqual(new[] { 0, 1 }, IndicesOf(LevelValidator.Validate(level), LevelProblemKind.IceNeverMelts));
        }

        [TestCase(0, 0)]
        [TestCase(-1, 1)]
        public void Validate_DoorWithoutCellsOrBeforeItsSide_IsAnError(int start, int length)
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Door(Red, Side.Top, 1).Door(Blue, Side.Bottom, start, length).Build();

            AssertSingle(LevelValidator.Validate(level), LevelProblemKind.DoorOutsideSide, Severity.Error, 1);
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

        private static List<int> IndicesOf(LevelReport report, LevelProblemKind kind)
        {
            var indices = new List<int>();
            foreach (var problem in report.Problems)
            {
                if (problem.Kind == kind)
                    indices.Add(problem.Index);
            }
            return indices;
        }
    }
}
