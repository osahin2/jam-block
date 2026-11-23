using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public static class LevelEdits
    {
        public static bool TryAddBlock(LevelData level, int colorId, Cell cell, out int index)
        {
            index = LevelLookup.NoBlock;
            if (!level.IsOnBoard(cell) || LevelLookup.BlockAt(level, cell) != LevelLookup.NoBlock)
                return false;
            level.Blocks.Add(new BlockData { ColorId = colorId, Position = cell, Shape = new List<Cell> { new Cell(0, 0) } });
            index = level.Blocks.Count - 1;
            return true;
        }

        public static bool TryGrowBlock(LevelData level, int blockIndex, Cell cell)
        {
            if (!level.IsOnBoard(cell) || LevelLookup.BlockAt(level, cell) != LevelLookup.NoBlock)
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
                var other = LevelLookup.BlockAt(level, cell);
                if (other != LevelLookup.NoBlock && other != index)
                    return false;
            }
            block.Position = position;
            return true;
        }

        public static bool TryRemoveBlockCell(LevelData level, Cell cell)
        {
            var index = LevelLookup.BlockAt(level, cell);
            if (index == LevelLookup.NoBlock)
                return false;
            var block = level.Blocks[index];
            block.Shape.Remove(cell - block.Position);
            TidyAfterCellsRemoved(level, index);
            return true;
        }

        public static void RemoveBlock(LevelData level, BlockData block) => level.Blocks.Remove(block);

        public static bool TryAddDoor(LevelData level, int colorId, Side side, int along, out int index)
        {
            index = LevelLookup.NoDoor;
            if (!DoorFits(level, side, along, along + 1, LevelLookup.NoDoor))
                return false;
            level.Doors.Add(new DoorData { ColorId = colorId, Side = side, Start = along, Length = 1 });
            index = level.Doors.Count - 1;
            return true;
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

        public static void RemoveDoor(LevelData level, DoorData door) => level.Doors.Remove(door);

        public static void Resize(LevelData level, int width, int height)
        {
            level.Width = width;
            level.Height = height;
            TrimBlocksToTheBoard(level);
            level.Doors.RemoveAll(door => !door.FitsSide(width, height));
        }

        private static void TrimBlocksToTheBoard(LevelData level)
        {
            for (var i = level.Blocks.Count - 1; i >= 0; i--)
            {
                var block = level.Blocks[i];
                block.Shape.RemoveAll(offset => !level.IsOnBoard(block.Position + offset));
                TidyAfterCellsRemoved(level, i);
            }
        }

        private static void TidyAfterCellsRemoved(LevelData level, int index)
        {
            var block = level.Blocks[index];
            if (block.Shape.Count == 0)
                level.Blocks.RemoveAt(index);
            else
                ArrowEdits.RemoveIfOffShape(block);
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
