namespace Game.Play
{
    public interface IBlockFeature
    {
        void Show(BlockView view, BlockOnBoard block);

        void CatchUp(BlockView view, BlockOnBoard block);
    }
}
