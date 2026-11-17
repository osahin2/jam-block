using System;
using Game.Data.Levels;

namespace Game.Logic.Levels
{
    // The one place that says what a valid level is: the level editor shows the report before saving, and a test
    // checks every level asset in the project. The game plays a level as given.
    public static class LevelValidator
    {
        private const int Free = -1;

        public static LevelReport Validate(LevelData level)
        {
            var report = new LevelReport();
            if (level.Width < 1 || level.Height < 1)
            {
                report.Add(LevelProblem.Error(LevelProblemKind.InvalidSize));
                return report;
            }
            if (level.TimeLimitSeconds < 1)
                report.Add(LevelProblem.Error(LevelProblemKind.InvalidTimeLimit));
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
                    report.Add(LevelProblem.Error(LevelProblemKind.EmptyBlock, i));
                foreach (var offset in block.Shape)
                {
                    var cell = block.Position + offset;
                    if (cell.X < 0 || cell.X >= level.Width || cell.Y < 0 || cell.Y >= level.Height)
                    {
                        report.Add(LevelProblem.Error(LevelProblemKind.BlockOutsideBoard, i, cell));
                        continue;
                    }
                    var index = cell.Y * level.Width + cell.X;
                    if (owner[index] == Free)
                        owner[index] = i;
                    else
                        report.Add(LevelProblem.Error(LevelProblemKind.BlocksOverlap, i, cell));
                }
            }
        }

        private static void CheckDoors(LevelData level, LevelReport report)
        {
            for (var i = 0; i < level.Doors.Count; i++)
            {
                var door = level.Doors[i];
                var sideLength = door.Side.Length(level.Width, level.Height);
                if (door.Length < 1 || door.Start < 0 || door.End > sideLength)
                    report.Add(LevelProblem.Error(LevelProblemKind.DoorOutsideSide, i));
            }
        }

        // Only whether some door of the block's color is wide enough; whether the block can reach it is a solver's job.
        private static void CheckBlocksCanLeave(LevelData level, LevelReport report)
        {
            for (var i = 0; i < level.Blocks.Count; i++)
            {
                var block = level.Blocks[i];
                if (block.Shape.Count > 0 && !FitsADoor(block, level))
                    report.Add(LevelProblem.Warning(LevelProblemKind.BlockCannotLeave, i));
            }
        }

        private static bool FitsADoor(BlockData block, LevelData level)
        {
            foreach (var door in level.Doors)
            {
                if (door.ColorId == block.ColorId && ExtentAlong(door.Side, block) <= door.Length)
                    return true;
            }
            return false;
        }

        private static int ExtentAlong(Side side, BlockData block)
        {
            var min = int.MaxValue;
            var max = int.MinValue;
            foreach (var cell in block.Shape)
            {
                var along = side.Along(cell);
                min = Math.Min(min, along);
                max = Math.Max(max, along);
            }
            return max - min + 1;
        }
    }
}
