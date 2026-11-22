using System.Linq;
using System.Reflection;
using Game.Data.Levels;
using Game.Logic.Levels;
using Game.Logic.Play;
using Game.Tests.Play;
using NUnit.Framework;

namespace Game.Tests.Levels
{
    public sealed class ArrangementKeyTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void Read_TwoAlikeBlocksSwapped_GivesEqualKeysWithTheSameHash()
        {
            var level = TwoBlocks(Red, Red);
            var alike = new InterchangeableBlocks(level);
            var swapped = Boards.Of(level);
            swapped.Place(0, new Cell(0, 1));
            swapped.Place(1, new Cell(0, 0));
            swapped.Place(0, new Cell(2, 0));

            var first = KeyOf(alike, Boards.Of(level));
            var second = KeyOf(alike, swapped);

            Assert.AreEqual(first, second);
            Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }

        [Test]
        public void Read_TwoDifferentBlocksSwapped_GivesDifferentKeys()
        {
            var level = TwoBlocks(Red, Blue);
            var alike = new InterchangeableBlocks(level);
            var swapped = Boards.Of(level);
            swapped.Place(0, new Cell(0, 1));
            swapped.Place(1, new Cell(0, 0));
            swapped.Place(0, new Cell(2, 0));

            Assert.AreNotEqual(KeyOf(alike, Boards.Of(level)), KeyOf(alike, swapped));
        }

        [Test]
        public void Read_ABlockMovedOrGone_GivesADifferentKey()
        {
            var level = TwoBlocks(Red, Blue);
            var alike = new InterchangeableBlocks(level);
            var moved = Boards.Of(level);
            moved.Place(0, new Cell(1, 1));
            var gone = Boards.Of(level);
            gone.TryLeave(1, out _);

            var start = KeyOf(alike, Boards.Of(level));

            Assert.AreNotEqual(start, KeyOf(alike, moved));
            Assert.AreNotEqual(start, KeyOf(alike, gone));
        }

        [Test]
        public void AreAlike_BlocksDifferingInColorShapeIceOrArrow_AreNot()
        {
            var level = new LevelBuilder(6, 6)
                .Rect(Red, 0, 0, 1, 1)
                .Rect(Red, 1, 0, 1, 1)
                .Rect(Blue, 2, 0, 1, 1)
                .Rect(Red, 0, 2, 2, 1)
                .Rect(Red, 3, 0, 1, 1).Ice(1)
                .Rect(Red, 4, 0, 1, 1).Arrow(ArrowAxis.Vertical)
                .Build();
            var blocks = level.Blocks;

            Assert.IsTrue(InterchangeableBlocks.AreAlike(blocks[0], blocks[1]));
            for (var other = 2; other < blocks.Count; other++)
                Assert.IsFalse(InterchangeableBlocks.AreAlike(blocks[0], blocks[other]), $"block {other}");
        }

        [Test]
        public void AreAlike_ComparesEveryFieldOfABlockButItsPosition()
        {
            Assert.AreEqual(new[] { "Arrow", "ColorId", "Ice", "Position", "Shape" }, FieldNames(typeof(BlockData)));
            Assert.AreEqual(new[] { "Axis", "Length", "Start" }, FieldNames(typeof(ArrowData)));
        }

        private static ArrangementKey KeyOf(InterchangeableBlocks alike, Board board)
        {
            var key = new ArrangementKey(alike.BlockCount);
            key.Read(alike, board.Blocks);
            return key;
        }

        private static string[] FieldNames(System.Type type) =>
            type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(field => field.Name).OrderBy(name => name).ToArray();

        private static LevelData TwoBlocks(int first, int second) =>
            new LevelBuilder(3, 2)
                .Rect(first, 0, 0, 1, 1)
                .Rect(second, 2, 0, 1, 1)
                .Door(Blue, Side.Right, 0)
                .Door(Red, Side.Left, 0)
                .Build();
    }
}
