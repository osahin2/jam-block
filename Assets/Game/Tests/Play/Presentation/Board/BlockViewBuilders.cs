using System.Collections.Generic;
using Game.Play;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Play
{
    public static class BlockViewBuilders
    {
        public static IReadOnlyList<IBlockFeature> TheGamesFeatures() =>
            BlockFeatures.Of(Load<IceSettings>(), Load<BoardArtSettings>());

        public static BlockViewBuilder WithTheGamesFeatures()
        {
            var art = Load<BoardArtSettings>();
            return new BlockViewBuilder(art, Load<ColorPalette>(), Load<BlockMotionSettings>(), new BlockPieceBuilder(art),
                TheGamesFeatures());
        }

        public static LevelStage StageWithTheGamesFeatures(CameraFramingSettings framing)
        {
            var art = Load<BoardArtSettings>();
            var builder = new BoardBuilder(art, Load<ColorPalette>(), Load<BlockMotionSettings>(), WithTheGamesFeatures());
            return new LevelStage(art, builder, new CameraFraming(framing));
        }

        private static T Load<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { "Assets/Game" });
            Assert.AreEqual(1, guids.Length, $"one {typeof(T).Name} asset");
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
