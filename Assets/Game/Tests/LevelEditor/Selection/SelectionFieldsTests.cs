using Game.Data.Levels;
using Game.Editor.LevelEditor;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.LevelEditor
{
    public sealed class SelectionFieldsTests
    {
        private const int Red = 1;

        [Test]
        public void ABlocksIceField_ReadsAndWritesItsIce()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Red, 2, 2, 1, 1).Build();
            var ice = Field(new BlockSelection(level.Blocks[1]), "Ice");

            ice.Set(level, 3);

            Assert.AreEqual(3, level.Blocks[1].Ice);
            Assert.AreEqual(3, ice.Value(level));
            Assert.AreEqual(0, level.Blocks[0].Ice);
        }

        [Test]
        public void ABlocksIceField_KeepsItsBlockWhenAnEarlierBlockIsRemoved()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Rect(Red, 2, 2, 1, 1).Build();
            var ice = Field(new BlockSelection(level.Blocks[1]), "Ice");

            level.Blocks.RemoveAt(0);
            ice.Set(level, 2);

            Assert.AreEqual(2, level.Blocks[0].Ice);
        }

        [Test]
        public void SetIce_BelowZero_SetsNoIce()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Ice(2).Build();

            IceEdits.Set(level.Blocks[0], -1);

            Assert.AreEqual(0, level.Blocks[0].Ice);
        }

        [Test]
        public void ADoor_HasNoFields()
        {
            var level = new LevelBuilder(3, 3).Door(Red, Side.Top, 0).Build();

            Assert.IsEmpty(new DoorSelection(level.Doors[0]).Fields);
        }

        private static SelectionField Field(ILevelSelection selection, string label)
        {
            foreach (var field in selection.Fields)
            {
                if (field.Label == label)
                    return field;
            }
            Assert.Fail($"no field {label}");
            return null;
        }
    }
}
