using Game.Play;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Play
{
    public sealed class BoardLayoutTests
    {
        [Test]
        public void Box_CoversTheCellsAndTheFrame_FromTheGroundToTheBlocksTops()
        {
            var art = ScriptableObject.CreateInstance<BoardArtSettings>();
            var halfWidth = 5 * art.CellSize * 0.5f + art.FrameWidth;
            var halfDepth = 6 * art.CellSize * 0.5f + art.FrameWidth;

            var box = new BoardLayout(5, 6, art).Box;

            Assert.AreEqual(new Vector3(-halfWidth, 0f, -halfDepth), box.min);
            Assert.AreEqual(new Vector3(halfWidth, art.TouchHeight, halfDepth), box.max);
            Object.DestroyImmediate(art);
        }
    }
}
