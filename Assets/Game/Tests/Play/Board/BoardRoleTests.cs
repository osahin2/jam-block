using Game.Data.Levels;
using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class BoardRoleTests
    {
        private const int Red = 1;

        private static readonly IBlockGuard[] NoGuards = new IBlockGuard[0];

        [Test]
        public void CanStep_InADirectionAMoveRuleForbids_IsFalseAndTheOthersStayFree()
        {
            var board = BoardWith(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Build(), Direction.Left);

            Assert.IsFalse(board.CanStep(0, Direction.Left));
            Assert.IsTrue(board.CanStep(0, Direction.Right));
            Assert.IsFalse(board.Step(0, Direction.Left).Moved);
            Assert.AreEqual(new Cell(1, 1), board.Blocks[0].Position);
        }

        [Test]
        public void TryLeave_AsksTheMoveRulesWithTheDirectionOutOfTheDoor()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 1, 0, 1, 1).Door(Red, Side.Top, 1).Build();

            var forbidsUp = BoardWith(level, Direction.Up);
            var forbidsDown = BoardWith(level, Direction.Down);

            Assert.IsFalse(forbidsUp.TryLeave(0, out var noDoor));
            Assert.AreEqual(StepResult.NoDoor, noDoor);
            Assert.IsTrue(forbidsDown.TryLeave(0, out var door));
            Assert.AreEqual(0, door);
        }

        [Test]
        public void Hit_WithoutGuards_TakesTheBlock()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Build());

            Assert.IsFalse(board.IsGuarded(0));
            board.Hit(0);

            Assert.IsTrue(board.Blocks[0].IsGone);
        }

        private static Board BoardWith(LevelData level, Direction forbidden) =>
            new Board(level, new IMoveRule[] { new ForbiddenDirectionRule(forbidden) }, NoGuards);
    }
}
