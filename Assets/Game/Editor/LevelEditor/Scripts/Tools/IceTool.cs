using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public sealed class IceTool : ILevelEditorTool
    {
        public string Name => "Ice";
        public string Tooltip => "Click a block to add one ice, right-click to take one off.";

        public string Describe(LevelData level, Cell cell)
        {
            var index = LevelLookup.BlockAt(level, cell);
            if (index == LevelLookup.NoBlock)
                return "Nothing to ice here. Ice goes on blocks.";
            var ice = level.Blocks[index].Ice;
            return ice > 0
                ? $"Block with {ice} ice: click to add one, right-click to take one off."
                : "Block: click to add one ice. It stays locked until that many blocks have left.";
        }

        public bool Press(LevelData level, Cell cell, int colorId) => ChangeIce(level, cell, +1);

        public bool Drag(LevelData level, Cell cell) => false;

        public void Release()
        {
        }

        public bool RightClick(LevelData level, Cell cell) => ChangeIce(level, cell, -1);

        private static bool ChangeIce(LevelData level, Cell cell, int change)
        {
            var index = LevelLookup.BlockAt(level, cell);
            if (index == LevelLookup.NoBlock)
                return false;
            var block = level.Blocks[index];
            var before = block.Ice;
            IceEdits.Set(block, before + change);
            return block.Ice != before;
        }
    }
}
