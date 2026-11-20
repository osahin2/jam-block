using System.Collections.Generic;
using Core.Pooling;
using UnityEngine;

namespace Game.Play
{
    // The game's board pieces: one Core pool per prefab, made empty the first time that prefab is asked for, idle under
    // the board root's idle object, so the pools fill while the first level is built and live and die with the Game
    // scene. It remembers every instance it hands out, so the caller gives a whole board back with one call before
    // building the next one, which then reuses those instances.
    public sealed class PooledBoardPieces : IBoardPieceSource
    {
        // Idle instances kept per prefab. A board needing more of one piece (over 16 x 16 cells for the ground) destroys
        // the excess when given back and makes it again on the next build.
        private const int MaxIdlePerPrefab = 256;

        private readonly Pools<BoardPiece> _pieces;
        private readonly Pools<BlockView> _blocks;
        private readonly Pools<DoorView> _doors;

        public PooledBoardPieces(BoardRoot root)
        {
            _pieces = new Pools<BoardPiece>(root.IdlePieces);
            _blocks = new Pools<BlockView>(root.IdlePieces);
            _doors = new Pools<DoorView>(root.IdlePieces);
        }

        public BoardPiece Take(BoardPiece prefab, Transform parent) => _pieces.Get(prefab, parent);

        public BlockView Take(BlockView prefab, Transform parent) => _blocks.Get(prefab, parent);

        public DoorView Take(DoorView prefab, Transform parent) => _doors.Get(prefab, parent);

        // Gives back every instance handed out since the last call. Pieces first: a block's pieces are its children and
        // go back to their own pools before the block does.
        public void ReleaseAll()
        {
            _pieces.ReleaseAll();
            _blocks.ReleaseAll();
            _doors.ReleaseAll();
        }

        // One pool per prefab, made on first use, and what was handed out since the last release.
        private sealed class Pools<T> where T : Component
        {
            private readonly Transform _idle;
            private readonly Dictionary<T, PrefabPool<T>> _byPrefab = new Dictionary<T, PrefabPool<T>>();
            private readonly List<(PrefabPool<T> Pool, T Instance)> _taken = new List<(PrefabPool<T>, T)>();

            public Pools(Transform idle)
            {
                _idle = idle;
            }

            public T Get(T prefab, Transform parent)
            {
                if (!_byPrefab.TryGetValue(prefab, out var pool))
                {
                    pool = new PrefabPool<T>(prefab, _idle, 0, MaxIdlePerPrefab);
                    _byPrefab.Add(prefab, pool);
                }
                var instance = pool.Get(parent);
                _taken.Add((pool, instance));
                return instance;
            }

            public void ReleaseAll()
            {
                foreach (var (pool, instance) in _taken)
                    pool.Release(instance);
                _taken.Clear();
            }
        }
    }
}
