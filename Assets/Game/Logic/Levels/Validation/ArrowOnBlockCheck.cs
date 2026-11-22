using Game.Data.Levels;

namespace Game.Logic.Levels
{
    public sealed class ArrowOnBlockCheck : ILevelCheck
    {
        public void Check(LevelData level, LevelReport report)
        {
            for (var i = 0; i < level.Blocks.Count; i++)
            {
                var arrow = level.Blocks[i].Arrow;
                if (arrow.IsSet && !arrow.LiesOn(level.Blocks[i].Shape))
                    report.Add(LevelProblem.BlockError(LevelProblemKind.ArrowOffBlock, i));
            }
        }
    }
}
