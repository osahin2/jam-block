using DG.Tweening;
using Game.Data.Levels;
using Game.Editor.LevelEditor;
using Game.Logic.Play;
using Game.Play;
using Game.Tests.Levels;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Play
{
    public sealed class DoorViewTests
    {
        private const int Red = 1;
        private const float Reach = 0.1f;
        private const float Gone = 0.3f;

        private BoardArtSettings _art;
        private BlockMotionSettings _motion;
        private Transform _board;
        private DoorView _door;
        private Transform _model;
        private Transform _arrow;

        [SetUp]
        public void SetUp()
        {
            _art = Load<BoardArtSettings>();
            _motion = Load<BlockMotionSettings>();
            var palette = Load<ColorPalette>();
            var builder = new BoardBuilder(_art, palette, _motion, BlockViewBuilders.WithTheGamesFeatures());
            var level = new LevelBuilder(4, 5).Rect(Red, 1, 0, 2, 1).Door(Red, Side.Top, 1, 2).Build();
            _board = new GameObject("Board").transform;
            _door = builder.Build(level, new LevelAttempt(level), new BoardLayout(4, 5, _art), _board,
                new PreviewBoardPieces()).Doors[0];
            _model = _door.transform.Find("Model");
            foreach (var piece in _board.GetComponentsInChildren<BoardPiece>())
            {
                if (piece.name == _art.DoorArrow.name + "(Clone)")
                    _arrow = piece.transform;
            }
        }

        [TearDown]
        public void TearDown()
        {
            DOTween.KillAll();
            Object.DestroyImmediate(_board.gameObject);
        }

        [Test]
        public void LetThrough_DropsTheDoorWithItsArrowAtOnce_AndRaisesThemAfterTheBlockIsGone()
        {
            var modelHeight = _model.position.y;
            var arrowHeight = _arrow.position.y;

            _door.LetThrough(Reach, Gone);
            GoTo(_motion.DoorDropDuration);

            Assert.AreEqual(modelHeight - _art.DoorDrop, _model.position.y, 1e-4f);
            Assert.AreEqual(arrowHeight - _art.DoorDrop, _arrow.position.y, 1e-4f);

            GoTo(Gone + _motion.DoorRiseDelay - 0.01f);
            Assert.AreEqual(modelHeight - _art.DoorDrop, _model.position.y, 1e-4f, "still down just before it rises");

            GoTo(Gone + _motion.DoorRiseDelay + _motion.DoorRiseDuration);
            Assert.AreEqual(modelHeight, _model.position.y, 1e-4f);
            Assert.AreEqual(arrowHeight, _arrow.position.y, 1e-4f);
        }

        private static void GoTo(float time)
        {
            foreach (var tween in DOTween.PlayingTweens())
                tween.Goto(time, true);
        }

        private static T Load<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { "Assets/Game" });
            Assert.AreEqual(1, guids.Length, $"one {typeof(T).Name} asset");
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
