using Game.Data.Levels;
using Game.Editor.LevelEditor;
using Game.Logic.Levels;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.LevelEditor
{
    public sealed class SolutionPlaybackTests
    {
        private const int Red = 1;
        private const int Blue = 2;
        private const int Green = 3;
        private const int Budget = 10000;

        [Test]
        public void New_ShowsTheLevelAsItStartsAndTheFirstMove()
        {
            var level = IceLevel();
            var playback = new SolutionPlayback(level, LevelSolver.Solve(level, Budget));

            Assert.AreEqual(0, playback.Step);
            Assert.IsTrue(playback.IsAtStart);
            Assert.AreEqual(2, playback.Shown.Blocks.Count);
            Assert.AreEqual(1, playback.NextMove.Block);
        }

        [Test]
        public void Next_AfterABlockLeaves_ShowsItGoneAndTheIceMelted()
        {
            var level = IceLevel();
            var playback = new SolutionPlayback(level, LevelSolver.Solve(level, Budget));

            playback.Next();

            Assert.AreEqual(1, playback.Shown.Blocks.Count);
            Assert.AreEqual(Blue, playback.Shown.Blocks[0].ColorId);
            Assert.AreEqual(0, playback.Shown.Blocks[0].Ice);
        }

        [Test]
        public void Next_ARearrangement_ShowsTheBlockAtItsNewPlace()
        {
            var level = StuckLevel();
            var solution = LevelSolver.Solve(level, Budget);
            var playback = new SolutionPlayback(level, solution);
            var move = playback.NextMove;

            playback.Next();

            Assert.IsFalse(move.Leaves);
            Assert.AreEqual(move.Position, playback.Shown.Blocks[move.Block].Position);
            Assert.AreEqual(level.Blocks.Count, playback.Shown.Blocks.Count);
        }

        [Test]
        public void Next_ToTheEnd_ShowsAClearBoardAndStops()
        {
            var level = StuckLevel();
            var playback = new SolutionPlayback(level, LevelSolver.Solve(level, Budget));

            for (var i = 0; i <= playback.MoveCount; i++)
                playback.Next();

            Assert.IsTrue(playback.IsAtEnd);
            Assert.AreEqual(playback.MoveCount, playback.Step);
            Assert.IsEmpty(playback.Shown.Blocks);
        }

        [Test]
        public void Previous_GoesBackOneMoveAndStopsAtTheStart()
        {
            var level = IceLevel();
            var playback = new SolutionPlayback(level, LevelSolver.Solve(level, Budget));
            playback.Next();

            playback.Previous();
            playback.Previous();

            Assert.AreEqual(0, playback.Step);
            Assert.AreEqual(2, playback.Shown.Blocks.Count);
            Assert.AreEqual(1, playback.Shown.Blocks[0].Ice);
        }

        [Test]
        public void NextCells_AreTheMovingBlocksCellsWhereItStandsAndWhereItGoes()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 2, 2, 1).Door(Red, Side.Top, 1, 2).Build();
            var playback = new SolutionPlayback(level, LevelSolver.Solve(level, Budget));

            CollectionAssert.AreEquivalent(new[] { new Cell(0, 2), new Cell(1, 2) }, playback.NextBlockCells());
            CollectionAssert.AreEquivalent(new[] { new Cell(1, 0), new Cell(2, 0) }, playback.NextTargetCells());
        }

        [Test]
        public void New_KeepsItsOwnCopyOfTheLevel()
        {
            var level = IceLevel();
            var playback = new SolutionPlayback(level, LevelSolver.Solve(level, Budget));

            level.Blocks[0].Position = new Cell(2, 0);
            playback.Next();
            playback.Previous();

            Assert.AreEqual(new Cell(1, 0), playback.Shown.Blocks[0].Position);
        }

        private static LevelData IceLevel() =>
            new LevelBuilder(3, 1)
                .Rect(Blue, 1, 0, 1, 1).Ice(1)
                .Rect(Red, 0, 0, 1, 1)
                .Door(Blue, Side.Right, 0)
                .Door(Red, Side.Left, 0)
                .Build();

        private static LevelData StuckLevel() =>
            new LevelBuilder(2, 2)
                .Rect(Blue, 0, 1, 1, 1).Arrow(ArrowAxis.Vertical)
                .Rect(Red, 0, 0, 1, 1)
                .Rect(Green, 1, 1, 1, 1).Ice(1)
                .Door(Blue, Side.Top, 0)
                .Door(Red, Side.Bottom, 1)
                .Door(Green, Side.Right, 1)
                .Build();
    }
}
