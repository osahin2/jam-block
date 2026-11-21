using Game.Data.Levels;
using Game.Logic.Play;
using Game.Play;
using Game.Tests.Levels;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Play
{
    public sealed class BlockViewBuilderTests
    {
        private const int Red = 1;
        private const int QuartersPerCell = 4;
        private const float Tolerance = 0.001f;

        private BoardArtSettings _art;
        private BoardRoot _board;
        private Transform _idle;
        private PooledBoardPieces _pieces;
        private BlockViewBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _art = Load<BoardArtSettings>();
            _board = new GameObject("Board").AddComponent<BoardRoot>();
            _idle = new GameObject("Idle Board Pieces").transform;
            var board = new SerializedObject(_board);
            board.FindProperty("_idlePieces").objectReferenceValue = _idle;
            board.ApplyModifiedPropertiesWithoutUndo();
            _pieces = new PooledBoardPieces(_board);
            _builder = new BlockViewBuilder(_art, Load<ColorPalette>(), Load<BlockMotionSettings>(), Load<IceSettings>(),
                new BlockPieceBuilder(_art));
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_board.gameObject);
            Object.DestroyImmediate(_idle.gameObject);
        }

        [Test]
        public void Rebuild_AfterACellIsHit_GivesTheOldPiecesBackAndBuildsTheCellsLeft()
        {
            var level = Line();
            var view = Build(level.Blocks[0]);
            var board = Boards.Of(level);
            board.HitCell(new Cell(2, 0));

            _builder.Rebuild(view, board.Blocks[0], new ArrowData(), _pieces);

            Assert.AreEqual(2, view.CellCount);
            Assert.AreEqual(2 * QuartersPerCell, view.GetComponentsInChildren<BoardPiece>().Length);
            Assert.AreEqual(QuartersPerCell, _idle.childCount);
            foreach (Transform piece in _idle)
                Assert.IsFalse(piece.gameObject.activeSelf, piece.name);
        }

        [Test]
        public void ReleaseAll_AfterARebuild_GivesBackEveryPieceOnce()
        {
            var level = Line();
            var view = Build(level.Blocks[0]);
            var board = Boards.Of(level);
            board.HitCell(new Cell(1, 0));
            _builder.Rebuild(view, board.Blocks[0], new ArrowData(), _pieces);

            _pieces.ReleaseAll();

            Assert.AreEqual(0, _board.transform.childCount);
            Assert.AreEqual(3 * QuartersPerCell + 1, _idle.childCount);
        }

        [Test]
        public void ReleasePieces_Twice_GivesNothingBackTheSecondTime()
        {
            var view = Build(Line().Blocks[0]);
            var other = Build(Line().Blocks[0]);
            view.ReleasePieces(_pieces);
            var taken = Build(Line().Blocks[0]);

            view.ReleasePieces(_pieces);

            Assert.AreEqual(3 * QuartersPerCell, taken.GetComponentsInChildren<BoardPiece>().Length);
            Assert.AreEqual(3 * QuartersPerCell, other.GetComponentsInChildren<BoardPiece>().Length);
            Assert.AreEqual(0, _idle.childCount);
        }

        [Test]
        public void Build_ForASplitPart_PlacesItsPiecesOnItsCellsFromTheSharedPosition()
        {
            var board = Boards.Of(Line());
            board.HitCell(new Cell(1, 0));
            var part = board.Blocks[1];

            var view = _builder.Build(part, new ArrowData(), Vector3.zero, _board.transform, _pieces);

            Assert.AreEqual(1, view.CellCount);
            var pieces = view.GetComponentsInChildren<BoardPiece>();
            Assert.AreEqual(QuartersPerCell, pieces.Length);
            var middle = Vector3.zero;
            foreach (var piece in pieces)
                middle += piece.transform.localPosition / pieces.Length;
            Assert.AreEqual(2 * _art.CellSize, middle.x, Tolerance);
            Assert.AreEqual(0f, middle.z, Tolerance);
        }

        [Test]
        public void Build_ForASplitPart_WithNoArrow_HidesTheIceAndArrowThePooledViewShowed()
        {
            var level = Line();
            level.Blocks[0].Ice = 2;
            level.Blocks[0].Arrow = new ArrowData { Axis = ArrowAxis.Horizontal, Start = new Cell(0, 0), Length = 2 };
            Build(level.Blocks[0]);
            _pieces.ReleaseAll();
            var board = Boards.Of(Line());
            board.HitCell(new Cell(1, 0));

            var view = _builder.Build(board.Blocks[1], new ArrowData(), Vector3.zero, _board.transform, _pieces);

            Assert.AreEqual(0, view.Ice.Shown);
            AssertNoArrowShown(view);
        }

        [Test]
        public void Build_ForTheSplitPartHoldingTheArrow_ShowsItWhereItWasDrawn()
        {
            var level = new LevelBuilder(5, 3).Rect(Red, 0, 0, 4, 1).Arrow(ArrowAxis.Horizontal, 2, 0, 2).Build();
            var mechanics = LevelMechanics.For(level);
            var board = new Board(level, mechanics.Rules, mechanics.Guards);
            board.HitCell(new Cell(1, 0));

            var view = _builder.Build(board.Blocks[1], mechanics.Arrow.ArrowOf(board, 1), Vector3.zero,
                _board.transform, _pieces);

            Assert.AreEqual(1, view.Arrow.GetComponentsInChildren<Renderer>().Length);
            Assert.AreEqual(2.5f * _art.CellSize, view.Arrow.transform.localPosition.x, Tolerance);
        }

        [Test]
        public void Rebuild_WithTheArrowGoneFromTheBlock_HidesIt()
        {
            var level = Line();
            level.Blocks[0].Arrow = new ArrowData { Axis = ArrowAxis.Horizontal, Start = new Cell(1, 0), Length = 2 };
            var view = Build(level.Blocks[0]);
            var board = Boards.Of(level);
            board.HitCell(new Cell(2, 0));

            _builder.Rebuild(view, board.Blocks[0], new ArrowData(), _pieces);

            AssertNoArrowShown(view);
        }

        private static void AssertNoArrowShown(BlockView view)
        {
            foreach (var model in view.Arrow.GetComponentsInChildren<Renderer>(true))
                Assert.IsFalse(model.gameObject.activeSelf, model.name);
        }

        private BlockView Build(BlockData block) => _builder.Build(block, Vector3.zero, _board.transform, _pieces);

        private static LevelData Line() => new LevelBuilder(4, 3).Rect(Red, 0, 0, 3, 1).Build();

        private static T Load<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { "Assets/Game" });
            Assert.AreEqual(1, guids.Length, $"one {typeof(T).Name} asset");
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
