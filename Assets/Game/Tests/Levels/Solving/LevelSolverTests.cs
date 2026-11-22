using Game.Data.Levels;
using Game.Logic.Levels;
using Game.Logic.Play;
using Game.Tests.Play;
using NUnit.Framework;

namespace Game.Tests.Levels
{
    public sealed class LevelSolverTests
    {
        private const int Red = 1;
        private const int Blue = 2;
        private const int Green = 3;
        private const int Budget = 10000;

        [Test]
        public void Solve_ABlockThatReachesItsDoor_LeavesInOneMove()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Door(Red, Side.Top, 2).Build();

            var solution = LevelSolver.Solve(level, Budget);

            Assert.AreEqual(Solvability.Solvable, solution.Verdict);
            Assert.AreEqual(1, solution.Moves.Count);
            Assert.AreEqual(0, solution.Rearrangements);
            Assert.IsTrue(solution.Moves[0].Leaves);
        }

        [Test]
        public void Solve_ABlockInTheWayThatCanLeave_LeavesItFirstWithNoRearrangement()
        {
            var level = new LevelBuilder(3, 1)
                .Rect(Red, 1, 0, 1, 1)
                .Rect(Blue, 0, 0, 1, 1)
                .Door(Red, Side.Left, 0)
                .Door(Blue, Side.Bottom, 0)
                .Build();

            var solution = LevelSolver.Solve(level, Budget);

            Assert.AreEqual(Solvability.Solvable, solution.Verdict);
            Assert.AreEqual(0, solution.Rearrangements);
            Assert.AreEqual(1, solution.Moves[0].Block);
            Assert.AreEqual(0, solution.Moves[1].Block);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Solve_NoBlockCanLeaveAtTheStart_FindsTheFewestRearrangements(int blockers)
        {
            var level = Column(blockers);

            var solution = LevelSolver.Solve(level, Budget);

            Assert.AreEqual(Solvability.Solvable, solution.Verdict);
            Assert.IsFalse(solution.Moves[0].Leaves);
            Assert.AreEqual(blockers, solution.Rearrangements);
            Assert.AreEqual(level.Blocks.Count + blockers, solution.Moves.Count);
        }

        [Test]
        public void Solve_TheMovesReplayedOnTheBoard_ClearIt()
        {
            var level = Column(3);

            var solution = LevelSolver.Solve(level, Budget);

            var board = Boards.Of(level);
            var reach = new BlockReach();
            foreach (var move in solution.Moves)
            {
                reach.Find(board, move.Block);
                if (move.Leaves)
                {
                    Assert.IsTrue(reach.CanLeave, $"Block {move.Block} cannot reach a door.");
                    board.Place(move.Block, move.Position);
                    Assert.IsTrue(board.TryLeave(move.Block, out _), $"Block {move.Block} did not leave.");
                    continue;
                }
                Assert.IsFalse(reach.CanLeave, $"Block {move.Block} could leave instead.");
                CollectionAssert.Contains(reach.Places, move.Position);
                board.Place(move.Block, move.Position);
            }

            Assert.IsTrue(board.IsCleared);
        }

        [Test]
        public void Solve_TwoBlocksThatCannotPassEachOther_IsUnsolvableAfterEveryArrangement()
        {
            var level = new LevelBuilder(3, 1)
                .Rect(Blue, 0, 0, 1, 1)
                .Rect(Red, 2, 0, 1, 1)
                .Door(Blue, Side.Right, 0)
                .Door(Red, Side.Left, 0)
                .Build();

            var solution = LevelSolver.Solve(level, Budget);

            Assert.AreEqual(Solvability.Unsolvable, solution.Verdict);
            Assert.IsEmpty(solution.Moves);
            Assert.AreEqual(3, solution.ExploredStates);
        }

        [Test]
        public void Solve_AMoveThatOpensTheWayOfADistantBlock_LetsThatBlockLeaveAtOnce()
        {
            var level = new LevelBuilder(3, 5)
                .Rect(Red, 0, 4, 1, 1).Arrow(ArrowAxis.Vertical)
                .Rect(Blue, 0, 1, 1, 1).Arrow(ArrowAxis.Horizontal)
                .Rect(Green, 2, 1, 1, 1).Ice(1)
                .Door(Red, Side.Top, 0)
                .Door(Blue, Side.Right, 1)
                .Door(Green, Side.Top, 2)
                .Build();

            var solution = LevelSolver.Solve(level, Budget);

            Assert.AreEqual(Solvability.Solvable, solution.Verdict);
            Assert.AreEqual(1, solution.Rearrangements);
            Assert.AreEqual(1, solution.Moves[0].Block);
            Assert.AreEqual(new Cell(1, 1), solution.Moves[0].Position);
            Assert.IsFalse(solution.Moves[0].Leaves);
            Assert.AreEqual(0, solution.Moves[1].Block);
            Assert.IsTrue(solution.Moves[1].Leaves);
        }

        [Test]
        public void Solve_ABlockThatFitsNoDoor_IsUnsolvableWithoutSearching()
        {
            var level = new LevelBuilder(4, 4)
                .Rect(Red, 0, 0, 1, 1)
                .Rect(Blue, 2, 2, 1, 1)
                .Door(Red, Side.Top, 0)
                .Build();

            var solution = LevelSolver.Solve(level, Budget);

            Assert.AreEqual(Solvability.Unsolvable, solution.Verdict);
            Assert.AreEqual(0, solution.ExploredStates);
        }

        [Test]
        public void Solve_MoreArrangementsThanTheBudget_IsUnknown()
        {
            var solution = LevelSolver.Solve(Column(3), 2);

            Assert.AreEqual(Solvability.Unknown, solution.Verdict);
            Assert.IsEmpty(solution.Moves);
        }

        [Test]
        public void Solve_IceThatMeltsWhenAnotherBlockLeaves_LeavesThatBlockFirst()
        {
            var level = new LevelBuilder(3, 1)
                .Rect(Blue, 1, 0, 1, 1).Ice(1)
                .Rect(Red, 0, 0, 1, 1)
                .Door(Blue, Side.Right, 0)
                .Door(Red, Side.Left, 0)
                .Build();

            var solution = LevelSolver.Solve(level, Budget);

            Assert.AreEqual(Solvability.Solvable, solution.Verdict);
            Assert.AreEqual(1, solution.Moves[0].Block);
            Assert.AreEqual(0, solution.Moves[1].Block);
        }

        [Test]
        public void Solve_IceThatNeverMelts_IsUnsolvable()
        {
            var level = new LevelBuilder(3, 1)
                .Rect(Blue, 1, 0, 1, 1).Ice(2)
                .Rect(Red, 0, 0, 1, 1)
                .Door(Blue, Side.Right, 0)
                .Door(Red, Side.Left, 0)
                .Build();

            var solution = LevelSolver.Solve(level, Budget);

            Assert.AreEqual(Solvability.Unsolvable, solution.Verdict);
        }

        [TestCase(ArrowAxis.None, Solvability.Solvable)]
        [TestCase(ArrowAxis.Horizontal, Solvability.Unsolvable)]
        public void Solve_AnArrowAcrossTheWayToTheDoor_DecidesWhetherItCanLeave(ArrowAxis axis, Solvability verdict)
        {
            var level = new LevelBuilder(2, 2).Rect(Red, 0, 1, 1, 1).Arrow(axis).Door(Red, Side.Right, 0).Build();

            var solution = LevelSolver.Solve(level, Budget);

            Assert.AreEqual(verdict, solution.Verdict);
        }

        private static LevelData Column(int blockers)
        {
            var builder = new LevelBuilder(2, blockers + 1)
                .Rect(Blue, 0, blockers, 1, 1).Arrow(ArrowAxis.Vertical)
                .Rect(Green, 1, blockers, 1, 1).Ice(1)
                .Door(Blue, Side.Top, 0)
                .Door(Red, Side.Bottom, 1)
                .Door(Green, Side.Right, blockers);
            for (var y = 0; y < blockers; y++)
                builder.Rect(Red, 0, y, 1, 1);
            return builder.Build();
        }
    }
}
