using Game.Data.Levels;
using Game.Editor.LevelEditor;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.LevelEditor
{
    public sealed class SelectToolTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void PressOnABlock_SelectsIt_AndDraggingMovesIt()
        {
            var level = new LevelBuilder(4, 4).Rect(Red, 0, 0, 1, 2).Build();
            var tool = new SelectTool();

            tool.Press(level, new Cell(0, 1), Red);
            Assert.IsInstanceOf<BlockSelection>(tool.Selection);
            Assert.IsTrue(tool.Drag(level, new Cell(1, 1)));
            Assert.IsTrue(tool.Drag(level, new Cell(2, 1)));

            Assert.AreEqual(new Cell(2, 0), level.Blocks[0].Position);
        }

        [Test]
        public void DraggingABlockIntoAnother_LeavesItWhereItLastFit()
        {
            var level = new LevelBuilder(4, 4).Rect(Red, 0, 0, 1, 1).Rect(Blue, 2, 0, 1, 1).Build();
            var tool = new SelectTool();

            tool.Press(level, new Cell(0, 0), Red);
            tool.Drag(level, new Cell(1, 0));
            Assert.IsFalse(tool.Drag(level, new Cell(2, 0)));

            Assert.AreEqual(new Cell(1, 0), level.Blocks[0].Position);
        }

        [Test]
        public void PressOnADoor_SelectsIt_AndDraggingMovesItAlongItsSide()
        {
            var level = new LevelBuilder(5, 4).Rect(Red, 0, 0, 1, 1).Door(Red, Side.Top, 0, 2).Build();
            var tool = new SelectTool();

            tool.Press(level, new Cell(1, -1), Red);
            Assert.IsInstanceOf<DoorSelection>(tool.Selection);
            Assert.IsTrue(tool.Drag(level, new Cell(3, 0)), "only the movement along the side counts");

            Assert.AreEqual(2, level.Doors[0].Start);
            Assert.AreEqual(2, level.Doors[0].Length);
        }

        [Test]
        public void Selection_StillMeansItsBlock_WhenAnEarlierBlockIsRemoved()
        {
            var level = new LevelBuilder(4, 4).Rect(Red, 0, 0, 1, 1).Rect(Blue, 2, 2, 1, 1).Build();
            var tool = new SelectTool();
            tool.Press(level, new Cell(2, 2), Red);

            level.Blocks.RemoveAt(0);

            Assert.IsTrue(tool.Selection.Exists(level));
            Assert.AreEqual(Blue, tool.Selection.ColorId(level));
            tool.Selection.Delete(level);
            Assert.IsEmpty(level.Blocks);
            Assert.IsFalse(tool.Selection.Exists(level));
        }

        [Test]
        public void PressOnNothing_ClearsTheSelection()
        {
            var level = new LevelBuilder(4, 4).Rect(Red, 0, 0, 1, 1).Build();
            var tool = new SelectTool();
            tool.Press(level, new Cell(0, 0), Red);

            tool.Press(level, new Cell(3, 3), Red);

            Assert.IsNull(tool.Selection);
        }

        [Test]
        public void Selection_RecolorsAndDeletesWhatItSelected()
        {
            var level = new LevelBuilder(4, 4).Rect(Red, 0, 0, 1, 1).Door(Red, Side.Top, 0).Build();
            var tool = new SelectTool();

            tool.Press(level, new Cell(0, -1), Red);
            tool.Selection.Recolor(level, Blue);
            Assert.AreEqual(Blue, level.Doors[0].ColorId);

            tool.Selection.Delete(level);
            Assert.IsEmpty(level.Doors);
            Assert.IsFalse(tool.Selection.Exists(level));
        }
    }
}
