using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class VacuumEffectTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        private readonly VacuumEffect _vacuum = new VacuumEffect();
        private LevelMechanics _mechanics;
        private Board _board;

        [Test]
        public void CanTarget_AnUnlockedBlock_IsTrue_ALockedOneOrAnEmptyCell_IsFalse()
        {
            Build(iceOnFirstRed: 1);

            Assert.IsTrue(_vacuum.CanTarget(_board, new Cell(1, 0)));
            Assert.IsFalse(_vacuum.CanTarget(_board, new Cell(0, 0)));
            Assert.IsFalse(_vacuum.CanTarget(_board, new Cell(3, 3)));
        }

        [Test]
        public void Apply_TakesEveryUnlockedBlockOfTheTappedColor_AndNoOther()
        {
            Build(iceOnFirstRed: 0);

            _vacuum.Apply(_board, new Cell(1, 0));

            Assert.IsTrue(_board.Blocks[0].IsGone);
            Assert.IsTrue(_board.Blocks[1].IsGone);
            Assert.IsTrue(_board.Blocks[2].IsGone);
            Assert.IsFalse(_board.Blocks[3].IsGone);
        }

        [Test]
        public void Apply_ReadsTheLocksAsTheyAreAtTheTap()
        {
            Build(iceOnFirstRed: 1);

            _vacuum.Apply(_board, new Cell(1, 0));

            Assert.IsFalse(_board.Blocks[0].IsGone);
            Assert.AreEqual(0, _mechanics.Ice.IceLeft(_board, 0));
            Assert.IsTrue(_board.Blocks[1].IsGone);
            Assert.IsTrue(_board.Blocks[2].IsGone);
        }

        [Test]
        public void Apply_AgainOnABoardWithFewerBlocks_TakesOnlyWhatIsLeft()
        {
            Build(iceOnFirstRed: 0);
            _vacuum.Apply(_board, new Cell(3, 0));

            _vacuum.Apply(_board, new Cell(1, 0));

            Assert.AreEqual(4, _board.GoneCount);
            Assert.IsTrue(_board.IsCleared);
        }

        private void Build(int iceOnFirstRed)
        {
            var level = new LevelBuilder(4, 4)
                .Rect(Red, 0, 0, 1, 1).Rect(Red, 1, 0, 1, 1).Rect(Red, 2, 2, 2, 1).Rect(Blue, 3, 0, 1, 1)
                .Build();
            level.Blocks[0].Ice = iceOnFirstRed;
            _mechanics = LevelMechanics.For(level);
            _board = new Board(level, _mechanics.Rules, _mechanics.Guards);
        }
    }
}
