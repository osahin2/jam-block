using System.Text;
using Game.Buttons;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Tests.Buttons
{
    public sealed class ScreenButtonsTests
    {
        [Test]
        public void EveryButtonInTheGamesPrefabs_TakesTouchesAndAnswersThem()
        {
            var problems = new StringBuilder();
            var buttons = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Game/Runtime" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                foreach (var button in prefab.GetComponentsInChildren<Button>(true))
                {
                    buttons++;
                    if (!TakesTouches(button))
                        problems.AppendLine($"{path}: {button.name} has no graphic that is a raycast target");
                    if (button.GetComponent<PressFeedback>() == null)
                        problems.AppendLine($"{path}: {button.name} has no PressFeedback");
                }
            }

            Assert.Greater(buttons, 0);
            Assert.IsEmpty(problems.ToString());
        }

        private static bool TakesTouches(Button button)
        {
            foreach (var graphic in button.GetComponentsInChildren<Graphic>(true))
            {
                if (graphic.raycastTarget)
                    return true;
            }

            return false;
        }
    }
}
