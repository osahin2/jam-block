namespace Game.Play
{
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
