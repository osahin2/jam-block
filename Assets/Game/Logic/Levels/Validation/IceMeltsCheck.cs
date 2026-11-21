using System;
using Game.Data.Levels;

namespace Game.Logic.Levels
{
    public sealed class IceMeltsCheck : ILevelCheck
    {
        public void Check(LevelData level, LevelReport report)
        {
            var canLeave = BlocksThatCanLeave(level);
            for (var i = 0; i < level.Blocks.Count; i++)
            {
                if (level.Blocks[i].Ice > canLeave)
                    report.Add(LevelProblem.BlockError(LevelProblemKind.IceNeverMelts, i));
            }
        }

        private static int BlocksThatCanLeave(LevelData level)
        {
            var ice = new int[level.Blocks.Count];
            for (var i = 0; i < ice.Length; i++)
                ice[i] = level.Blocks[i].Ice;
            Array.Sort(ice);
            for (var i = 0; i < ice.Length; i++)
            {
                if (ice[i] > i)
                    return i;
            }
            return ice.Length;
        }
    }
}
