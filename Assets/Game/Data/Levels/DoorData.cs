using System;

namespace Game.Data.Levels
{
    [Serializable]
    public sealed class DoorData
    {
        public int ColorId;
        public Side Side;
        // First covered cell along the side: a column on Top and Bottom, a row on Left and Right.
        public int Start;
        public int Length = 1;
    }
}
