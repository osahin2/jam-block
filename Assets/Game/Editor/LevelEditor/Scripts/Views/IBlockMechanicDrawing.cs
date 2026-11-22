using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public interface IBlockMechanicDrawing
    {
        void DrawOnBlock(BlockData block, IBoardCanvas canvas);

        void DrawOnTop(BlockData block, IBoardCanvas canvas);
    }
}
