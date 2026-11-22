using Game.Data.Levels;
using Game.Editor.LevelEditor;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.LevelEditor
{
    public sealed class IceToolTests
    {
        private const int Red = 1;

        [Test]
        public void ClickingABlock_AddsOneIce()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 2, 1).Build();
            var tool = new IceTool();

            Assert.IsTrue(tool.Press(level, new Cell(1, 0), Red));
            tool.Release();
            tool.Press(level, new Cell(0, 0), Red);

            Assert.AreEqual(2, level.Blocks[0].Ice);
        }

        [Test]
        public void RightClickingABlock_TakesOneIceOff()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Ice(2).Build();

            Assert.IsTrue(new IceTool().RightClick(level, new Cell(0, 0)));

            Assert.AreEqual(1, level.Blocks[0].Ice);
        }

        [Test]
        public void RightClickingABlockWithoutIce_ChangesNothing()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Build();

            Assert.IsFalse(new IceTool().RightClick(level, new Cell(0, 0)));
            Assert.AreEqual(0, level.Blocks[0].Ice);
        }

        [Test]
        public void ClickingOrDraggingOffABlock_ChangesNothing()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Build();
            var tool = new IceTool();

            Assert.IsFalse(tool.Press(level, new Cell(2, 2), Red));
            Assert.IsFalse(tool.Drag(level, new Cell(0, 0)));
            Assert.IsFalse(tool.RightClick(level, new Cell(2, 2)));
            Assert.AreEqual(0, level.Blocks[0].Ice);
        }
    }
}
