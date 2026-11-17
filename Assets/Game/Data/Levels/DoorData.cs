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

        // One past the last covered cell along the side.
        public int End => Start + Length;

        public bool Covers(int along) => along >= Start && along < End;
    }
}
