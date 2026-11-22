using System;
using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public static class IceEdits
    {
        public static void Set(BlockData block, int ice) => block.Ice = Math.Max(0, ice);
    }
}
