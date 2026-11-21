using System.Text;
using Game.Buttons;
using Game.Popups;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Popups
{
    public sealed class PopupEntrancePartsTests
    {
        [Test]
        public void NoEntrancePart_IsAButtonWithPressFeedback()
        {
            var problems = new StringBuilder();
            var entrances = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Game/Runtime" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var entrance = AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponent<PopupEntrance>();
                if (entrance == null)
                    continue;
                entrances++;
                var parts = new SerializedObject(entrance).FindProperty("_parts");
                for (var i = 0; i < parts.arraySize; i++)
                {
                    var part = (RectTransform)parts.GetArrayElementAtIndex(i).objectReferenceValue;
                    if (part.GetComponent<PressFeedback>() != null)
                        problems.AppendLine($"{path}: part {part.name} has a PressFeedback");
                }
            }

            Assert.Greater(entrances, 0);
            Assert.IsEmpty(problems.ToString());
        }
    }
}
