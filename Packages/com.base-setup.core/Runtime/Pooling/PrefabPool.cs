using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Core.Pooling
{
    public sealed class PrefabPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _root;
        private readonly ObjectPool<T> _pool;
        private readonly Vector3 _prefabLocalPosition;
        private readonly Quaternion _prefabLocalRotation;
        private readonly Vector3 _prefabLocalScale;

        public PrefabPool(T prefab, Transform root, int prewarmCount, int maxSize)
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));
            if (root == null)
                throw new ArgumentNullException(nameof(root));
            if (maxSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxSize), maxSize, "[Core.Pooling] Max size must be greater than 0.");
            if (prewarmCount < 0 || prewarmCount > maxSize)
                throw new ArgumentOutOfRangeException(nameof(prewarmCount), prewarmCount, "[Core.Pooling] Prewarm count must be between 0 and max size.");

            _prefab = prefab;
            _root = root;
            var prefabTransform = prefab.transform;
            _prefabLocalPosition = prefabTransform.localPosition;
            _prefabLocalRotation = prefabTransform.localRotation;
            _prefabLocalScale = prefabTransform.localScale;

            // Activation lives in Get, not in an actionOnGet callback, so prewarming never enables an instance.
            _pool = new ObjectPool<T>(
                Create,
                actionOnGet: null,
                OnRelease,
                DestroyInstance,
                collectionCheck: Application.isEditor,
                defaultCapacity: prewarmCount,
                maxSize: maxSize);

            Prewarm(prewarmCount);
        }

        public T Get(Transform parent)
        {
            var instance = _pool.Get();
            var instanceTransform = instance.transform;
            instanceTransform.SetParent(parent, false);
            instanceTransform.SetLocalPositionAndRotation(_prefabLocalPosition, _prefabLocalRotation);
            instanceTransform.localScale = _prefabLocalScale;
            instance.gameObject.SetActive(true);
            return instance;
        }

        // Releasing an instance twice throws in the editor; one beyond the max size is destroyed.
        public void Release(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _pool.Release(instance);
        }

        // Instances handed out by Get stay with their owner and can still be released.
        public void Clear()
        {
            _pool.Clear();
        }

        private T Create()
        {
            var instance = Object.Instantiate(_prefab, _root);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void OnRelease(T instance)
        {
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_root, false);
        }

        private void Prewarm(int count)
        {
            var instances = new T[count];
            for (var i = 0; i < count; i++)
                instances[i] = _pool.Get();
            for (var i = 0; i < count; i++)
                _pool.Release(instances[i]);
        }

        private static void DestroyInstance(T instance)
        {
            if (instance == null)
                return;

            // Destroy is deferred and refused outside play mode; edit-mode callers such as tests need the immediate form.
            if (Application.isPlaying)
                Object.Destroy(instance.gameObject);
            else
                Object.DestroyImmediate(instance.gameObject);
        }
    }
}
