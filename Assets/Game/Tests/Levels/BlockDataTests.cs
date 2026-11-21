using Game.Data.Levels;
using NUnit.Framework;

namespace Game.Tests.Levels
{
    public sealed class BlockDataTests
    {
        [Test]
        public void Extents_OfAnL_CountCellsAlongAndAcrossEachSide()
        {
            var block = new BlockData();
            block.Shape.AddRange(new[] { new Cell(0, 0), new Cell(0, 1), new Cell(0, 2), new Cell(1, 2) });

            Assert.AreEqual(2, block.ExtentAlong(Side.Top), "the width a top door needs");
            Assert.AreEqual(3, block.ExtentAcross(Side.Bottom), "how far it slides through a bottom door");
            Assert.AreEqual(3, block.ExtentAlong(Side.Left));
            Assert.AreEqual(2, block.ExtentAcross(Side.Right));
        }
    }
}
