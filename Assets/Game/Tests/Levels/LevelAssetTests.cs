using System.Collections.Generic;
using Game.Data.Levels;
using Game.Levels;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Levels
{
    public sealed class LevelAssetTests
    {
        [Test]
        public void LevelData_SurvivesUnitySerialization()
        {
            var source = ScriptableObject.CreateInstance<LevelAsset>();
            source.Level.Width = 4;
            source.Level.Height = 5;
            source.Level.TimeLimitSeconds = 90;
            source.Level.Blocks.Add(new BlockData { ColorId = 3, Position = new Cell(1, 2), Shape = new List<Cell> { new Cell(0, 0), new Cell(1, 0) } });
            source.Level.Doors.Add(new DoorData { ColorId = 3, Side = Side.Right, Start = 2, Length = 2 });
            var copy = ScriptableObject.CreateInstance<LevelAsset>();

            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(source), copy);

            Assert.AreEqual(4, copy.Level.Width);
            Assert.AreEqual(5, copy.Level.Height);
            Assert.AreEqual(90, copy.Level.TimeLimitSeconds);
            Assert.AreEqual(3, copy.Level.Blocks[0].ColorId);
            Assert.AreEqual(new Cell(1, 2), copy.Level.Blocks[0].Position);
            CollectionAssert.AreEqual(source.Level.Blocks[0].Shape, copy.Level.Blocks[0].Shape);
            Assert.AreEqual(Side.Right, copy.Level.Doors[0].Side);
            Assert.AreEqual(2, copy.Level.Doors[0].Start);
            Assert.AreEqual(2, copy.Level.Doors[0].Length);
            Object.DestroyImmediate(source);
            Object.DestroyImmediate(copy);
        }
    }
}
