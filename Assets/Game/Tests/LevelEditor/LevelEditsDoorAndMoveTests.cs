using Game.Data.Levels;
using Game.Editor.LevelEditor.Editing;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.LevelEditor
{
    public sealed class LevelEditsDoorAndMoveTests
    {
        private const int Red = 1;
        private const int Blue = 2;

        [Test]
        public void TryGetSide_RingCellsHaveASide_CornersAndBoardCellsDoNot()
        {
            var level = new LevelBuilder(3, 4).Build();

            Assert.IsTrue(LevelEdits.TryGetSide(level, new Cell(1, -1), out var top));
            Assert.AreEqual(Side.Top, top);
            Assert.IsTrue(LevelEdits.TryGetSide(level, new Cell(3, 2), out var right));
            Assert.AreEqual(Side.Right, right);
            Assert.IsFalse(LevelEdits.TryGetSide(level, new Cell(-1, -1), out _));
            Assert.IsFalse(LevelEdits.TryGetSide(level, new Cell(1, 1), out _));
            Assert.IsFalse(LevelEdits.TryGetSide(level, new Cell(1, -2), out _));
        }

        [Test]
        public void DoorAt_FindsTheDoorCoveringARingCell()
        {
            var level = new LevelBuilder(4, 4).Door(Red, Side.Left, 1, 2).Build();

            Assert.AreEqual(0, LevelEdits.DoorAt(level, new Cell(-1, 2)));
            Assert.AreEqual(LevelEdits.NoDoor, LevelEdits.DoorAt(level, new Cell(-1, 0)));
            Assert.AreEqual(LevelEdits.NoDoor, LevelEdits.DoorAt(level, new Cell(0, 2)));
        }

        [Test]
        public void DoorCells_AreTheRingCellsItCovers()
        {
            var level = new LevelBuilder(4, 3).Door(Red, Side.Bottom, 1, 2).Build();

            CollectionAssert.AreEqual(new[] { new Cell(1, 3), new Cell(2, 3) }, LevelEdits.DoorCells(level, 0));
        }

        [Test]
        public void AddDoor_RefusesACellAnotherDoorCovers()
        {
            var level = new LevelBuilder(4, 4).Door(Red, Side.Top, 0, 2).Build();

            Assert.AreEqual(LevelEdits.NoDoor, LevelEdits.AddDoor(level, Blue, Side.Top, 1));
            Assert.AreEqual(1, LevelEdits.AddDoor(level, Blue, Side.Top, 2));
        }

        [Test]
        public void TrySetDoorSpan_StaysOnItsSideAndOffOtherDoors()
        {
            var level = new LevelBuilder(5, 4).Door(Red, Side.Top, 0, 1).Door(Blue, Side.Top, 3, 1).Build();

            Assert.IsTrue(LevelEdits.TrySetDoorSpan(level, 0, 0, 3));
            Assert.IsFalse(LevelEdits.TrySetDoorSpan(level, 0, 0, 4), "over the blue door");
            Assert.IsFalse(LevelEdits.TrySetDoorSpan(level, 1, 3, 6), "past the end of the side");
            Assert.AreEqual(3, level.Doors[0].Length);
        }

        [Test]
        public void TryMoveBlock_MovesOnlyOntoFreeBoardCells()
        {
            var level = new LevelBuilder(4, 4).Rect(Red, 0, 0, 2, 1).Rect(Blue, 3, 0, 1, 1).Build();

            Assert.IsTrue(LevelEdits.TryMoveBlock(level, 0, new Cell(1, 0)), "onto its own former cell and a free one");
            Assert.IsFalse(LevelEdits.TryMoveBlock(level, 0, new Cell(2, 0)), "onto the blue block");
            Assert.IsFalse(LevelEdits.TryMoveBlock(level, 0, new Cell(3, 1)), "off the board");
            Assert.AreEqual(new Cell(1, 0), level.Blocks[0].Position);
        }

        [Test]
        public void TryRemoveBlockCell_RemovesTheCellAndAnEmptyBlock()
        {
            var level = new LevelBuilder(4, 4).Rect(Red, 0, 0, 2, 1).Build();

            Assert.IsTrue(LevelEdits.TryRemoveBlockCell(level, new Cell(0, 0)));
            Assert.AreEqual(1, level.Blocks[0].Shape.Count);
            Assert.IsTrue(LevelEdits.TryRemoveBlockCell(level, new Cell(1, 0)));
            Assert.IsEmpty(level.Blocks);
            Assert.IsFalse(LevelEdits.TryRemoveBlockCell(level, new Cell(1, 0)));
        }
    }
}
