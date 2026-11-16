using System.Text;
using Game.Levels;
using Game.Logic.Levels;
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
            foreach (var guid in AssetDatabase.FindAssets($"t:{nameof(LevelAsset)}"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var report = LevelValidator.Validate(AssetDatabase.LoadAssetAtPath<LevelAsset>(path).Level);
                if (report.HasErrors)
                    failures.AppendLine($"{path}: {report}");
            }

            Assert.IsEmpty(failures.ToString());
        }
    }
}
