using Game.Data.Levels;
using Game.Editor.LevelEditor;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.LevelEditor
{
    public sealed class DoorAndEraseToolTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void DoorTool_PressOnTheFrame_PlacesADoor_AndDraggingMakesItLonger()
        {
            var level = new LevelBuilder(5, 4).Build();
            var tool = new DoorTool();

            Assert.IsTrue(tool.Press(level, new Cell(3, -1), Red));
            Assert.IsTrue(tool.Drag(level, new Cell(2, -1)));
            Assert.IsTrue(tool.Drag(level, new Cell(1, -1)));
            tool.Release();

            var door = level.Doors[0];
            Assert.AreEqual(Side.Top, door.Side);
            Assert.AreEqual(Red, door.ColorId);
            Assert.AreEqual(1, door.Start);
            Assert.AreEqual(3, door.Length);
        }

        [Test]
        public void DoorTool_PressOnACornerOrTheBoard_PlacesNothing()
        {
            var level = new LevelBuilder(4, 4).Build();
            var tool = new DoorTool();

            Assert.IsFalse(tool.Press(level, new Cell(-1, -1), Red));
            Assert.IsFalse(tool.Press(level, new Cell(1, 1), Red));
            Assert.IsEmpty(level.Doors);
        }

        [Test]
        public void DoorTool_DragFromADoorsEnd_ChangesItsLength()
        {
            var level = new LevelBuilder(6, 4).Door(Red, Side.Left, 0, 3).Build();
            var tool = new DoorTool();

            Assert.IsFalse(tool.Press(level, new Cell(-1, 2), Blue), "pressing a door changes nothing yet");
            Assert.IsTrue(tool.Drag(level, new Cell(-1, 0)), "the bottom end follows, the top stays");

            Assert.AreEqual(0, level.Doors[0].Start);
            Assert.AreEqual(1, level.Doors[0].Length);
            Assert.AreEqual(Red, level.Doors[0].ColorId);
        }

        [Test]
        public void DoorTool_StopsAtAnotherDoor()
        {
            var level = new LevelBuilder(6, 4).Door(Blue, Side.Top, 3).Build();
            var tool = new DoorTool();

            tool.Press(level, new Cell(1, -1), Red);
            Assert.IsTrue(tool.Drag(level, new Cell(2, -1)));
            Assert.IsFalse(tool.Drag(level, new Cell(3, -1)));

            Assert.AreEqual(2, level.Doors[1].Length);
        }

        [Test]
        public void EraseTool_ErasesBlockCellsAndWholeDoors()
        {
            var level = new LevelBuilder(4, 4).Rect(Red, 0, 0, 3, 1).Door(Red, Side.Top, 0, 3).Build();
            var tool = new EraseTool();

            Assert.IsTrue(tool.Press(level, new Cell(0, 0), Red));
            Assert.IsTrue(tool.Drag(level, new Cell(1, 0)));
            Assert.IsFalse(tool.Drag(level, new Cell(1, 1)));
            Assert.AreEqual(1, level.Blocks[0].Shape.Count);

            Assert.IsTrue(tool.Press(level, new Cell(1, -1), Red));
            Assert.IsEmpty(level.Doors);
        }
    }
}
