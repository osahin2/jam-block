using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public static class LevelLookup
    {
        public const int NoBlock = -1;
        public const int NoDoor = -1;

        public static int BlockAt(LevelData level, Cell cell)
        {
            for (var i = 0; i < level.Blocks.Count; i++)
            {
                var block = level.Blocks[i];
                if (block.Shape.Contains(cell - block.Position))
                    return i;
            }
            return NoBlock;
        }

        public static int DoorAt(LevelData level, Cell cell)
        {
            if (!TryGetSide(level, cell, out var side))
                return NoDoor;
            var along = side.Along(cell);
            for (var i = 0; i < level.Doors.Count; i++)
            {
                var door = level.Doors[i];
                if (door.Side == side && door.Covers(along))
                    return i;
            }
            return NoDoor;
        }

        public static bool TryGetSide(LevelData level, Cell cell, out Side side)
        {
            var onColumn = cell.X >= 0 && cell.X < level.Width;
            var onRow = cell.Y >= 0 && cell.Y < level.Height;
            var inRingRow = cell.Y == -1 || cell.Y == level.Height;
            var inRingColumn = cell.X == -1 || cell.X == level.Width;
            side = cell.Y < 0 ? Side.Top : cell.Y >= level.Height ? Side.Bottom : cell.X < 0 ? Side.Left : Side.Right;
            return (inRingRow && onColumn) || (inRingColumn && onRow);
        }

        public static IEnumerable<Cell> BlockCells(BlockData block)
        {
            foreach (var offset in block.Shape)
                yield return block.Position + offset;
        }

        public static IEnumerable<Cell> DoorCells(LevelData level, DoorData door)
        {
            for (var along = door.Start; along < door.End; along++)
                yield return RingCell(level, door.Side, along);
        }

        private static Cell RingCell(LevelData level, Side side, int along) =>
            side == Side.Top ? new Cell(along, -1)
            : side == Side.Bottom ? new Cell(along, level.Height)
            : side == Side.Left ? new Cell(-1, along)
            : new Cell(level.Width, along);
    }
}
