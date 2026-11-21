using System;
using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Logic.Levels
{
    public static class LevelValidator
    {
        private const int Free = -1;

        public static LevelReport Validate(LevelData level)
        {
            var report = new LevelReport();
            if (level.Width < 1 || level.Height < 1)
            {
                report.Add(LevelProblem.LevelError(LevelProblemKind.InvalidSize));
                return report;
            }
            if (level.TimeLimitSeconds < 1)
                report.Add(LevelProblem.LevelError(LevelProblemKind.InvalidTimeLimit));
            if (level.Blocks.Count == 0)
                report.Add(LevelProblem.LevelError(LevelProblemKind.NoBlocks));
            CheckBlockCells(level, report);
            CheckDoors(level, report);
            CheckBlocksCanLeave(level, report);
            return report;
        }

        private static void CheckBlockCells(LevelData level, LevelReport report)
        {
            var owner = new int[level.Width * level.Height];
            Array.Fill(owner, Free);
            for (var i = 0; i < level.Blocks.Count; i++)
            {
                var block = level.Blocks[i];
                if (block.Shape.Count == 0)
                    report.Add(LevelProblem.BlockError(LevelProblemKind.EmptyBlock, i));
                else if (!IsConnected(block.Shape))
                    report.Add(LevelProblem.BlockError(LevelProblemKind.DisconnectedBlock, i));
                foreach (var offset in block.Shape)
                {
                    var cell = block.Position + offset;
                    if (!level.IsOnBoard(cell))
                    {
                        report.Add(LevelProblem.BlockError(LevelProblemKind.BlockOutsideBoard, i, cell));
                        continue;
                    }
                    var index = cell.IndexIn(level.Width);
                    if (owner[index] == Free)
                        owner[index] = i;
                    else
                        report.Add(LevelProblem.BlockError(LevelProblemKind.BlocksOverlap, i, cell));
                }
            }
        }

        private static void CheckDoors(LevelData level, LevelReport report)
        {
            for (var i = 0; i < level.Doors.Count; i++)
            {
                var door = level.Doors[i];
                if (!door.FitsSide(level.Width, level.Height))
                    report.Add(LevelProblem.DoorError(LevelProblemKind.DoorOutsideSide, i));
                if (OverlapsAnEarlierDoor(level, i))
                    report.Add(LevelProblem.DoorError(LevelProblemKind.DoorsOverlap, i));
            }
        }

        private static bool IsConnected(List<Cell> shape)
        {
            var reached = new HashSet<Cell> { shape[0] };
            var open = new Stack<Cell>();
            open.Push(shape[0]);
            while (open.Count > 0)
            {
                var cell = open.Pop();
                foreach (var neighbour in shape)
                {
                    if (cell.Touches(neighbour) && reached.Add(neighbour))
                        open.Push(neighbour);
                }
            }
            return reached.Count == new HashSet<Cell>(shape).Count;
        }

        private static bool OverlapsAnEarlierDoor(LevelData level, int index)
        {
            var door = level.Doors[index];
            for (var i = 0; i < index; i++)
            {
                var other = level.Doors[i];
                if (other.Overlaps(door.Side, door.Start, door.End))
                    return true;
            }
            return false;
        }

        private static void CheckBlocksCanLeave(LevelData level, LevelReport report)
        {
            for (var i = 0; i < level.Blocks.Count; i++)
            {
                var block = level.Blocks[i];
                if (block.Shape.Count > 0 && !FitsADoor(block, level))
                    report.Add(LevelProblem.BlockWarning(LevelProblemKind.BlockCannotLeave, i));
            }
        }

        private static bool FitsADoor(BlockData block, LevelData level)
        {
            foreach (var door in level.Doors)
            {
                if (door.ColorId == block.ColorId && block.ExtentAlong(door.Side) <= door.Length)
                    return true;
            }
            return false;
        }
    }
}
