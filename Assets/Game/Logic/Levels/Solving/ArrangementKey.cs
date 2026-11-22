using System;
using System.Collections.Generic;
using Game.Logic.Play;

namespace Game.Logic.Levels
{
    internal sealed class ArrangementKey : IEquatable<ArrangementKey>
    {
        private readonly int[] _codes;
        private int _hash;

        public ArrangementKey(int blockCount) => _codes = new int[blockCount];

        public void Read(InterchangeableBlocks alike, IReadOnlyList<BoardBlock> blocks)
        {
            alike.Write(blocks, _codes);
            var hash = 17;
            for (var i = 0; i < _codes.Length; i++)
                hash = hash * 31 + _codes[i];
            _hash = hash;
        }

        public ArrangementKey Copy()
        {
            var copy = new ArrangementKey(_codes.Length);
            Array.Copy(_codes, copy._codes, _codes.Length);
            copy._hash = _hash;
            return copy;
        }

        public bool Equals(ArrangementKey other)
        {
            if (other == null || other._hash != _hash || other._codes.Length != _codes.Length)
                return false;
            for (var i = 0; i < _codes.Length; i++)
            {
                if (other._codes[i] != _codes[i])
                    return false;
            }
            return true;
        }

        public override bool Equals(object obj) => obj is ArrangementKey other && Equals(other);

        public override int GetHashCode() => _hash;
    }
}
