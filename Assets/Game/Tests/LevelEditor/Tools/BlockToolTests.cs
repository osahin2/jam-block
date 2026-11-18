using Game.Data.Levels;
using Game.Editor.LevelEditor.Editing;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.LevelEditor
{
    public sealed class BlockToolTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void Stroke_OnEmptyCells_DrawsOneBlockInTheChosenColor()
        {
            var level = new LevelBuilder(4, 4).Build();
            var tool = new BlockTool();

            Assert.IsTrue(tool.Press(level, new Cell(0, 0), Red));
            Assert.IsTrue(tool.Drag(level, new Cell(1, 0)));
            Assert.IsTrue(tool.Drag(level, new Cell(1, 1)));
            tool.Release();

            Assert.AreEqual(1, level.Blocks.Count);
            Assert.AreEqual(Red, level.Blocks[0].ColorId);
            Assert.AreEqual(3, level.Blocks[0].Shape.Count);
        }

        [Test]
        public void Stroke_StartingOnABlock_ExtendsThatBlock()
        {
            var level = new LevelBuilder(4, 4).Rect(Red, 0, 0, 1, 1).Build();
            var tool = new BlockTool();

            Assert.IsFalse(tool.Press(level, new Cell(0, 0), Blue));
            Assert.IsTrue(tool.Drag(level, new Cell(0, 1)));

            Assert.AreEqual(1, level.Blocks.Count);
            Assert.AreEqual(Red, level.Blocks[0].ColorId);
            Assert.AreEqual(2, level.Blocks[0].Shape.Count);
        }

        [Test]
        public void Stroke_StartingOffTheBoard_ChangesNothing()
        {
            var level = new LevelBuilder(4, 4).Build();
            var tool = new BlockTool();

            Assert.IsFalse(tool.Press(level, new Cell(-1, 0), Red));
            Assert.IsFalse(tool.Drag(level, new Cell(0, 0)));

            Assert.IsEmpty(level.Blocks);
        }

        [Test]
        public void Stroke_AfterRelease_StartsANewBlock()
        {
            var level = new LevelBuilder(4, 4).Build();
            var tool = new BlockTool();

            tool.Press(level, new Cell(0, 0), Red);
            tool.Release();
            tool.Press(level, new Cell(2, 0), Blue);
            tool.Release();

            Assert.AreEqual(2, level.Blocks.Count);
            Assert.AreEqual(Blue, level.Blocks[1].ColorId);
        }
    }
}
