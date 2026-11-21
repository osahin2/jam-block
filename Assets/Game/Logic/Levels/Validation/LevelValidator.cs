using Game.Data.Levels;

namespace Game.Logic.Levels
{
    public static class LevelValidator
    {
        private static readonly ILevelCheck[] Checks =
        {
            new TimeLimitCheck(),
            new HasBlocksCheck(),
            new BlockCellsCheck(),
            new DoorPlacementCheck(),
            new BlocksCanLeaveCheck()
        };

        public static LevelReport Validate(LevelData level)
        {
            var report = new LevelReport();
            if (level.Width < 1 || level.Height < 1)
            {
                report.Add(LevelProblem.LevelError(LevelProblemKind.InvalidSize));
                return report;
            }
            foreach (var check in Checks)
                check.Check(level, report);
            return report;
        }
    }
}
