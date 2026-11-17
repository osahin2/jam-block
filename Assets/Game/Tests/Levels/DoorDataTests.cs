using Game.Data.Levels;
using NUnit.Framework;

namespace Game.Tests.Levels
{
    public sealed class DoorDataTests
    {
        [Test]
        public void FitsSide_NeedsACellAndStaysWithinTheSide()
        {
            Assert.IsTrue(new DoorData { Side = Side.Left, Start = 2, Length = 2 }.FitsSide(3, 4), "left runs along the height");
            Assert.IsFalse(new DoorData { Side = Side.Top, Start = 2, Length = 2 }.FitsSide(3, 4), "past the end");
            Assert.IsFalse(new DoorData { Side = Side.Top, Start = -1 }.FitsSide(3, 4), "before the start");
            Assert.IsFalse(new DoorData { Side = Side.Top, Length = 0 }.FitsSide(3, 4), "no cells");
        }

        [Test]
        public void Overlaps_OnlySharedCellsOnTheSameSide()
        {
            var door = new DoorData { Side = Side.Top, Start = 1, Length = 2 };

            Assert.IsTrue(door.Overlaps(Side.Top, 2, 4));
            Assert.IsFalse(door.Overlaps(Side.Top, 3, 4), "side by side");
            Assert.IsFalse(door.Overlaps(Side.Bottom, 1, 3), "another side");
        }
    }
}
