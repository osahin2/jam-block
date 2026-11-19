using Game.Data.Levels;
using Game.Play;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Play
{
    public sealed class TouchPlaneTests
    {
        // In cells.
        private const float Tolerance = 0.001f;

        private BoardArtSettings _art;
        private Camera _camera;
        private BoardRoot _board;
        private BoardLayout _layout;
        private TouchPlane _plane;

        [SetUp]
        public void SetUp()
        {
            _art = ScriptableObject.CreateInstance<BoardArtSettings>();
            _camera = new GameObject("Camera").AddComponent<Camera>();
            _camera.fieldOfView = 45f;
            _camera.aspect = 1080f / 1920f;
            _camera.transform.SetPositionAndRotation(new Vector3(0f, 25f, -9f), Quaternion.Euler(71.4f, 0f, 0f));
            _board = new GameObject("Board").AddComponent<BoardRoot>();
            _layout = new BoardLayout(5, 6, _art);
            _plane = new TouchPlane(_camera, _board, _art);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_camera.gameObject);
            Object.DestroyImmediate(_board.gameObject);
            Object.DestroyImmediate(_art);
        }

        [TestCase(0, 0)]
        [TestCase(4, 5)]
        [TestCase(2, 3)]
        public void TryProject_TheTopOfACellOnScreen_GivesTheCellsCenterOnTheBoard(int x, int y)
        {
            AssertProjectsOntoCellCenter(x, y);
        }

        [Test]
        public void TryProject_FollowsTheBoardRootsTransform()
        {
            _board.transform.SetPositionAndRotation(new Vector3(1.5f, 0.5f, -2f), Quaternion.Euler(0f, 30f, 0f));
            _board.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            AssertProjectsOntoCellCenter(0, 0);
            AssertProjectsOntoCellCenter(4, 5);
        }

        [Test]
        public void TryProject_APointBesideTheBoard_StillProjects()
        {
            var beside = new Vector3(_layout.Right + 3f * _art.CellSize, _art.TouchHeight, 0f);
            var screen = _camera.WorldToScreenPoint(_board.transform.TransformPoint(beside));

            Assert.IsTrue(_plane.TryProject(screen, out var local));
            Assert.Greater(_layout.LocalToBoard(local).x, 5f);
        }

        [Test]
        public void TryProject_ARayAwayFromThePlane_Misses()
        {
            _camera.transform.rotation = Quaternion.Euler(-10f, 0f, 0f);
            var center = new Vector2(_camera.pixelWidth * 0.5f, _camera.pixelHeight * 0.5f);

            Assert.IsFalse(_plane.TryProject(center, out _));
        }

        // The top of the cell's blocks, seen on screen and projected back, lands on the cell's center.
        private void AssertProjectsOntoCellCenter(int x, int y)
        {
            var top = _layout.CellCenter(new Cell(x, y)) + new Vector3(0f, _art.TouchHeight, 0f);
            var screen = _camera.WorldToScreenPoint(_board.transform.TransformPoint(top));

            Assert.IsTrue(_plane.TryProject(screen, out var local));
            var onBoard = _layout.LocalToBoard(local);
            Assert.AreEqual(x + 0.5f, onBoard.x, Tolerance, "x");
            Assert.AreEqual(y + 0.5f, onBoard.y, Tolerance, "y");
        }
    }
}
