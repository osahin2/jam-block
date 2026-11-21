using Game.Data.Levels;
using NUnit.Framework;

namespace Game.Tests.Levels
{
    public sealed class ShapeMiddleTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void Of_ASquare_IsItsCenter()
        {
            var shape = new[] { new Cell(0, 0), new Cell(1, 0), new Cell(0, 1), new Cell(1, 1) };

            AssertAt(0.5f, 0.5f, ShapeMiddle.Of(shape));
        }

        [Test]
        public void Of_AnL_IsItsCenterWhenThatLiesOnTheShape()
        {
            var shape = new[] { new Cell(0, 0), new Cell(0, 1), new Cell(0, 2), new Cell(1, 2) };

            AssertAt(0.25f, 1.25f, ShapeMiddle.Of(shape));
        }

        [Test]
        public void Of_AShapeWhoseCenterIsOffIt_IsTheCellNearestTheCenter()
        {
            var shape = new[]
            {
                new Cell(0, 0), new Cell(1, 0), new Cell(2, 0), new Cell(3, 0),
                new Cell(0, 1), new Cell(0, 2), new Cell(0, 3)
            };

            AssertAt(1f, 0f, ShapeMiddle.Of(shape));
        }

        private static void AssertAt(float x, float y, BoardPoint middle)
        {
            Assert.AreEqual(x, middle.X, Tolerance);
            Assert.AreEqual(y, middle.Y, Tolerance);
        }
    }
}
