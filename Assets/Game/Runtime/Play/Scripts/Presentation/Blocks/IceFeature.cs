namespace Game.Play
{
    public sealed class IceFeature : IBlockFeature
    {
        private readonly IceSettings _settings;
        private readonly BoardArtSettings _art;

        public IceFeature(IceSettings settings, BoardArtSettings art)
        {
            _settings = settings;
            _art = art;
        }

        public void Show(BlockView view, BlockOnBoard block) =>
            view.Part<IceCover>().Show(IceLeft(block), view, _settings, _art);

        public void CatchUp(BlockView view, BlockOnBoard block) => view.Part<IceCover>().CatchUp(IceLeft(block));

        private static int IceLeft(BlockOnBoard block) => block.Mechanics.Ice.IceLeft(block.Board, block.Index);
    }
}
