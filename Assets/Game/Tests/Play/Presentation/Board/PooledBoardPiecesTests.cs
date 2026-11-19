using Game.Data.Levels;
using Game.Play;
using Game.Tests.Levels;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Play
{
    // Uses the project's settings and prefabs, so the game's real pieces are pooled.
    public sealed class PooledBoardPiecesTests
    {
        private const int Red = 1;

        private BoardRoot _board;
        private Transform _idle;
        private Camera _camera;
        private PooledBoardPieces _pieces;
        private LevelStage _stage;

        [SetUp]
        public void SetUp()
        {
            var art = Load<BoardArtSettings>();
            _board = new GameObject("Board").AddComponent<BoardRoot>();
            _idle = new GameObject("Idle Board Pieces").transform;
            var board = new SerializedObject(_board);
            board.FindProperty("_idlePieces").objectReferenceValue = _idle;
            board.ApplyModifiedPropertiesWithoutUndo();
            _camera = new GameObject("Camera").AddComponent<Camera>();
            _pieces = new PooledBoardPieces(_board);
            _stage = new LevelStage(art, Load<ColorPalette>(), Load<BlockMotionSettings>(), Load<CameraFramingSettings>());
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_board.gameObject);
            Object.DestroyImmediate(_idle.gameObject);
            Object.DestroyImmediate(_camera.gameObject);
        }

        [Test]
        public void Constructor_MakesNoPieces()
        {
            Assert.AreEqual(0, _idle.childCount);
        }

        [Test]
        public void ReleaseAll_GivesEveryPieceOfTheBoardBack_Inactive()
        {
            Show(TwoBlocks());
            var made = CountAll();

            _pieces.ReleaseAll();

            Assert.AreEqual(0, _board.transform.childCount);
            Assert.AreEqual(made, _idle.childCount);
            foreach (Transform piece in _idle)
                Assert.IsFalse(piece.gameObject.activeSelf, piece.name);
        }

        [Test]
        public void Show_TheSameLevelAgain_ReusesEveryPiece()
        {
            Show(TwoBlocks());
            var made = CountAll();
            _pieces.ReleaseAll();

            Show(TwoBlocks());

            Assert.AreEqual(made, CountAll(), "nothing new was made");
            Assert.AreEqual(0, _idle.childCount, "every idle piece was taken again");
        }

        [Test]
        public void Show_AfterReleaseAll_KeepsNothingOfTheLevelBefore()
        {
            Show(TwoBlocks());
            var made = CountAll();
            _pieces.ReleaseAll();

            var staged = Show(new LevelBuilder(3, 3).Rect(Red, 1, 1, 1, 1).Build());

            var blocks = _board.GetComponentsInChildren<BlockView>(true);
            Assert.AreEqual(1, blocks.Length);
            Assert.AreSame(staged.Blocks[0], blocks[0]);
            Assert.AreEqual(staged.Layout.CellCenter(new Cell(1, 1)), blocks[0].transform.localPosition);
            // One cell of one block is four quarter pieces; 9 ground tiles, 2 wall pieces per edge cell and 4 corners.
            Assert.AreEqual(4, blocks[0].transform.childCount);
            Assert.AreEqual(1 + 9 + 2 * 12 + 4, _board.transform.childCount);
            foreach (var piece in _board.GetComponentsInChildren<Transform>(true))
                Assert.IsTrue(piece.gameObject.activeSelf, piece.name);
            Assert.AreEqual(made, CountAll(), "a smaller level is built from the pieces of the larger one");
        }

        private StagedLevel Show(LevelData level) => _stage.Show(level, _board.transform, _pieces, _camera);

        private static LevelData TwoBlocks() =>
            new LevelBuilder(4, 5).Rect(Red, 0, 0, 2, 2).Rect(Red, 1, 3, 3, 1).Door(Red, Side.Top, 0, 2).Build();

        // Every instance the pools made, on the board or idle.
        private int CountAll() =>
            _idle.childCount + _board.GetComponentsInChildren<BoardPiece>(true).Length +
            _board.GetComponentsInChildren<BlockView>(true).Length;

        private static T Load<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { "Assets/Game" });
            Assert.AreEqual(1, guids.Length, $"one {typeof(T).Name} asset");
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
