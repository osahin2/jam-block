using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public static class ArrowEdits
    {
        public static bool Set(BlockData block, ArrowAxis axis, Cell start, int length)
        {
            var arrow = block.Arrow;
            if (arrow.Axis == axis && arrow.Start == start && arrow.Length == length)
                return false;
            block.Arrow = new ArrowData { Axis = axis, Start = start, Length = length };
            return true;
        }

        public static bool Remove(BlockData block)
        {
            if (!block.Arrow.IsSet)
                return false;
            block.Arrow = new ArrowData();
            return true;
        }

        public static void RemoveIfOffShape(BlockData block)
        {
            if (block.Arrow.IsSet && !block.Arrow.LiesOn(block.Shape))
                Remove(block);
        }
    }
}
