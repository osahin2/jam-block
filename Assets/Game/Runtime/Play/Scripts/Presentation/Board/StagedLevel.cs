namespace Game.Play
{
    // A level as shown on the board: the layout its views were placed with, one view per block and one per door, each
    // in level order.
    public readonly struct StagedLevel
    {
        public BoardLayout Layout { get; }
        public BlockView[] Blocks { get; }
        public DoorView[] Doors { get; }

        public StagedLevel(BoardLayout layout, BlockView[] blocks, DoorView[] doors)
        {
            Layout = layout;
            Blocks = blocks;
            Doors = doors;
        }
    }
}
