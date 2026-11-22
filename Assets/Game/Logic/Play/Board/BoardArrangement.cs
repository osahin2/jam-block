using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    internal sealed class BoardArrangement
    {
        private readonly Cell[] _positions;
        private readonly bool[] _gone;

        public int BlockCount => _positions.Length;

        public BoardArrangement(IReadOnlyList<BoardBlock> blocks)
        {
            _positions = new Cell[blocks.Count];
            _gone = new bool[blocks.Count];
            for (var i = 0; i < blocks.Count; i++)
            {
                _gone[i] = blocks[i].IsGone;
                _positions[i] = blocks[i].Position;
            }
        }

        public Cell PositionOf(int block) => _positions[block];

        public bool IsGone(int block) => _gone[block];
    }
}
