using System;

namespace Game.Data.Levels
{
    [Serializable]
    public sealed class DoorData
    {
        public int ColorId;
        public Side Side;
        // A column on Top and Bottom, a row on Left and Right.
        public int Start;
        public int Length = 1;

        public int End => Start + Length;

        public bool Covers(int along) => along >= Start && along < End;

        public bool FitsSide(int width, int height) => SpanFits(Side, Start, End, width, height);

        public bool Overlaps(Side side, int start, int end) => Side == side && Start < end && start < End;

        public static bool SpanFits(Side side, int start, int end, int width, int height) =>
            start >= 0 && end > start && end <= side.Length(width, height);
    }
}
