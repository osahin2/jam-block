using System;
using System.Collections.Generic;
using Game.Data.Levels;
using Game.Logic.Play;

namespace Game.Logic.Levels
{
    internal sealed class InterchangeableBlocks
    {
        private const int GoneCode = int.MinValue;

        private readonly int[] _order;
        private readonly List<int> _groupStarts = new List<int>();
        private readonly List<int> _groupSizes = new List<int>();

        public int BlockCount => _order.Length;

        public InterchangeableBlocks(LevelData level)
        {
            var blocks = level.Blocks;
            var order = new List<int>(blocks.Count);
            var grouped = new bool[blocks.Count];
            for (var first = 0; first < blocks.Count; first++)
            {
                if (grouped[first])
                    continue;
                var start = order.Count;
                for (var other = first; other < blocks.Count; other++)
                {
                    if (grouped[other] || !AreAlike(blocks[first], blocks[other]))
                        continue;
                    grouped[other] = true;
                    order.Add(other);
                }
                if (order.Count - start < 2)
                    continue;
                _groupStarts.Add(start);
                _groupSizes.Add(order.Count - start);
            }
            _order = order.ToArray();
        }

        public static bool AreAlike(BlockData first, BlockData second) =>
            first.ColorId == second.ColorId && first.Ice == second.Ice &&
            SameShape(first.Shape, second.Shape) && SameArrow(first.Arrow, second.Arrow);

        public void Write(IReadOnlyList<BoardBlock> blocks, int[] codes)
        {
            for (var slot = 0; slot < _order.Length; slot++)
                codes[slot] = CodeOf(blocks[_order[slot]]);
            for (var group = 0; group < _groupStarts.Count; group++)
                Array.Sort(codes, _groupStarts[group], _groupSizes[group]);
        }

        private static int CodeOf(BoardBlock block) =>
            block.IsGone ? GoneCode : (block.Position.X << 16) | (block.Position.Y & 0xFFFF);

        private static bool SameShape(List<Cell> first, List<Cell> second)
        {
            if (first.Count != second.Count)
                return false;
            for (var i = 0; i < first.Count; i++)
            {
                if (first[i] != second[i])
                    return false;
            }
            return true;
        }

        private static bool SameArrow(ArrowData first, ArrowData second) =>
            first.Axis == second.Axis && first.Start == second.Start && first.Length == second.Length;
    }
}
