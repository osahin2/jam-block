using Game.Data.Levels;
using Game.Logic.Play;

namespace Game.Logic.Levels
{
    public sealed class BlocksCanLeaveCheck : ILevelCheck
    {
        public void Check(LevelData level, LevelReport report)
        {
            for (var i = 0; i < level.Blocks.Count; i++)
            {
                if (!CanEverLeave(level.Blocks[i], level))
                    report.Add(LevelProblem.BlockWarning(LevelProblemKind.BlockCannotLeave, i));
            }
        }

        internal static bool EveryBlockFitsADoor(LevelData level)
        {
            foreach (var block in level.Blocks)
            {
                if (!CanEverLeave(block, level))
                    return false;
            }
            return true;
        }

        private static bool CanEverLeave(BlockData block, LevelData level) =>
            block.Shape.Count == 0 || FitsADoor(block, level);

        private static bool FitsADoor(BlockData block, LevelData level)
        {
            foreach (var door in level.Doors)
            {
                if (door.ColorId == block.ColorId && block.ExtentAlong(door.Side) <= door.Length &&
                    ArrowMechanic.AxisAllows(block.Arrow.Axis, Direction.OutOf(door.Side)))
                    return true;
            }
            return false;
        }
    }
}
