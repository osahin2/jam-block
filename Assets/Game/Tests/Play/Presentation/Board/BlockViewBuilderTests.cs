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
        private LevelMechanics _mechanics;
        private Board _logic;

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
            _builder = BlockViewBuilders.WithTheGamesFeatures();
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
            StartLevel(Line());
            var view = Build(0);
            _logic.HitCell(new Cell(2, 0));

            _builder.Rebuild(view, On(0), _pieces);

            Assert.AreEqual(2, view.CellCount);
            Assert.AreEqual(2 * QuartersPerCell, view.GetComponentsInChildren<BoardPiece>().Length);
            Assert.AreEqual(QuartersPerCell, _idle.childCount);
            foreach (Transform piece in _idle)
                Assert.IsFalse(piece.gameObject.activeSelf, piece.name);
        }

        [Test]
        public void ReleaseAll_AfterARebuild_GivesBackEveryPieceOnce()
        {
            StartLevel(Line());
            var view = Build(0);
            _logic.HitCell(new Cell(1, 0));
            _builder.Rebuild(view, On(0), _pieces);

            _pieces.ReleaseAll();

            Assert.AreEqual(0, _board.transform.childCount);
            Assert.AreEqual(3 * QuartersPerCell + 1, _idle.childCount);
        }

        [Test]
        public void ReleasePieces_Twice_GivesNothingBackTheSecondTime()
        {
            StartLevel(Line());
            var view = Build(0);
            var other = Build(0);
            view.ReleasePieces(_pieces);
            var taken = Build(0);

            view.ReleasePieces(_pieces);

            Assert.AreEqual(3 * QuartersPerCell, taken.GetComponentsInChildren<BoardPiece>().Length);
            Assert.AreEqual(3 * QuartersPerCell, other.GetComponentsInChildren<BoardPiece>().Length);
            Assert.AreEqual(0, _idle.childCount);
        }

        [Test]
        public void Build_ForASplitPart_PlacesItsPiecesOnItsCellsFromTheSharedPosition()
        {
            StartLevel(Line());
            _logic.HitCell(new Cell(1, 0));

            var view = Build(1);

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
        public void Build_ABlockWithIce_ShowsItsIce()
        {
            var level = Line();
            level.Blocks[0].Ice = 2;
            StartLevel(level);

            var view = Build(0);

            Assert.AreEqual(2, view.Part<IceCover>().Shown);
        }

        [Test]
        public void Build_ForASplitPart_WithNoArrow_HidesTheIceAndArrowThePooledViewShowed()
        {
            var level = Line();
            level.Blocks[0].Ice = 2;
            level.Blocks[0].Arrow = new ArrowData { Axis = ArrowAxis.Horizontal, Start = new Cell(0, 0), Length = 2 };
            StartLevel(level);
            Build(0);
            _pieces.ReleaseAll();
            StartLevel(Line());
            _logic.HitCell(new Cell(1, 0));

            var view = Build(1);

            Assert.AreEqual(0, view.Part<IceCover>().Shown);
            AssertNoArrowShown(view);
        }

        [Test]
        public void Build_ForTheSplitPartHoldingTheArrow_ShowsItWhereItWasDrawn()
        {
            StartLevel(new LevelBuilder(5, 3).Rect(Red, 0, 0, 4, 1).Arrow(ArrowAxis.Horizontal, 2, 0, 2).Build());
            _logic.HitCell(new Cell(1, 0));

            var view = Build(1);

            var arrow = view.Part<BlockArrow>();
            Assert.AreEqual(1, arrow.GetComponentsInChildren<Renderer>().Length);
            Assert.AreEqual(2.5f * _art.CellSize, arrow.transform.localPosition.x, Tolerance);
        }

        [Test]
        public void Rebuild_WithTheArrowGoneFromTheBlock_HidesIt()
        {
            var level = Line();
            level.Blocks[0].Arrow = new ArrowData { Axis = ArrowAxis.Horizontal, Start = new Cell(1, 0), Length = 2 };
            StartLevel(level);
            var view = Build(0);
            _logic.HitCell(new Cell(2, 0));

            _builder.Rebuild(view, On(0), _pieces);

            AssertNoArrowShown(view);
        }

        private static void AssertNoArrowShown(BlockView view)
        {
            foreach (var model in view.Part<BlockArrow>().GetComponentsInChildren<Renderer>(true))
                Assert.IsFalse(model.gameObject.activeSelf, model.name);
        }

        private void StartLevel(LevelData level)
        {
            _mechanics = LevelMechanics.For(level);
            _logic = new Board(level, _mechanics.Rules, _mechanics.Guards);
        }

        private BlockOnBoard On(int index) => new BlockOnBoard(_logic, _mechanics, index);

        private BlockView Build(int index) => _builder.Build(On(index), Vector3.zero, _board.transform, _pieces);

        private static LevelData Line() => new LevelBuilder(4, 3).Rect(Red, 0, 0, 3, 1).Build();

        private static T Load<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { "Assets/Game" });
            Assert.AreEqual(1, guids.Length, $"one {typeof(T).Name} asset");
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
