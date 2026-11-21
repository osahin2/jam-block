using Game.Data.Levels;

namespace Game.Logic.Levels
{
    public sealed class HasBlocksCheck : ILevelCheck
    {
        public void Check(LevelData level, LevelReport report)
        {
            if (level.Blocks.Count == 0)
                report.Add(LevelProblem.LevelError(LevelProblemKind.NoBlocks));
        }
    }
}
