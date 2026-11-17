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

        // At least one cell long and within its side of a board of this size.
        public bool FitsSide(int width, int height) => SpanFits(Side, Start, End, width, height);

        // Shares a cell with the span from start to end (exclusive) on the given side.
        public bool Overlaps(Side side, int start, int end) => Side == side && Start < end && start < End;

        public static bool SpanFits(Side side, int start, int end, int width, int height) =>
            start >= 0 && end > start && end <= side.Length(width, height);
    }
}
