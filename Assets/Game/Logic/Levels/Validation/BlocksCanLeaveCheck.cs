using Game.Data.Levels;

namespace Game.Logic.Levels
{
    public sealed class BlocksCanLeaveCheck : ILevelCheck
    {
        public void Check(LevelData level, LevelReport report)
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
