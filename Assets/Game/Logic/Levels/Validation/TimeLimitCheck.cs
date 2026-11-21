using Game.Data.Levels;

namespace Game.Logic.Levels
{
    public sealed class TimeLimitCheck : ILevelCheck
    {
        public void Check(LevelData level, LevelReport report)
        {
            if (level.TimeLimitSeconds < 1)
                report.Add(LevelProblem.LevelError(LevelProblemKind.InvalidTimeLimit));
        }
    }
}
