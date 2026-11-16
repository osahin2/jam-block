using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Core.Pooling
{
    /// <summary>
    /// Reuses instances of one prefab. Idle instances wait inactive under <c>root</c>; <see cref="Get"/> hands one out
    /// active under the given parent, <see cref="Release"/> takes it back. At most <c>maxSize</c> idle instances are
    /// kept; one released beyond that is destroyed. Built on <see cref="ObjectPool{T}"/>.
    /// </summary>
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

        /// <summary>
        /// Returns an idle instance, or a new one if none is idle, active under <paramref name="parent"/> (null for
        /// the scene root) with the prefab's local position, rotation and scale, like <c>Instantiate(prefab, parent)</c>.
        /// </summary>
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

        /// <summary>
        /// Deactivates the instance and puts it back under the root. Releasing the same instance twice throws in the
        /// editor. If the pool already holds max size idle instances, this one is destroyed instead.
        /// </summary>
        public void Release(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _pool.Release(instance);
        }

        /// <summary>
        /// Destroys the idle instances. Instances handed out by <see cref="Get"/> are left to their owner and can
        /// still be released. The pool stays usable.
        /// </summary>
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
