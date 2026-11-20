using Game.Play;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Play
{
    public sealed class CameraFramingTests
    {
        private const float Tolerance = 0.002f;

        private BoardArtSettings _art;
        private CameraFramingSettings _settings;
        private Camera _camera;
        private Transform _board;

        [SetUp]
        public void SetUp()
        {
            _art = ScriptableObject.CreateInstance<BoardArtSettings>();
            _settings = ScriptableObject.CreateInstance<CameraFramingSettings>();
            _camera = new GameObject("Camera").AddComponent<Camera>();
            _camera.fieldOfView = 45f;
            _camera.transform.SetPositionAndRotation(new Vector3(0f, 25f, -9f), Quaternion.Euler(71.4f, 0f, 0f));
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

        // 1080 x 1920 and 20:9 (1080 x 2400), the smallest and largest authored boards and a wide one.
        [TestCase(1080, 1920, 5, 6)]
        [TestCase(1080, 1920, 4, 5)]
        [TestCase(1080, 2400, 5, 6)]
        [TestCase(1080, 2400, 4, 5)]
        [TestCase(1080, 1920, 8, 3)]
        public void Frame_FitsTheBoardInsideTheMargins_AndFillsThem(int screenWidth, int screenHeight, int width, int height)
        {
            _camera.aspect = (float)screenWidth / screenHeight;
            var layout = new BoardLayout(width, height, _art);

            new CameraFraming(_settings).Frame(_camera, layout, _board);

            var side = _settings.SideMargin;
            var topAndBottom = _settings.TopAndBottomMargin;
            var room = float.MaxValue;
            foreach (var corner in Corners(layout))
            {
                var onScreen = _camera.WorldToViewportPoint(corner);
                Assert.Greater(onScreen.z, 0f, "in front of the camera");
                Assert.That(onScreen.x, Is.InRange(side - Tolerance, 1f - side + Tolerance), $"x of {corner}");
                Assert.That(onScreen.y, Is.InRange(topAndBottom - Tolerance, 1f - topAndBottom + Tolerance), $"y of {corner}");
                room = Mathf.Min(room, onScreen.x - side, 1f - side - onScreen.x, onScreen.y - topAndBottom,
                    1f - topAndBottom - onScreen.y);
            }
            Assert.AreEqual(0f, room, Tolerance, "the board reaches the margins on at least one side");
        }

        [Test]
        public void Frame_KeepsTheCameraTurned_AndTheBoardCenteredWhereverTheBoardIs()
        {
            _camera.aspect = 1080f / 1920f;
            var rotation = _camera.transform.rotation;
            _board.position = new Vector3(3f, 1f, -2f);

            new CameraFraming(_settings).Frame(_camera, new BoardLayout(5, 6, _art), _board);

            Assert.AreEqual(rotation, _camera.transform.rotation);
            var center = _camera.WorldToViewportPoint(_board.position);
            Assert.AreEqual(0.5f, center.x, Tolerance);
            Assert.AreEqual(0.5f, center.y, Tolerance);
        }

        [Test]
        public void Frame_ASmallerBoard_BringsTheCameraCloser()
        {
            _camera.aspect = 1080f / 1920f;
            var framing = new CameraFraming(_settings);

            framing.Frame(_camera, new BoardLayout(5, 6, _art), _board);
            var large = Vector3.Distance(_camera.transform.position, _board.position);
            framing.Frame(_camera, new BoardLayout(4, 5, _art), _board);
            var small = Vector3.Distance(_camera.transform.position, _board.position);

            Assert.Less(small, large);
        }

        private Vector3[] Corners(BoardLayout layout)
        {
            var halfWidth = layout.Right + _art.FrameWidth;
            var halfDepth = layout.Top + _art.FrameWidth;
            var corners = new Vector3[8];
            var i = 0;
            for (var x = -1; x <= 1; x += 2)
            {
                for (var z = -1; z <= 1; z += 2)
                {
                    corners[i++] = _board.TransformPoint(new Vector3(x * halfWidth, 0f, z * halfDepth));
                    corners[i++] = _board.TransformPoint(new Vector3(x * halfWidth, layout.Box.max.y, z * halfDepth));
                }
            }
            return corners;
        }
    }
}
