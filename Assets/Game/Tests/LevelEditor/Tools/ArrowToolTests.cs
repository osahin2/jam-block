using Game.Data.Levels;
using Game.Editor.LevelEditor;
using Game.Play;
using Game.Tests.Levels;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.LevelEditor
{
    public sealed class ArrowToolTests
    {
        private const int Red = 1;

        [Test]
        public void DraggingAlongARow_DrawsAHorizontalArrowFromThePressedCell()
        {
            var level = new LevelBuilder(5, 3).Rect(Red, 1, 1, 3, 1).Build();

            Stroke(level, new Cell(1, 1), new Cell(2, 1), new Cell(3, 1));

            AssertArrow(level.Blocks[0].Arrow, ArrowAxis.Horizontal, new Cell(0, 0), 3);
        }

        [Test]
        public void DraggingBackward_StartsTheArrowWhereTheDragEnds()
        {
            var level = new LevelBuilder(5, 3).Rect(Red, 1, 1, 3, 1).Build();

            Stroke(level, new Cell(3, 1), new Cell(2, 1));

            AssertArrow(level.Blocks[0].Arrow, ArrowAxis.Horizontal, new Cell(1, 0), 2);
        }

        [Test]
        public void DraggingAlongAColumn_DrawsAVerticalArrow()
        {
            var level = new LevelBuilder(3, 4).Rect(Red, 1, 0, 1, 3).Build();

            Stroke(level, new Cell(1, 0), new Cell(1, 1));

            AssertArrow(level.Blocks[0].Arrow, ArrowAxis.Vertical, new Cell(0, 0), 2);
        }

        [Test]
        public void DraggingPastTheBlock_EndsTheArrowOnItsLastCell()
        {
            var level = new LevelBuilder(5, 3).Rect(Red, 0, 0, 2, 1).Build();

            Stroke(level, new Cell(0, 0), new Cell(1, 0), new Cell(2, 0), new Cell(3, 0));

            AssertArrow(level.Blocks[0].Arrow, ArrowAxis.Horizontal, new Cell(0, 0), 2);
        }

        [Test]
        public void DraggingFar_DrawsNoLongerThanTheLongestArrow()
        {
            var level = new LevelBuilder(6, 1).Rect(Red, 0, 0, 5, 1).Build();

            Stroke(level, new Cell(0, 0), new Cell(1, 0), new Cell(2, 0), new Cell(3, 0), new Cell(4, 0));

            Assert.AreEqual(ArrowTool.MaxLength, level.Blocks[0].Arrow.Length);
        }

        [Test]
        public void ClickingABlockWithAnArrow_RemovesIt()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 2, 1).Arrow(ArrowAxis.Horizontal, 0, 0, 2).Build();
            var tool = new ArrowTool();

            Assert.IsTrue(tool.Press(level, new Cell(1, 0), Red));
            tool.Release();

            Assert.IsFalse(level.Blocks[0].Arrow.IsSet);
        }

        [Test]
        public void DraggingBackToThePressedCell_RemovesTheArrow()
        {
            var level = new LevelBuilder(5, 3).Rect(Red, 0, 0, 3, 1).Build();

            Stroke(level, new Cell(0, 0), new Cell(1, 0), new Cell(0, 0));

            Assert.IsFalse(level.Blocks[0].Arrow.IsSet);
        }

        [Test]
        public void PressingAnEmptyCell_DrawsNothing()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Build();
            var tool = new ArrowTool();

            Assert.IsFalse(tool.Press(level, new Cell(2, 2), Red));
            Assert.IsFalse(tool.Drag(level, new Cell(2, 1)));
        }

        [Test]
        public void TheBlockPrefab_HasAnArrowForEveryLengthTheToolDraws()
        {
            var block = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Game/Runtime/Play/Prefabs/Blocks/Block.prefab");

            Assert.AreEqual(ArrowTool.MaxLength, block.GetComponentInChildren<BlockArrow>(true).LongestModel);
        }

        private static void Stroke(LevelData level, Cell press, params Cell[] drags)
        {
            var tool = new ArrowTool();
            tool.Press(level, press, Red);
            foreach (var cell in drags)
                tool.Drag(level, cell);
            tool.Release();
        }

        private static void AssertArrow(ArrowData arrow, ArrowAxis axis, Cell start, int length)
        {
            Assert.AreEqual(axis, arrow.Axis);
            Assert.AreEqual(start, arrow.Start);
            Assert.AreEqual(length, arrow.Length);
        }
    }
}
