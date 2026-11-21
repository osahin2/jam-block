using DG.Tweening;
using Game.Data.Levels;
using Game.Play;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Play
{
    public sealed class BlockVanishTests
    {
        private BlockView _view;
        private Outline _outline;
        private BlockMotionSettings _motion;
        private IceSettings _ice;

        [SetUp]
        public void SetUp()
        {
            _view = new GameObject("Block").AddComponent<BlockView>();
            _outline = _view.gameObject.AddComponent<Outline>();
            var view = new SerializedObject(_view);
            view.FindProperty("_outline").objectReferenceValue = _outline;
            view.FindProperty("_ice").objectReferenceValue = IceCovers.Under(_view.transform);
            view.ApplyModifiedPropertiesWithoutUndo();
            _motion = ScriptableObject.CreateInstance<BlockMotionSettings>();
            _ice = ScriptableObject.CreateInstance<IceSettings>();
            _view.Init(_motion, _ice, new[] { new Cell(0, 0) });
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_view.gameObject);
            Object.DestroyImmediate(_motion);
            Object.DestroyImmediate(_ice);
        }

        [Test]
        public void TryVanish_OnAShownBlock_StartsLeavingAndHidesTheOutline()
        {
            _view.Hold();

            Assert.IsTrue(_view.TryVanish(Vector3.up, 0.3f, 0f, Ease.InQuad));

            Assert.IsTrue(_view.IsLeaving);
            Assert.IsFalse(_outline.IsShown);
        }

        [Test]
        public void TryVanish_OnABlockAlreadyLeavingOrHidden_DoesNothing()
        {
            _view.Leave(Vector3.zero, Vector3.forward, 0.3f);
            Assert.IsFalse(_view.TryVanish(Vector3.up, 0.3f, 0f, Ease.InQuad));

            _view.gameObject.SetActive(false);
            Assert.IsFalse(_view.TryVanish(Vector3.up, 0.3f, 0f, Ease.InQuad));
        }

        [Test]
        public void Init_PutsBackTheScaleABlockVanishedWith()
        {
            _view.transform.localScale = Vector3.zero;

            _view.Init(_motion, _ice, new[] { new Cell(0, 0) });

            Assert.AreEqual(Vector3.one, _view.transform.localScale);
        }
    }
}
