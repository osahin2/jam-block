using System;
using System.Collections.Generic;

namespace Game.Data.Levels
{
    [Serializable]
    public sealed class BlockData
    {
        public int ColorId;
        public Cell Position;
        // Relative to Position.
        public List<Cell> Shape = new List<Cell>();
    }
}
