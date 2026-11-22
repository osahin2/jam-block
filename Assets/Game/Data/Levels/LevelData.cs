using System;
using System.Collections.Generic;

namespace Game.Data.Levels
{
    [Serializable]
    public sealed class LevelData
    {
        public int Width = 5;
        public int Height = 6;
        public int TimeLimitSeconds = 300;
        public List<BlockData> Blocks = new List<BlockData>();
        public List<DoorData> Doors = new List<DoorData>();

        public bool IsOnBoard(Cell cell) => cell.IsInside(Width, Height);
    }
}
