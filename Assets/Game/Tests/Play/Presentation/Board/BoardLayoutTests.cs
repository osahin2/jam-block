using Game.Play;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Play
{
    public sealed class BoardLayoutTests
    {
        private BoardArtSettings _art;

        [SetUp]
        public void SetUp() => _art = ScriptableObject.CreateInstance<BoardArtSettings>();

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_art);

        [Test]
        public void Box_CoversTheCellsAndTheFrame_FromTheGroundToTheBlocksTops()
        {
            var halfWidth = 5 * _art.CellSize * 0.5f + _art.FrameWidth;
            var halfDepth = 6 * _art.CellSize * 0.5f + _art.FrameWidth;

            var box = new BoardLayout(5, 6, _art).Box;

            Assert.AreEqual(new Vector3(-halfWidth, 0f, -halfDepth), box.min);
            Assert.AreEqual(new Vector3(halfWidth, _art.TouchHeight, halfDepth), box.max);
        }

        [Test]
        public void Box_ReachesTheFrameTopAndTheDoorsOuterFace_WhenTheFrameIsTallerAndTheDoorsThicker()
        {
            Set("_blockLift", -1f);
            Set("_frameLift", 1f);
            Set("_wallThickness", 0.6f);
            Set("_doorThickness", 0.8f);
            var halfWidth = 5 * _art.CellSize * 0.5f + _art.DoorWidth;

            var box = new BoardLayout(5, 6, _art).Box;

            Assert.Greater(_art.FrameTop, _art.TouchHeight);
            Assert.AreEqual(_art.FrameTop, box.max.y, 1e-5f);
            Assert.AreEqual(halfWidth, box.max.x, 1e-5f);
        }

        private void Set(string field, float value)
        {
            var serialized = new SerializedObject(_art);
            serialized.FindProperty(field).floatValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
