using System.Collections.Generic;
using System.Text;
using Core.Installers;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Bootstrap
{
    public sealed class InstallerAssetTests
    {
        [Test]
        public void EveryInstallerAndItsSettings_HaveNoMissingReferences()
        {
            var missing = new StringBuilder();
            foreach (var guid in AssetDatabase.FindAssets($"t:{nameof(ScriptableObjectInstaller)}", new[] { "Assets/Game" }))
            {
                var installer = AssetDatabase.LoadAssetAtPath<ScriptableObjectInstaller>(AssetDatabase.GUIDToAssetPath(guid));
                foreach (var referenced in CollectMissing(installer, missing))
                {
                    if (referenced is ScriptableObject settings)
                        CollectMissing(settings, missing);
                }
            }

            Assert.IsEmpty(missing.ToString());
        }

        private static List<Object> CollectMissing(Object asset, StringBuilder missing)
        {
            var referenced = new List<Object>();
            var property = new SerializedObject(asset).GetIterator();
            while (property.NextVisible(true))
            {
                if (property.propertyType != SerializedPropertyType.ObjectReference || property.name == "m_Script")
                    continue;
                if (property.objectReferenceValue == null)
                    missing.AppendLine($"{asset.name}.{property.propertyPath} is not assigned");
                else
                    referenced.Add(property.objectReferenceValue);
            }
            return referenced;
        }
    }
}
