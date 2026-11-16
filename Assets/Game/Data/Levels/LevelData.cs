using System;
using System.Collections.Generic;

namespace Game.Data.Levels
{
    // Cells count columns (X) from the left and rows (Y) from the top.
    [Serializable]
    public sealed class LevelData
    {
        public int Width = 5;
        public int Height = 6;
        public int TimeLimitSeconds = 300;
        public List<BlockData> Blocks = new List<BlockData>();
        public List<DoorData> Doors = new List<DoorData>();
    }
}
