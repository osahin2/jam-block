namespace Game.Play
{
    public sealed class ArrowFeature : IBlockFeature
    {
        private readonly BoardArtSettings _art;

        public ArrowFeature(BoardArtSettings art)
        {
            _art = art;
        }

        public void Show(BlockView view, BlockOnBoard block) =>
            view.Part<BlockArrow>().Show(block.Mechanics.Arrow.ArrowOf(block.Board, block.Index), _art);

        public void CatchUp(BlockView view, BlockOnBoard block)
        {
        }
    }
}
