namespace Game.Play
{
    // A level as shown on the board: the layout its views were placed with and one view per block, in level order.
    public readonly struct StagedLevel
    {
        public BoardLayout Layout { get; }
        public BlockView[] Blocks { get; }

        public StagedLevel(BoardLayout layout, BlockView[] blocks)
        {
            Layout = layout;
            Blocks = blocks;
        }
    }
}
