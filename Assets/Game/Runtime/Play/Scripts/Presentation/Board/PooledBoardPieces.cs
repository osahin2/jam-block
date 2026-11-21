using System.Collections.Generic;
using Core.Pooling;
using UnityEngine;

namespace Game.Play
{
    public sealed class PooledBoardPieces : IBoardPieceSource
    {
        private const int MaxIdlePerPrefab = 256;

        private readonly Transform _idle;
        private readonly Dictionary<Component, PrefabPool<Component>> _pools =
            new Dictionary<Component, PrefabPool<Component>>();
        private readonly List<(PrefabPool<Component> Pool, Component Instance)> _taken =
            new List<(PrefabPool<Component>, Component)>();

        public PooledBoardPieces(BoardRoot root)
        {
            _idle = root.IdlePieces;
        }

        public T Take<T>(T prefab, Transform parent) where T : Component
        {
            if (!_pools.TryGetValue(prefab, out var pool))
            {
                pool = new PrefabPool<Component>(prefab, _idle, 0, MaxIdlePerPrefab);
                _pools.Add(prefab, pool);
            }
            var instance = (T)pool.Get(parent);
            _taken.Add((pool, instance));
            return instance;
        }

        public void ReleaseAll()
        {
            for (var i = _taken.Count - 1; i >= 0; i--)
                _taken[i].Pool.Release(_taken[i].Instance);
            _taken.Clear();
        }
    }
}
