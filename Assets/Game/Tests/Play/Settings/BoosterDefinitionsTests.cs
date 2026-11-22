using System.Collections.Generic;
using System.Linq;
using Game.Play;
using NUnit.Framework;
using UnityEditor;

namespace Game.Tests.Play
{
    public sealed class BoosterDefinitionsTests
    {
        private const string HudPath = "Assets/Game/Runtime/Play/Prefabs/UI/LevelHud.prefab";

        [Test]
        public void EveryBooster_HasADistinctIdAndWhatItsSlotAndPopupShow()
        {
            var definitions = Definitions();
            Assert.IsNotEmpty(definitions);

            foreach (var definition in definitions)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(definition.Id), definition.name);
                Assert.IsNotNull(definition.Icon, definition.name);
                Assert.IsFalse(string.IsNullOrWhiteSpace(definition.Title), definition.name);
                Assert.IsFalse(string.IsNullOrWhiteSpace(definition.Description), definition.name);
            }
            CollectionAssert.AllItemsAreUnique(definitions.Select(definition => definition.Id));
        }

        [Test]
        public void TheHud_HasASlotForEveryBooster()
        {
            var hud = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(HudPath);
            var bar = hud.GetComponentInChildren<BoosterBar>(true);
            var slots = new SerializedObject(bar).FindProperty("_slots");

            Assert.GreaterOrEqual(slots.arraySize, Definitions().Count);
            for (var i = 0; i < slots.arraySize; i++)
                Assert.IsNotNull(slots.GetArrayElementAtIndex(i).objectReferenceValue);
        }

        private static List<BoosterDefinition> Definitions() =>
            AssetDatabase.FindAssets($"t:{nameof(BoosterDefinition)}", new[] { "Assets/Game" })
                .Select(guid => AssetDatabase.LoadAssetAtPath<BoosterDefinition>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();
    }
}
