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
                CheckShape(level.Blocks[i], i, report);
                CheckCells(level, i, owner, report);
            }
        }

        private static void CheckShape(BlockData block, int index, LevelReport report)
        {
            if (block.Shape.Count == 0)
                report.Add(LevelProblem.BlockError(LevelProblemKind.EmptyBlock, index));
            else if (!IsConnected(block.Shape))
                report.Add(LevelProblem.BlockError(LevelProblemKind.DisconnectedBlock, index));
        }

        private static void CheckCells(LevelData level, int index, int[] owner, LevelReport report)
        {
            var block = level.Blocks[index];
            foreach (var offset in block.Shape)
            {
                var cell = block.Position + offset;
                if (!level.IsOnBoard(cell))
                    report.Add(LevelProblem.BlockError(LevelProblemKind.BlockOutsideBoard, index, cell));
                else if (!TryClaim(owner, cell.IndexIn(level.Width), index))
                    report.Add(LevelProblem.BlockError(LevelProblemKind.BlocksOverlap, index, cell));
            }
        }

        private static bool TryClaim(int[] owner, int cellIndex, int block)
        {
            if (owner[cellIndex] != Free)
                return false;
            owner[cellIndex] = block;
            return true;
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
