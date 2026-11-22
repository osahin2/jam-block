using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class HammerEffectTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        private readonly HammerEffect _hammer = new HammerEffect();

        [Test]
        public void CanTarget_ABlock_IcedOrNot_IsTrue_AndAnEmptyCell_IsFalse()
        {
            var board = BoardWith(iceOnRed: 2);

            Assert.IsTrue(_hammer.CanTarget(board, new Cell(0, 0)));
            Assert.IsTrue(_hammer.CanTarget(board, new Cell(2, 2)));
            Assert.IsFalse(_hammer.CanTarget(board, new Cell(1, 1)));
        }

        [Test]
        public void Apply_OnAnUnlockedBlock_TakesItWhole()
        {
            var board = BoardWith(iceOnRed: 0);

            _hammer.Apply(board, new Cell(0, 1));

            Assert.IsTrue(board.Blocks[0].IsGone);
            Assert.AreEqual(Board.NoBlock, board.BlockAt(new Cell(0, 0)));
            Assert.IsFalse(board.Blocks[1].IsGone);
        }

        [Test]
        public void Apply_OnALockedBlock_TakesOneIceAndKeepsIt()
        {
            var level = Level();
            level.Blocks[0].Ice = 2;
            var mechanics = LevelMechanics.For(level);
            var board = new Board(level, mechanics.Rules, mechanics.Guards);

            _hammer.Apply(board, new Cell(0, 0));

            Assert.IsFalse(board.Blocks[0].IsGone);
            Assert.AreEqual(1, mechanics.Ice.IceLeft(board, 0));
        }

        private static Board BoardWith(int iceOnRed)
        {
            var level = Level();
            level.Blocks[0].Ice = iceOnRed;
            return Boards.Of(level);
        }

        private static LevelData Level() =>
            new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 2).Rect(Blue, 2, 2, 1, 1).Build();
    }
}
