using Game.Play;
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

            _view.Follow(Vector3.right);

            Assert.IsTrue(_view.enabled);
        }

        [Test]
        public void SetPose_WhileFollowing_PutsTheBlockThereAndStops()
        {
            _view.Follow(Vector3.right);

            _view.SetPose(Vector3.forward);

            Assert.AreEqual(Vector3.forward, _view.transform.localPosition);
            Assert.IsFalse(_view.enabled);
        }
    }
}
