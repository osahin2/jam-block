using System;
using Game.Data.Levels;
using Game.Logic.Levels;
using NUnit.Framework;

namespace Game.Tests.Levels
{
    public sealed class LevelSolverEquivalenceTests
    {
        private const int Seed = 7;
        private const int LevelCount = 150;
        private const int Budget = 3000;
        private const int Colors = 2;

        [Test]
        public void Solve_RandomSmallLevels_AgreesWithASearchWithoutShortcuts()
        {
            var random = new Random(Seed);
            var compared = 0;
            var decided = 0;
            while (compared < LevelCount)
            {
                var level = RandomLevel(random);
                if (LevelValidator.Validate(level).HasErrors)
                    continue;
                compared++;
                var expected = ReferenceSolver.Solve(level, Budget);
                var solution = LevelSolver.Solve(level, Budget);
                if (expected.Verdict == Solvability.Unknown || solution.Verdict == Solvability.Unknown)
                    continue;
                decided++;
                Assert.AreEqual(expected.Verdict, solution.Verdict, $"level {compared}");
                if (expected.Verdict == Solvability.Solvable)
                    Assert.AreEqual(expected.Rearrangements, solution.Rearrangements, $"level {compared}");
            }
            TestContext.WriteLine($"{decided} of {compared} levels decided by both");
            Assert.Greater(decided, LevelCount / 2);
        }

        private static LevelData RandomLevel(Random random)
        {
            var width = random.Next(3, 6);
            var height = random.Next(3, 6);
            var builder = new LevelBuilder(width, height);
            var blocks = random.Next(2, 5);
            for (var i = 0; i < blocks; i++)
            {
                var wide = random.Next(3) == 0;
                var tall = !wide && random.Next(3) == 0;
                builder.Rect(1 + random.Next(Colors), random.Next(width - 1), random.Next(height - 1), wide ? 2 : 1, tall ? 2 : 1);
                if (random.Next(5) == 0)
                    builder.Ice(1);
                if (random.Next(5) == 0)
                    builder.Arrow(random.Next(2) == 0 ? ArrowAxis.Horizontal : ArrowAxis.Vertical);
            }
            for (var color = 1; color <= Colors; color++)
            {
                var side = (Side)random.Next(4);
                var across = side == Side.Top || side == Side.Bottom ? width : height;
                builder.Door(color, side, random.Next(across - 1), random.Next(1, 3));
            }
            return builder.Build();
        }
    }
}
