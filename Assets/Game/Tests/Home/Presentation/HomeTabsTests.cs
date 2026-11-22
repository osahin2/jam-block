using System.Collections.Generic;
using System.Text;
using Game.Home;
using Game.Tests.Buttons;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Tests.Home
{
    public sealed class HomeTabsTests
    {
        private const string ScreenPath = "Assets/Game/Runtime/Home/Prefabs/UI/HomeScreen.prefab";

        [Test]
        public void EveryTab_TakesTouches()
        {
            var screen = LoadScreen();
            var problems = new StringBuilder();
            foreach (var tab in screen.GetComponentsInChildren<TabView>(true))
            {
                if (!TouchTargets.TakesTouches(tab))
                    problems.AppendLine($"{tab.name} has no graphic that is a raycast target");
            }

            foreach (var locked in screen.GetComponentsInChildren<LockedTabView>(true))
            {
                if (!TouchTargets.TakesTouches(locked))
                    problems.AppendLine($"{locked.name} has no graphic that is a raycast target");
            }

            Assert.IsEmpty(problems.ToString());
        }

        [Test]
        public void EverySwitchingTab_TurnsItsOwnPanelOnAndOff_AndOpensWithItsPanelShown()
        {
            var tabs = LoadScreen().GetComponentsInChildren<TabView>(true);
            var problems = new StringBuilder();
            var panels = new HashSet<GameObject>();
            var on = 0;
            foreach (var tab in tabs)
            {
                var toggle = tab.GetComponent<Toggle>();
                if (toggle.isOn)
                    on++;
                var events = toggle.onValueChanged;
                if (events.GetPersistentEventCount() != 1 || events.GetPersistentMethodName(0) != nameof(GameObject.SetActive)
                    || events.GetPersistentTarget(0) is not GameObject panel)
                {
                    problems.AppendLine($"{tab.name} does not set exactly one panel active");
                    continue;
                }

                if (!panels.Add(panel))
                    problems.AppendLine($"{tab.name} shares its panel {panel.name} with another tab");
                if (panel.activeSelf != toggle.isOn)
                    problems.AppendLine($"{tab.name} opens {(toggle.isOn ? "on" : "off")} but its panel {panel.name} does not");
            }

            Assert.Greater(tabs.Length, 1);
            Assert.AreEqual(1, on, "exactly one tab opens selected");
            Assert.IsEmpty(problems.ToString());
        }

        private static GameObject LoadScreen() => AssetDatabase.LoadAssetAtPath<GameObject>(ScreenPath);
    }
}
