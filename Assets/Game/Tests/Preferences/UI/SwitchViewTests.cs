using Game.Preferences;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Tests.Preferences
{
    public sealed class SwitchViewTests
    {
        private SwitchView _view;
        private SwitchSettings _settings;
        private Image _track;
        private RectTransform _knob;
        private Image _onFace;
        private Image _offFace;

        [SetUp]
        public void SetUp()
        {
            _track = new GameObject("Switch", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            _track.rectTransform.sizeDelta = new Vector2(210f, 76f);
            _knob = (RectTransform)new GameObject("Knob", typeof(RectTransform)).transform;
            _knob.SetParent(_track.transform, false);
            _onFace = new GameObject("OnFace", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            _offFace = new GameObject("OffFace", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            _onFace.transform.SetParent(_knob, false);
            _offFace.transform.SetParent(_knob, false);

            _view = _track.gameObject.AddComponent<SwitchView>();
            _settings = ScriptableObject.CreateInstance<SwitchSettings>();
            _view.Construct(_settings);
            var serialized = new SerializedObject(_view);
            serialized.FindProperty("_track").objectReferenceValue = _track;
            serialized.FindProperty("_knob").objectReferenceValue = _knob;
            serialized.FindProperty("_onFace").objectReferenceValue = _onFace;
            serialized.FindProperty("_offFace").objectReferenceValue = _offFace;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_track.gameObject);
            Object.DestroyImmediate(_settings);
        }

        [Test]
        public void Show_On_PutsTheOnKnobOverTheRightEndOfAnOnTrack()
        {
            _view.Show(true);

            Assert.AreEqual((210f - 76f) * 0.5f, _knob.anchoredPosition.x, 0.001f);
            Assert.AreEqual(1f, _onFace.color.a);
            Assert.AreEqual(0f, _offFace.color.a);
            Assert.AreEqual(_settings.OnTrack, _track.color);
        }

        [Test]
        public void Show_Off_PutsTheOffKnobOverTheLeftEndOfAnOffTrack()
        {
            _view.Show(true);

            _view.Show(false);

            Assert.AreEqual(-(210f - 76f) * 0.5f, _knob.anchoredPosition.x, 0.001f);
            Assert.AreEqual(0f, _onFace.color.a);
            Assert.AreEqual(1f, _offFace.color.a);
            Assert.AreEqual(_settings.OffTrack, _track.color);
        }
    }
}
