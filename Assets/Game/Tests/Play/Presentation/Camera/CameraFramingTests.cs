using Game.Play;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Tests.Play
{
    public sealed class CameraFramingTests
    {
        private const float Tolerance = 0.002f;

        private static readonly Rect FullScreen = new Rect(0f, 0f, 1f, 1f);
        private static readonly Rect Notched = new Rect(0f, 0.015f, 1f, 0.94f);

        private BoardArtSettings _art;
        private CameraFramingSettings _settings;
        private CameraFraming _framing;
        private Camera _camera;
        private Transform _board;

        [SetUp]
        public void SetUp()
        {
            _art = ScriptableObject.CreateInstance<BoardArtSettings>();
            _settings = ScriptableObject.CreateInstance<CameraFramingSettings>();
            _framing = new CameraFraming(_settings);
            _camera = new GameObject("Camera").AddComponent<Camera>();
            _camera.fieldOfView = 63f;
            _camera.transform.SetPositionAndRotation(new Vector3(0f, 25f, -9f), Quaternion.Euler(76f, 0f, 0f));
            _board = new GameObject("Board").transform;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_camera.gameObject);
            Object.DestroyImmediate(_board.gameObject);
            Object.DestroyImmediate(_settings);
            Object.DestroyImmediate(_art);
        }

        [TestCase(1080, 1920, 5, 6, false)]
        [TestCase(1080, 1920, 4, 5, false)]
        [TestCase(1080, 2400, 5, 6, false)]
        [TestCase(1080, 2400, 4, 5, true)]
        [TestCase(1080, 1920, 8, 3, false)]
        [TestCase(1080, 1920, 4, 9, false)]
        [TestCase(1080, 2400, 4, 9, true)]
        [TestCase(1600, 2560, 5, 6, false)]
        [TestCase(1600, 2560, 4, 9, false)]
        [TestCase(1536, 2048, 5, 6, false)]
        [TestCase(1536, 2048, 8, 3, false)]
        public void Frame_FitsTheBoardInsideTheWindow_AndReachesItOnOneAxis(int screenWidth, int screenHeight, int width,
            int height, bool notched)
        {
            _camera.aspect = (float)screenWidth / screenHeight;
            var safeArea = notched ? Notched : FullScreen;
            var layout = new BoardLayout(width, height, _art);

            _framing.Frame(_camera, layout, _board, safeArea);

            var window = _framing.ViewportWindow(safeArea);
            var shown = OnScreen(layout);
            Assert.That(shown.xMin, Is.GreaterThanOrEqualTo(window.xMin - Tolerance), "left");
            Assert.That(shown.xMax, Is.LessThanOrEqualTo(window.xMax + Tolerance), "right");
            Assert.That(shown.yMin, Is.GreaterThanOrEqualTo(window.yMin - Tolerance), "bottom");
            Assert.That(shown.yMax, Is.LessThanOrEqualTo(window.yMax + Tolerance), "top");
            Assert.IsTrue(Reaches(shown.xMin, shown.xMax, window.xMin, window.xMax) ||
                Reaches(shown.yMin, shown.yMax, window.yMin, window.yMax), $"{shown} fills {window} on one axis");
        }

        [TestCase(1080, 1920, 5, 6)]
        [TestCase(1080, 2400, 5, 6)]
        [TestCase(1080, 1920, 4, 9)]
        [TestCase(1536, 2048, 5, 6)]
        public void Frame_CentersTheBoardAsSeenInTheWindow(int screenWidth, int screenHeight, int width, int height)
        {
            _camera.aspect = (float)screenWidth / screenHeight;
            var layout = new BoardLayout(width, height, _art);

            _framing.Frame(_camera, layout, _board, Notched);

            var window = _framing.ViewportWindow(Notched);
            var shown = OnScreen(layout);
            Assert.AreEqual(window.center.x, shown.center.x, Tolerance, "across");
            Assert.AreEqual(window.center.y, shown.center.y, Tolerance, "up");
        }

        [Test]
        public void Frame_KeepsTheCameraTurned_AndFitsTheBoardWhereverTheBoardIs()
        {
            _camera.aspect = 1080f / 1920f;
            var rotation = _camera.transform.rotation;
            _board.position = new Vector3(3f, 1f, -2f);
            var layout = new BoardLayout(5, 6, _art);

            _framing.Frame(_camera, layout, _board, FullScreen);

            Assert.AreEqual(rotation, _camera.transform.rotation);
            var window = _framing.ViewportWindow(FullScreen);
            var shown = OnScreen(layout);
            Assert.AreEqual(window.center.x, shown.center.x, Tolerance);
            Assert.That(shown.yMin, Is.GreaterThanOrEqualTo(window.yMin - Tolerance));
            Assert.That(shown.yMax, Is.LessThanOrEqualTo(window.yMax + Tolerance));
        }

        [Test]
        public void Frame_ASmallerBoard_BringsTheCameraCloser()
        {
            _camera.aspect = 1080f / 1920f;

            _framing.Frame(_camera, new BoardLayout(5, 6, _art), _board, FullScreen);
            var large = Vector3.Distance(_camera.transform.position, _board.position);
            _framing.Frame(_camera, new BoardLayout(4, 5, _art), _board, FullScreen);
            var small = Vector3.Distance(_camera.transform.position, _board.position);

            Assert.Less(small, large);
        }

        [Test]
        public void ViewportWindow_MeasuresTheMarginsInUIUnitsScaledByTheHeight_AndTheSidesInWidths()
        {
            var unit = 1f / 1920f;

            var window = _framing.ViewportWindow(Notched);

            Assert.AreEqual(_settings.SideMargin, window.xMin, Tolerance);
            Assert.AreEqual(1f - _settings.SideMargin, window.xMax, Tolerance);
            Assert.AreEqual(Notched.yMin + _settings.BottomMargin * unit, window.yMin, Tolerance);
            Assert.AreEqual(Notched.yMax - _settings.TopMargin * unit, window.yMax, Tolerance);
        }

        [Test]
        public void Settings_MeasureInTheUIRootsReferenceResolution_WhichMatchesTheScreensHeight()
        {
            var settings = AssetDatabase.LoadAssetAtPath<CameraFramingSettings>(
                "Assets/Game/Runtime/Play/Settings/CameraFramingSettings.asset");
            var scaler = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Game/Runtime/Bootstrap/Prefabs/GameUIRoot.prefab")
                .GetComponentInChildren<CanvasScaler>(true);

            Assert.AreEqual(CanvasScaler.ScaleMode.ScaleWithScreenSize, scaler.uiScaleMode);
            Assert.AreEqual(CanvasScaler.ScreenMatchMode.MatchWidthOrHeight, scaler.screenMatchMode);
            Assert.AreEqual(1f, scaler.matchWidthOrHeight);
            Assert.AreEqual(scaler.referenceResolution, settings.UiReferenceResolution);
        }

        [Test]
        public void Frame_InAWideView_FitsTheBoardInsideTheWindow()
        {
            _camera.aspect = 1920f / 1080f;
            var layout = new BoardLayout(5, 6, _art);

            _framing.Frame(_camera, layout, _board, FullScreen);

            var window = _framing.ViewportWindow(FullScreen);
            var shown = OnScreen(layout);
            Assert.That(shown.yMin, Is.GreaterThanOrEqualTo(window.yMin - Tolerance), "bottom");
            Assert.That(shown.yMax, Is.LessThanOrEqualTo(window.yMax + Tolerance), "top");
            Assert.AreEqual(0.5f, shown.center.x, Tolerance, "across");
        }

        private static bool Reaches(float min, float max, float windowMin, float windowMax) =>
            Mathf.Abs(min - windowMin) < Tolerance && Mathf.Abs(max - windowMax) < Tolerance;

        private Rect OnScreen(BoardLayout layout)
        {
            var box = layout.Box;
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            for (var i = 0; i < 8; i++)
            {
                var corner = _board.TransformPoint(new Vector3((i & 1) == 0 ? box.min.x : box.max.x,
                    (i & 2) == 0 ? box.min.y : box.max.y, (i & 4) == 0 ? box.min.z : box.max.z));
                var point = _camera.WorldToViewportPoint(corner);
                Assert.Greater(point.z, 0f, "in front of the camera");
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
            }
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}
