using System;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Pooling.Tests
{
    public sealed class PrefabPoolTests
    {
        private GameObject _prefab;
        private Transform _root;
        private Transform _parent;

        [SetUp]
        public void SetUp()
        {
            _prefab = new GameObject("Prefab");
            _root = new GameObject("PoolRoot").transform;
            _parent = new GameObject("Parent").transform;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_parent.gameObject);
            Object.DestroyImmediate(_root.gameObject);
            Object.DestroyImmediate(_prefab);
        }

        [Test]
        public void Prewarm_CreatesInactiveInstancesUnderRoot()
        {
            CreatePool(prewarmCount: 3, maxSize: 5);

            Assert.AreEqual(3, _root.childCount);
            foreach (Transform child in _root)
                Assert.IsFalse(child.gameObject.activeSelf);
        }

        [Test]
        public void Get_ActivatesAndReparents()
        {
            var pool = CreatePool(prewarmCount: 1, maxSize: 5);

            var instance = pool.Get(_parent);

            Assert.IsTrue(instance.gameObject.activeSelf);
            Assert.AreSame(_parent, instance.parent);
            Assert.AreEqual(0, _root.childCount);
        }

        [Test]
        public void Get_RestoresPrefabLocalPose()
        {
            _prefab.transform.localScale = new Vector3(2f, 2f, 2f);
            var pool = CreatePool(prewarmCount: 0, maxSize: 5);
            var instance = pool.Get(_parent);
            instance.localPosition = new Vector3(5f, 0f, 0f);
            instance.localScale = Vector3.zero;
            pool.Release(instance);

            var reused = pool.Get(_parent);

            Assert.AreEqual(Vector3.zero, reused.localPosition);
            Assert.AreEqual(new Vector3(2f, 2f, 2f), reused.localScale);
        }

        [Test]
        public void Release_DeactivatesAndReturnsUnderRoot()
        {
            var pool = CreatePool(prewarmCount: 0, maxSize: 5);
            var instance = pool.Get(_parent);

            pool.Release(instance);

            Assert.IsFalse(instance.gameObject.activeSelf);
            Assert.AreSame(_root, instance.parent);
        }

        [Test]
        public void GetAfterRelease_ReusesTheSameInstance()
        {
            var pool = CreatePool(prewarmCount: 0, maxSize: 5);
            var first = pool.Get(_parent);
            pool.Release(first);

            var second = pool.Get(_parent);

            Assert.AreSame(first, second);
        }

        [Test]
        public void ReleaseTwice_Throws()
        {
            var pool = CreatePool(prewarmCount: 0, maxSize: 5);
            var instance = pool.Get(_parent);
            pool.Release(instance);

            Assert.Throws<InvalidOperationException>(() => pool.Release(instance));
        }

        [Test]
        public void Clear_DestroysIdleInstances_AndLeavesHandedOutOnes()
        {
            var pool = CreatePool(prewarmCount: 2, maxSize: 5);
            var handedOut = pool.Get(_parent);
            var idle = _root.GetChild(0);

            pool.Clear();

            Assert.IsTrue(idle == null);
            Assert.AreEqual(0, _root.childCount);
            Assert.IsTrue(handedOut != null);
        }

        [Test]
        public void Get_BeyondMaxSize_StillCreatesInstances()
        {
            var pool = CreatePool(prewarmCount: 0, maxSize: 2);

            var a = pool.Get(_parent);
            var b = pool.Get(_parent);
            var c = pool.Get(_parent);

            Assert.AreEqual(3, _parent.childCount);
            Assert.AreNotSame(a, c);
            Assert.AreNotSame(b, c);
        }

        [Test]
        public void Release_BeyondMaxSize_DestroysTheExtraInstance()
        {
            var pool = CreatePool(prewarmCount: 0, maxSize: 2);
            var a = pool.Get(_parent);
            var b = pool.Get(_parent);
            var c = pool.Get(_parent);

            pool.Release(a);
            pool.Release(b);
            pool.Release(c);

            Assert.AreEqual(2, _root.childCount);
            Assert.IsTrue(c == null);
        }

        [Test]
        public void Constructor_RejectsPrewarmAboveMaxSize()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreatePool(prewarmCount: 3, maxSize: 2));
        }

        private PrefabPool<Transform> CreatePool(int prewarmCount, int maxSize)
        {
            return new PrefabPool<Transform>(_prefab.transform, _root, prewarmCount, maxSize);
        }
    }
}
