using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public static class LevelEdits
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

        public static Cell RingCell(LevelData level, Side side, int along) =>
            side == Side.Top ? new Cell(along, -1)
            : side == Side.Bottom ? new Cell(along, level.Height)
            : side == Side.Left ? new Cell(-1, along)
            : new Cell(level.Width, along);

        public static IEnumerable<Cell> BlockCells(LevelData level, int index)
        {
            var block = level.Blocks[index];
            foreach (var offset in block.Shape)
                yield return block.Position + offset;
        }

        public static IEnumerable<Cell> DoorCells(LevelData level, int index)
        {
            var door = level.Doors[index];
            for (var along = door.Start; along < door.End; along++)
                yield return RingCell(level, door.Side, along);
        }

        public static int AddBlock(LevelData level, int colorId, Cell cell)
        {
            level.Blocks.Add(new BlockData { ColorId = colorId, Position = cell, Shape = new List<Cell> { new Cell(0, 0) } });
            return level.Blocks.Count - 1;
        }

        public static bool TryGrowBlock(LevelData level, int blockIndex, Cell cell)
        {
            if (!level.IsOnBoard(cell) || BlockAt(level, cell) != NoBlock)
                return false;
            var block = level.Blocks[blockIndex];
            var offset = cell - block.Position;
            foreach (var own in block.Shape)
            {
                if (own.Touches(offset))
                {
                    block.Shape.Add(offset);
                    return true;
                }
            }
            return false;
        }

        public static bool TryMoveBlock(LevelData level, int index, Cell position)
        {
            var block = level.Blocks[index];
            if (block.Position == position)
                return false;
            foreach (var offset in block.Shape)
            {
                var cell = position + offset;
                if (!level.IsOnBoard(cell))
                    return false;
                var other = BlockAt(level, cell);
                if (other != NoBlock && other != index)
                    return false;
            }
            block.Position = position;
            return true;
        }

        public static bool TryRemoveBlockCell(LevelData level, Cell cell)
        {
            var index = BlockAt(level, cell);
            if (index == NoBlock)
                return false;
            var block = level.Blocks[index];
            block.Shape.Remove(cell - block.Position);
            if (block.Shape.Count == 0)
                level.Blocks.RemoveAt(index);
            return true;
        }

        public static int AddDoor(LevelData level, int colorId, Side side, int along)
        {
            if (!DoorFits(level, side, along, along + 1, NoDoor))
                return NoDoor;
            level.Doors.Add(new DoorData { ColorId = colorId, Side = side, Start = along, Length = 1 });
            return level.Doors.Count - 1;
        }

        public static bool TrySetDoorSpan(LevelData level, int index, int start, int end)
        {
            var door = level.Doors[index];
            if ((door.Start == start && door.End == end) || !DoorFits(level, door.Side, start, end, index))
                return false;
            door.Start = start;
            door.Length = end - start;
            return true;
        }

        public static void Resize(LevelData level, int width, int height)
        {
            level.Width = width;
            level.Height = height;
            for (var i = level.Blocks.Count - 1; i >= 0; i--)
            {
                var block = level.Blocks[i];
                block.Shape.RemoveAll(offset => !level.IsOnBoard(block.Position + offset));
                if (block.Shape.Count == 0)
                    level.Blocks.RemoveAt(i);
            }
            level.Doors.RemoveAll(door => !door.FitsSide(width, height));
        }

        private static bool DoorFits(LevelData level, Side side, int start, int end, int ignoredDoor)
        {
            if (!DoorData.SpanFits(side, start, end, level.Width, level.Height))
                return false;
            for (var i = 0; i < level.Doors.Count; i++)
            {
                if (i != ignoredDoor && level.Doors[i].Overlaps(side, start, end))
                    return false;
            }
            return true;
        }
    }
}
