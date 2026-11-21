using System.Collections.Generic;
using Game.Data.Levels;
using Game.Editor.LevelEditor;
using Game.Play;
using Game.Tests.Levels;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Play
{
    public sealed class BoardBuilderTests
    {
        private const int Red = 1;
        private const int Other = 2;
        private const float Tolerance = 0.02f;

        private BoardArtSettings _art;
        private ColorPalette _palette;
        private BoardBuilder _builder;
        private Transform _board;

        [SetUp]
        public void SetUp()
        {
            _art = Load<BoardArtSettings>();
            _palette = Load<ColorPalette>();
            var motion = Load<BlockMotionSettings>();
            _builder = new BoardBuilder(_art, _palette, motion,
                new BlockViewBuilder(_art, _palette, motion, Load<IceSettings>(), new BlockPieceBuilder(_art)));
            _board = new GameObject("Board").transform;
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_board.gameObject);

        [Test]
        public void Build_DoorInTheMiddleOfASide_OneBarPerWallRunAndPerDoor()
        {
            var layout = Build(new LevelBuilder(4, 5).Rect(Red, 1, 0, 2, 1).Door(Red, Side.Top, 1, 2).Build()).Layout;
            var cell = layout.CellSize;

            var walls = TopPieces(_art.Wall, layout);
            var doors = TopPieces(_art.Door, layout);

            Assert.AreEqual(2, walls.Count);
            Assert.AreEqual(1, doors.Count);
            AssertSpan(layout.Left, layout.Left + cell, walls[0]);
            AssertSpan(layout.Left + cell, layout.Left + 3f * cell, doors[0]);
            AssertSpan(layout.Left + 3f * cell, layout.Right, walls[1]);
            Assert.AreEqual(2 + 3, Pieces(_art.Wall).Count);
        }

        [Test]
        public void Build_Door_PutsOneArrowOnItsTopAtItsMiddle_PointingOut()
        {
            var layout = Build(new LevelBuilder(4, 5).Rect(Red, 1, 0, 2, 1).Door(Red, Side.Top, 1, 2).Build()).Layout;

            var arrows = Pieces(_art.DoorArrow);
            var door = TopPieces(_art.Door, layout)[0].transform;

            Assert.AreEqual(1, arrows.Count);
            var arrow = arrows[0].transform;
            Assert.AreEqual(door.localPosition.x, arrow.localPosition.x, 1e-4f);
            Assert.AreEqual(door.localPosition.z, arrow.localPosition.z, 1e-4f);
            Assert.AreEqual(_art.FrameTop, arrow.localPosition.y, 1e-4f);
            Assert.AreEqual(1f, Vector3.Dot(arrow.forward, Vector3.forward), 1e-4f);
        }

        [Test]
        public void Build_Doors_OneViewPerDoorInLevelOrder_DoorAndBurstInItsColor()
        {
            var level = new LevelBuilder(4, 5).Rect(Red, 0, 0, 1, 1).Rect(Other, 3, 4, 1, 1)
                .Door(Other, Side.Right, 4).Door(Red, Side.Top, 0).Build();

            var staged = Build(level);

            Assert.AreEqual(2, staged.Doors.Length);
            Assert.Greater(staged.Doors[0].transform.localPosition.x, staged.Layout.Right, "the first door is on the right");
            Assert.Greater(staged.Doors[1].transform.localPosition.z, staged.Layout.Top, "the second door is on top");
            var door = staged.Doors[0];
            Assert.AreSame(_palette.ForDoor(Other), door.GetComponentInChildren<MeshRenderer>().sharedMaterial);
            Assert.AreSame(_palette.ForDoor(Other), door.GetComponentInChildren<ParticleSystemRenderer>().sharedMaterial);
        }

        [Test]
        public void Build_GroundBase_LiesUnderEveryTile_AndReachesUnderTheWalls()
        {
            var layout = Build(new LevelBuilder(4, 5).Rect(Red, 1, 0, 2, 1).Build()).Layout;

            var bases = Pieces(_art.GroundBase);
            Assert.AreEqual(1, bases.Count);
            var under = bases[0].GetComponentInChildren<MeshRenderer>().bounds;
            var reach = _art.WallWidth * 0.5f;
            Assert.AreEqual(layout.Left - reach, under.min.x, Tolerance);
            Assert.AreEqual(layout.Right + reach, under.max.x, Tolerance);
            Assert.AreEqual(-layout.Top - reach, under.min.z, Tolerance);
            Assert.AreEqual(layout.Top + reach, under.max.z, Tolerance);
            foreach (var tile in Pieces(_art.Ground))
                Assert.Less(under.max.y, tile.GetComponentInChildren<MeshRenderer>().bounds.max.y, tile.name);
        }

        private StagedLevel Build(LevelData level) =>
            _builder.Build(level, new BoardLayout(level.Width, level.Height, _art), _board, new PreviewBoardPieces());

        private List<T> TopPieces<T>(T prefab, BoardLayout layout) where T : Component
        {
            var top = Pieces(prefab).FindAll(piece => piece.transform.localPosition.z > layout.Top);
            top.Sort((a, b) => a.transform.localPosition.x.CompareTo(b.transform.localPosition.x));
            return top;
        }

        private List<T> Pieces<T>(T prefab) where T : Component
        {
            var made = new List<T>();
            foreach (var piece in _board.GetComponentsInChildren<T>())
            {
                if (piece.name == prefab.name + "(Clone)")
                    made.Add(piece);
            }
            return made;
        }

        private static void AssertSpan(float from, float to, Component piece)
        {
            var bounds = piece.GetComponentInChildren<MeshRenderer>().bounds;
            Assert.AreEqual(from, bounds.min.x, Tolerance, piece.name);
            Assert.AreEqual(to, bounds.max.x, Tolerance, piece.name);
        }

        private static T Load<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { "Assets/Game" });
            Assert.AreEqual(1, guids.Length, $"one {typeof(T).Name} asset");
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
