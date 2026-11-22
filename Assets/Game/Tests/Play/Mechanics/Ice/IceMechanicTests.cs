using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class IceMechanicTests
    {
        private const int Red = 1;
        private const int Blue = 2;
        private const int Green = 3;

        private LevelMechanics _mechanics;
        private Board _board;

        [Test]
        public void IceLeft_AtTheStart_IsTheLevelsIce()
        {
            Build(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Blue, 2, 2, 1, 1).Build(), 2);

            Assert.AreEqual(2, _mechanics.Ice.IceLeft(_board, 0));
            Assert.AreEqual(0, _mechanics.Ice.IceLeft(_board, 1));
        }

        [Test]
        public void ABlockWithIceLeft_CannotStepOrLeave()
        {
            Build(new LevelBuilder(3, 3).Rect(Red, 1, 0, 1, 1).Door(Red, Side.Top, 1).Build(), 1);

            Assert.IsFalse(_board.CanStep(0, Direction.Down));
            Assert.IsFalse(_board.Step(0, Direction.Down).Moved);
            Assert.IsFalse(_board.TryLeave(0, out _));
            Assert.IsTrue(_board.IsLocked(0));
        }

        [Test]
        public void IceLeft_DropsByOneForEveryBlockGone_ThroughADoorOrHit()
        {
            Build(new LevelBuilder(4, 4)
                .Rect(Red, 0, 3, 1, 1).Rect(Blue, 1, 0, 1, 1).Rect(Green, 3, 3, 1, 1)
                .Door(Blue, Side.Top, 1).Build(), 3);

            _board.TryLeave(1, out _);
            Assert.AreEqual(2, _mechanics.Ice.IceLeft(_board, 0));

            _board.Hit(2);
            Assert.AreEqual(1, _mechanics.Ice.IceLeft(_board, 0));
        }

        [Test]
        public void Hit_OnABlockWithIceLeft_TakesOneIceAndKeepsTheBlock()
        {
            Build(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Build(), 2);

            Assert.IsTrue(_board.IsGuarded(0));
            _board.Hit(0);

            Assert.AreEqual(1, _mechanics.Ice.IceLeft(_board, 0));
            Assert.IsFalse(_board.Blocks[0].IsGone);
            Assert.AreEqual(0, _board.GoneCount);
        }

        [Test]
        public void Hit_OnceTheIceIsGone_TakesTheBlock()
        {
            Build(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Blue, 2, 2, 1, 1).Build(), 1);
            _board.Hit(0);
            Assert.IsFalse(_board.IsGuarded(0));

            _board.Hit(0);

            Assert.IsTrue(_board.Blocks[0].IsGone);
            Assert.AreEqual(0, _mechanics.Ice.IceLeft(_board, 0));
        }

        [Test]
        public void ABlockWhoseIceIsGone_PlaysAsAnyOther()
        {
            Build(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Rect(Blue, 0, 2, 1, 1).Door(Red, Side.Top, 1).Build(), 1);

            _board.Hit(1);

            Assert.IsFalse(_board.IsLocked(0));
            Assert.IsTrue(_board.CanStep(0, Direction.Left));
            Assert.IsTrue(_board.Step(0, Direction.Up).HasLeft);
        }

        [Test]
        public void IceLeft_NeverDropsBelowZero()
        {
            Build(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Blue, 2, 0, 1, 1).Rect(Green, 2, 2, 1, 1).Build(), 1);

            _board.Hit(1);
            _board.Hit(2);

            Assert.AreEqual(0, _mechanics.Ice.IceLeft(_board, 0));
        }

        private void Build(LevelData level, int iceOnFirstBlock)
        {
            level.Blocks[0].Ice = iceOnFirstBlock;
            _mechanics = LevelMechanics.For(level);
            _board = new Board(level, _mechanics.Rules, _mechanics.Guards);
        }
    }
}
