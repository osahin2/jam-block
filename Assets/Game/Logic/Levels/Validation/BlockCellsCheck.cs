using System;
using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Logic.Levels
{
    public sealed class BlockCellsCheck : ILevelCheck
    {
        private const int Free = -1;

        public void Check(LevelData level, LevelReport report)
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
    }
}
