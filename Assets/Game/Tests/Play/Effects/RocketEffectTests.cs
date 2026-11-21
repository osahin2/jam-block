using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class RocketEffectTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        private readonly RocketEffect _rocket = new RocketEffect();

        [Test]
        public void CanTarget_AnyCellOfABlock_IcedOrNot_IsTrue_AndAnEmptyCell_IsFalse()
        {
            var board = BoardWith(iceOnRed: 2);

            Assert.IsTrue(_rocket.CanTarget(board, new Cell(0, 0)));
            Assert.IsTrue(_rocket.CanTarget(board, new Cell(0, 2)));
            Assert.IsTrue(_rocket.CanTarget(board, new Cell(2, 2)));
            Assert.IsFalse(_rocket.CanTarget(board, new Cell(1, 1)));
        }

        [Test]
        public void Apply_OnTheMiddleOfAnUnlockedBlock_TakesTheCellAndSplitsTheRest()
        {
            var board = BoardWith(iceOnRed: 0);

            _rocket.Apply(board, new Cell(0, 1));

            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(0, 1)));
            Assert.AreEqual(0, board.BlockAt(new Cell(0, 0)));
            Assert.AreEqual(2, board.BlockAt(new Cell(0, 2)));
            Assert.AreEqual(3, board.RemainingBlocks);
        }

        [Test]
        public void Apply_OnALockedBlock_TakesOneIceAndKeepsTheCell()
        {
            var level = Level();
            level.Blocks[0].Ice = 2;
            var mechanics = LevelMechanics.For(level);
            var board = new Board(level, mechanics.Rules, mechanics.Guards);

            _rocket.Apply(board, new Cell(0, 1));

            Assert.AreEqual(0, board.BlockAt(new Cell(0, 1)));
            Assert.AreEqual(1, mechanics.Ice.IceLeft(board, 0));
        }

        private static Board BoardWith(int iceOnRed)
        {
            var level = Level();
            level.Blocks[0].Ice = iceOnRed;
            return Boards.Of(level);
        }

        private static LevelData Level() =>
            new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 3).Rect(Blue, 2, 2, 1, 1).Build();
    }
}
