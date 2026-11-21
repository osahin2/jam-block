using Game.Data.Levels;
using Game.Play;
using Game.Tests.Levels;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Play
{
    public sealed class BlockViewTests
    {
        private BlockView _view;
        private Outline _outline;
        private BlockMotionSettings _motion;

        [SetUp]
        public void SetUp()
        {
            _view = new GameObject("Block").AddComponent<BlockView>();
            _outline = _view.gameObject.AddComponent<Outline>();
            var view = new SerializedObject(_view);
            view.FindProperty("_outline").objectReferenceValue = _outline;
            view.ApplyModifiedPropertiesWithoutUndo();
            _motion = ScriptableObject.CreateInstance<BlockMotionSettings>();
            _view.Init(_motion, 1);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_view.gameObject);
            Object.DestroyImmediate(_motion);
        }

        [Test]
        public void Hold_ShowsTheOutline_SnapToOnReleaseHidesIt()
        {
            _view.Hold();
            Assert.IsTrue(_outline.IsShown);

            _view.SnapTo(Vector3.right);

            Assert.IsFalse(_outline.IsShown);
        }

        [Test]
        public void Leave_HidesTheOutline()
        {
            _view.Hold();

            _view.Leave(Vector3.zero, Vector3.forward, 0.3f);

            Assert.IsFalse(_outline.IsShown);
        }

        [Test]
        public void Leave_WhileTrailingItsTarget_SnapsFromWhereTheBlockIsDrawn()
        {
            _view.SetPose(Vector3.left);
            _view.Follow(Vector3.zero);

            _view.Leave(Vector3.zero, Vector3.forward, 0.3f);

            Assert.AreEqual(Vector3.left, _view.transform.localPosition, "the snap tweens there, it does not jump");
            Assert.IsFalse(_view.enabled, "the snap replaces the drag's follow");
            Assert.IsTrue(_view.IsLeaving);
        }

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

        [Test]
        public void CatchUp_WithABlockStillOnTheBoard_KeepsShowingIt()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(1, 0, 0, 1, 1).Build());

            _view.CatchUp(board.Blocks[0]);

            Assert.IsTrue(_view.gameObject.activeSelf);
        }

        [Test]
        public void CatchUp_WithABlockGoneWithoutLeaving_StopsShowingIt()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(1, 0, 0, 1, 1).Build());
            board.Hit(0);

            _view.CatchUp(board.Blocks[0]);

            Assert.IsFalse(_view.gameObject.activeSelf);
        }

        [Test]
        public void CatchUp_WhileTheBlockLeavesThroughItsDoor_LetsItSlideOut()
        {
            var board = Boards.Of(new LevelBuilder(3, 3).Rect(1, 1, 0, 1, 1).Door(1, Side.Top, 1).Build());
            board.TryLeave(0, out _);
            _view.Leave(Vector3.zero, Vector3.forward, 0.3f);

            _view.CatchUp(board.Blocks[0]);

            Assert.IsTrue(_view.gameObject.activeSelf);
            Assert.IsTrue(_view.IsLeaving);
        }
    }
}
