using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Constraints;
using Allocating = UnityEngine.TestTools.Constraints.Is;

namespace Game.Tests.Play
{
    public sealed class OutlineTests
    {
        private GameObject _block;
        private Outline _outline;
        private Renderer[] _pieces;
        private Material _own;
        private Material _mask;
        private Material _fill;

        [SetUp]
        public void SetUp()
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            _own = new Material(shader);
            _mask = new Material(shader);
            _fill = new Material(shader);
            _block = new GameObject("Block");
            _outline = _block.AddComponent<Outline>();
            _pieces = new Renderer[2];
            for (var i = 0; i < _pieces.Length; i++)
            {
                _pieces[i] = new GameObject("Piece").AddComponent<MeshRenderer>();
                _pieces[i].transform.SetParent(_block.transform);
                _pieces[i].sharedMaterial = _own;
            }
            _outline.SetRenderers(_pieces, _mask, _fill);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_block);
            Object.DestroyImmediate(_own);
            Object.DestroyImmediate(_mask);
            Object.DestroyImmediate(_fill);
        }

        [Test]
        public void SetRenderers_LeavesItHidden()
        {
            Assert.IsFalse(_outline.IsShown);
            Assert.AreEqual(new[] { _own }, _pieces[0].sharedMaterials);
        }

        [Test]
        public void Show_AddsTheMaskAndFillToEveryRenderer_HideTakesThemOff()
        {
            _outline.Show();

            foreach (var piece in _pieces)
                Assert.AreEqual(new[] { _own, _mask, _fill }, piece.sharedMaterials);

            _outline.Hide();

            foreach (var piece in _pieces)
                Assert.AreEqual(new[] { _own }, piece.sharedMaterials);
        }

        [Test]
        public void SetRenderers_ForgetsTheRenderersBefore()
        {
            var other = new GameObject("Other").AddComponent<MeshRenderer>();
            other.transform.SetParent(_block.transform);
            other.sharedMaterial = _own;

            _outline.SetRenderers(new Renderer[] { other }, _mask, _fill);
            _outline.Show();

            Assert.AreEqual(new[] { _own }, _pieces[0].sharedMaterials);
            Assert.AreEqual(new[] { _own, _mask, _fill }, other.sharedMaterials);
        }

        [Test]
        public void SetRenderers_WhileShown_TakesTheOutlineOffTheRenderersBefore()
        {
            _outline.Show();

            _outline.SetRenderers(new Renderer[0], _mask, _fill);

            Assert.IsFalse(_outline.IsShown);
            foreach (var piece in _pieces)
                Assert.AreEqual(new[] { _own }, piece.sharedMaterials);
        }

        [Test]
        public void ShowAndHide_AllocateNothing()
        {
            _outline.Show();
            _outline.Hide();

            Assert.That(() =>
            {
                _outline.Show();
                _outline.Hide();
            }, Allocating.Not.AllocatingGCMemory());
        }
    }
}
