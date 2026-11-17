using Game.Play.Presentation;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Play
{
    public sealed class BlockViewTests
    {
        private BlockView _view;

        [SetUp]
        public void SetUp() => _view = new GameObject("Block").AddComponent<BlockView>();

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_view.gameObject);

        [Test]
        public void Follow_AfterTheViewStopped_StartsItAgain()
        {
            _view.enabled = false;

            _view.Follow(Vector3.right, 0.05f);

            Assert.IsTrue(_view.enabled);
        }
    }
}
