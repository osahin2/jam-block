using System;
using System.Collections.Generic;
using Game.Data.Levels;
using Game.Logic.Levels;

namespace Game.Editor.LevelEditor
{
    public static class LevelProblemGuide
    {
        private static readonly Dictionary<LevelProblemKind, string> Explanations = new Dictionary<LevelProblemKind, string>
        {
            [LevelProblemKind.InvalidSize] = "The board needs at least one column and one row.",
            [LevelProblemKind.InvalidTimeLimit] = "The time limit must be at least 1 second.",
            [LevelProblemKind.NoBlocks] = "The level has no blocks. Draw one with the Block tool.",
            [LevelProblemKind.EmptyBlock] = "This block has no cells. Delete it.",
            [LevelProblemKind.BlockOutsideBoard] = "A cell of this block is outside the board.",
            [LevelProblemKind.BlocksOverlap] = "This block covers a cell that another block covers too.",
            [LevelProblemKind.DisconnectedBlock] =
                "This block is in pieces that do not touch. A block must be one piece: join the pieces or erase one.",
            [LevelProblemKind.DoorOutsideSide] = "This door runs past the end of its side.",
            [LevelProblemKind.DoorsOverlap] = "This door overlaps another door on the same side.",
            [LevelProblemKind.BlockCannotLeave] =
                "No door of this block's color is wide enough for it on a side it can move toward, so it can never leave. " +
                "Widen a door or add one; for a block with an arrow, a door must be at either end of the arrow.",
            [LevelProblemKind.IceNeverMelts] =
                "This block's ice never melts: ice drops by one for each block that leaves, and too few blocks can leave " +
                "before it. Lower its ice or add blocks without ice.",
            [LevelProblemKind.ArrowOffBlock] =
                "This block's arrow runs over cells the block does not have. Draw the arrow again with the Arrow tool."
        };

        private static readonly Dictionary<LevelPart, Func<LevelData, int, ILevelSelection>> Subjects =
            new Dictionary<LevelPart, Func<LevelData, int, ILevelSelection>>
            {
                [LevelPart.Block] = (level, index) => new BlockSelection(level.Blocks[index]),
                [LevelPart.Door] = (level, index) => new DoorSelection(level.Doors[index])
            };

        public static string Explain(LevelProblemKind kind) =>
            Explanations.TryGetValue(kind, out var explanation) ? explanation : kind.ToString();

        public static ILevelSelection SubjectOf(LevelProblem problem, LevelData level) =>
            Subjects.TryGetValue(problem.Part, out var create) ? create(level, problem.Index) : null;

        public static string ExplainAt(LevelReport report, LevelData level, Cell cell)
        {
            foreach (var problem in report.Problems)
            {
                var subject = SubjectOf(problem, level);
                if (subject == null)
                    continue;
                foreach (var subjectCell in subject.Cells(level))
                {
                    if (subjectCell == cell)
                        return Explain(problem.Kind);
                }
            }
            return null;
        }
    }
}
