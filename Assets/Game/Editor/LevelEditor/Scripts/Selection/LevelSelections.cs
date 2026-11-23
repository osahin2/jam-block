using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public static class LevelSelections
    {
        public static ILevelSelection At(LevelData level, Cell cell)
        {
            var block = LevelLookup.BlockAt(level, cell);
            if (block != LevelLookup.NoBlock)
                return new BlockSelection(level.Blocks[block]);
            var door = LevelLookup.DoorAt(level, cell);
            return door != LevelLookup.NoDoor ? new DoorSelection(level.Doors[door]) : null;
        }
    }
}
