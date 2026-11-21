using System.Text;
using Game.Buttons;
using Game.Home;
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

        [Test]
        public void EveryTabInTheGamesPrefabs_TakesTouchesAndAnswersThem()
        {
            var problems = new StringBuilder();
            var tabs = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Game/Runtime" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                foreach (var toggle in prefab.GetComponentsInChildren<Toggle>(true))
                {
                    tabs++;
                    if (!TakesTouches(toggle))
                        problems.AppendLine($"{path}: {toggle.name} has no graphic that is a raycast target");
                    if (toggle.GetComponent<TabView>() == null)
                        problems.AppendLine($"{path}: {toggle.name} has no TabView");
                }

                foreach (var locked in prefab.GetComponentsInChildren<LockedTabView>(true))
                {
                    tabs++;
                    if (!TakesTouches(locked))
                        problems.AppendLine($"{path}: {locked.name} has no graphic that is a raycast target");
                }
            }

            Assert.Greater(tabs, 0);
            Assert.IsEmpty(problems.ToString());
        }

        private static bool TakesTouches(Component target)
        {
            foreach (var graphic in target.GetComponentsInChildren<Graphic>(true))
            {
                if (graphic.raycastTarget)
                    return true;
            }

            return false;
        }
    }
}
