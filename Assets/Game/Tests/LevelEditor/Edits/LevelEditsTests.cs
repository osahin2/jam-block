using Game.Data.Levels;
using Game.Editor.LevelEditor;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.LevelEditor
{
    public sealed class LevelEditsTests
    {
        private const int Red = 1;

        [Test]
        public void AddBlock_MakesAOneCellBlockAtTheCell()
        {
            var level = new LevelBuilder(3, 3).Build();

            var index = LevelEdits.AddBlock(level, Red, new Cell(1, 2));

            Assert.AreEqual(0, index);
            Assert.AreEqual(Red, level.Blocks[0].ColorId);
            Assert.AreEqual(index, LevelEdits.BlockAt(level, new Cell(1, 2)));
            Assert.AreEqual(LevelEdits.NoBlock, LevelEdits.BlockAt(level, new Cell(1, 1)));
        }

        [Test]
        public void TryGrowBlock_AddsAFreeCellTouchingTheBlock()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Build();

            Assert.IsTrue(LevelEdits.TryGrowBlock(level, 0, new Cell(1, 0)));
            Assert.AreEqual(0, LevelEdits.BlockAt(level, new Cell(1, 0)));
            Assert.AreEqual(2, level.Blocks[0].Shape.Count);
        }

        [Test]
        public void TryGrowBlock_RefusesACellTouchingOnlyAtACorner()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Build();

            Assert.IsFalse(LevelEdits.TryGrowBlock(level, 0, new Cell(2, 2)));
            Assert.AreEqual(1, level.Blocks[0].Shape.Count);
        }

        [Test]
        public void TryGrowBlock_RefusesACellOfAnotherBlockOrOffTheBoard()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Red, 1, 0, 1, 1).Build();

            Assert.IsFalse(LevelEdits.TryGrowBlock(level, 0, new Cell(1, 0)));
            Assert.IsFalse(LevelEdits.TryGrowBlock(level, 0, new Cell(-1, 0)));
            Assert.AreEqual(1, level.Blocks[0].Shape.Count);
        }

        [Test]
        public void Resize_Smaller_DropsTheCellsAndDoorsThatFallOutside()
        {
            var level = new LevelBuilder(4, 4)
                .Rect(Red, 0, 0, 2, 1)
                .Rect(Red, 2, 2, 2, 2)
                .Door(Red, Side.Top, 0, 2)
                .Door(Red, Side.Top, 2, 2)
                .Build();

            LevelEdits.Resize(level, 3, 2);

            Assert.AreEqual(3, level.Width);
            Assert.AreEqual(2, level.Height);
            Assert.AreEqual(1, level.Blocks.Count, "the block wholly outside is removed");
            Assert.AreEqual(2, level.Blocks[0].Shape.Count, "the block inside is kept");
            Assert.AreEqual(1, level.Doors.Count, "the door past the new side end is removed");
            Assert.AreEqual(0, level.Doors[0].Start);
        }

        [Test]
        public void Resize_Larger_KeepsEverything()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 3, 3).Door(Red, Side.Left, 0, 3).Build();

            LevelEdits.Resize(level, 5, 6);

            Assert.AreEqual(9, level.Blocks[0].Shape.Count);
            Assert.AreEqual(1, level.Doors.Count);
        }
    }
}
