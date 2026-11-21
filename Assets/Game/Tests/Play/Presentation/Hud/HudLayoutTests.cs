using Game.Play;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Tests.Play
{
    public sealed class HudLayoutTests
    {
        private RectTransform _screen;
        private CameraFramingSettings _framing;

        [SetUp]
        public void SetUp()
        {
            _framing = AssetDatabase.LoadAssetAtPath<CameraFramingSettings>(
                "Assets/Game/Runtime/Play/Settings/CameraFramingSettings.asset");
            _screen = new GameObject("Screen", typeof(RectTransform)).GetComponent<RectTransform>();
            _screen.sizeDelta = _framing.UiReferenceResolution;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_screen.gameObject);
        }

        // The framing's margins are copied from the HUD's layout, not read from it, so this keeps the two in step.
        [Test]
        public void HudAndBoosterSlots_StayWithinTheFramingMargins()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Game/Runtime/Play/Prefabs/UI/LevelHud.prefab");
            var hud = (RectTransform)Object.Instantiate(prefab, _screen).transform;
            hud.anchorMin = Vector2.zero;
            hud.anchorMax = Vector2.one;
            hud.offsetMin = Vector2.zero;
            hud.offsetMax = Vector2.zero;
            var content = (RectTransform)hud.Find("Content");
            var area = content.rect;

            foreach (RectTransform element in content)
            {
                var drawn = Drawn(content, element);
                if (element.anchorMin.y >= 0.5f)
                    Assert.That(area.yMax - drawn.yMin, Is.LessThanOrEqualTo(_framing.TopMargin), element.name);
                else
                    Assert.That(drawn.yMax - area.yMin, Is.LessThanOrEqualTo(_framing.BottomMargin), element.name);
            }
        }

        // Only what is drawn counts: a group's own rect may be larger than the pictures in it.
        private static Rect Drawn(RectTransform content, RectTransform element)
        {
            var corners = new Vector3[4];
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            foreach (var graphic in element.GetComponentsInChildren<Graphic>(true))
            {
                graphic.rectTransform.GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    Vector2 local = content.InverseTransformPoint(corner);
                    min = Vector2.Min(min, local);
                    max = Vector2.Max(max, local);
                }
            }
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}
