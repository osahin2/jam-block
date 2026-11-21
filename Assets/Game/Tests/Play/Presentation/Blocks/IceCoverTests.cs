using System.Collections.Generic;
using Game.Data.Levels;
using Game.Play;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Play
{
    public sealed class IceCoverTests
    {
        private static readonly Cell[] Shape = { new Cell(0, 0), new Cell(1, 0) };

        private Transform _block;
        private IceCover _cover;
        private Renderer _piece;
        private GameObject _count;

        [SetUp]
        public void SetUp()
        {
            _block = new GameObject("Block").transform;
            _cover = IceCovers.Under(_block);
            _count = _cover.GetComponentInChildren<TextMeshPro>(true).gameObject;
            _piece = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<Renderer>();
            _piece.transform.SetParent(_block);
            _piece.sharedMaterial = Load<ColorPalette>().ForBlock(1);
            _cover.Init(Load<IceSettings>(), new List<Renderer> { _piece });
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_block.gameObject);

        [Test]
        public void Show_WithIce_ShowsTheCountAndTurnsThePiecesToIce()
        {
            Show(2, Shape);

            Assert.AreEqual(2, _cover.Shown);
            Assert.IsTrue(_count.activeSelf);
            Assert.IsTrue(_piece.HasPropertyBlock());
        }

        [Test]
        public void Show_WithoutIce_ShowsNoCountAndLeavesThePiecesAsTheyAre()
        {
            Show(2, Shape);

            Show(0, Shape);

            Assert.AreEqual(0, _cover.Shown);
            Assert.IsFalse(_count.activeSelf);
            Assert.IsFalse(_piece.HasPropertyBlock());
        }

        [Test]
        public void Show_ABlockWithoutCells_ShowsNoIce()
        {
            Show(2, new Cell[0]);

            Assert.AreEqual(0, _cover.Shown);
            Assert.IsFalse(_count.activeSelf);
        }

        [Test]
        public void CatchUp_WithLessIceLeft_CracksDownToIt()
        {
            Show(3, Shape);

            _cover.CatchUp(1);

            Assert.AreEqual(1, _cover.Shown);
            Assert.IsTrue(_count.activeSelf);
            Assert.IsTrue(_piece.HasPropertyBlock());
        }

        [Test]
        public void CatchUp_WithTheSameIceLeft_ChangesNothing()
        {
            Show(2, Shape);

            _cover.CatchUp(2);
            _cover.CatchUp(3);

            Assert.AreEqual(2, _cover.Shown);
        }

        [Test]
        public void CatchUp_WithNoIceLeft_MeltsIt()
        {
            Show(1, Shape);

            _cover.CatchUp(0);

            Assert.AreEqual(0, _cover.Shown);
        }

        private void Show(int ice, Cell[] shape) =>
            _cover.Show(ice, shape, _piece.sharedMaterial, Load<BoardArtSettings>());

        private static T Load<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { "Assets/Game" });
            Assert.AreEqual(1, guids.Length, $"one {typeof(T).Name} asset");
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
