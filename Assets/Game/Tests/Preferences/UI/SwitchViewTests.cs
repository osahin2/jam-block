using Game.Preferences;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Preferences
{
    public sealed class SwitchViewTests
    {
        private SwitchView _view;
        private GameObject _on;
        private GameObject _off;

        [SetUp]
        public void SetUp()
        {
            _view = new GameObject("Switch").AddComponent<SwitchView>();
            _on = new GameObject("On");
            _off = new GameObject("Off");
            var serialized = new SerializedObject(_view);
            serialized.FindProperty("_on").objectReferenceValue = _on;
            serialized.FindProperty("_off").objectReferenceValue = _off;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_view.gameObject);
            Object.DestroyImmediate(_on);
            Object.DestroyImmediate(_off);
        }

        [Test]
        public void Show_On_ShowsOnlyTheOnKnob()
        {
            _view.Show(true);

            Assert.IsTrue(_on.activeSelf);
            Assert.IsFalse(_off.activeSelf);
        }

        [Test]
        public void Show_Off_ShowsOnlyTheOffKnob()
        {
            _view.Show(false);

            Assert.IsFalse(_on.activeSelf);
            Assert.IsTrue(_off.activeSelf);
        }
    }
}
