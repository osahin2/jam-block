using System.Text;
using Game.Editor.LevelEditor;
using Game.Levels;
using Game.Logic.Levels;
using Game.Play;
using NUnit.Framework;
using UnityEditor;

namespace Game.Tests.Levels
{
    public sealed class ProjectLevelsTests
    {
        [Test]
        public void EveryLevelAsset_HasNoErrors()
        {
            var failures = new StringBuilder();
            var levels = LevelAssets();
            Assert.IsNotEmpty(levels, "No level assets found.");
            foreach (var asset in levels)
            {
                var report = LevelValidator.Validate(asset.Level);
                if (report.HasErrors)
                    failures.AppendLine($"{AssetDatabase.GetAssetPath(asset)}: {report}");
            }

            Assert.IsEmpty(failures.ToString());
        }

        [Test]
        public void EveryLevelAsset_IsSolvable()
        {
            var settingsGuids = AssetDatabase.FindAssets($"t:{nameof(SolverSettings)}");
            Assert.IsNotEmpty(settingsGuids);
            var budget = AssetDatabase.LoadAssetAtPath<SolverSettings>(AssetDatabase.GUIDToAssetPath(settingsGuids[0])).FullCheckBudget;
            var failures = new StringBuilder();
            foreach (var asset in LevelAssets())
            {
                var path = AssetDatabase.GetAssetPath(asset);
                var solution = LevelSolver.Solve(asset.Level, budget);
                TestContext.WriteLine(
                    $"{path}: {solution.Verdict}, {solution.Moves.Count} moves, {solution.Rearrangements} rearrangements, {solution.ExploredStates} states");
                if (solution.Verdict != Solvability.Solvable)
                    failures.AppendLine($"{path}: {solution.Verdict}");
            }

            Assert.IsEmpty(failures.ToString());
        }

        [Test]
        public void EveryColorIdInTheLevels_HasAMaterialInThePalette()
        {
            var palettes = AssetDatabase.FindAssets($"t:{nameof(ColorPalette)}");
            Assert.AreEqual(1, palettes.Length, "Expected one color palette asset.");
            var palette = AssetDatabase.LoadAssetAtPath<ColorPalette>(AssetDatabase.GUIDToAssetPath(palettes[0]));

            var failures = new StringBuilder();
            foreach (var asset in LevelAssets())
            {
                var path = AssetDatabase.GetAssetPath(asset);
                foreach (var block in asset.Level.Blocks)
                    CheckColor(palette, block.ColorId, $"{path}: block", failures);
                foreach (var door in asset.Level.Doors)
                    CheckColor(palette, door.ColorId, $"{path}: door", failures);
            }

            Assert.IsEmpty(failures.ToString());
        }

        private static LevelAsset[] LevelAssets()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(LevelAsset)}");
            var levels = new LevelAsset[guids.Length];
            for (var i = 0; i < guids.Length; i++)
                levels[i] = AssetDatabase.LoadAssetAtPath<LevelAsset>(AssetDatabase.GUIDToAssetPath(guids[i]));
            return levels;
        }

        private static void CheckColor(ColorPalette palette, int colorId, string owner, StringBuilder failures)
        {
            if (!palette.Has(colorId))
                failures.AppendLine($"{owner} color {colorId} has no material in the palette");
        }
    }
}
