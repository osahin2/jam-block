using System.Text;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Bootstrap
{
    public sealed class PrefabReferencesTests
    {
        [Test]
        public void EveryGameComponentInTheGamesPrefabs_HasItsReferencesAssigned()
        {
            var missing = new StringBuilder();
            var components = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Game/Runtime" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                foreach (var component in AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (component == null || component.GetType().Namespace?.StartsWith("Game") != true)
                        continue;
                    components++;
                    var property = new SerializedObject(component).GetIterator();
                    while (property.NextVisible(true))
                    {
                        if (property.propertyType == SerializedPropertyType.ObjectReference && property.name != "m_Script" &&
                            property.objectReferenceValue == null)
                            missing.AppendLine($"{path}: {component.name}.{component.GetType().Name}.{property.propertyPath} is not assigned");
                    }
                }
            }

            Assert.Greater(components, 0);
            Assert.IsEmpty(missing.ToString());
        }
    }
}
