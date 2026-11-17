using Game.Data.Levels;

namespace Game.Logic.Play
{
    public readonly struct DragPose
    {
        public int Block { get; }
        public Cell Position { get; }
        // Sub-cell offset in cells, toward a free neighbour only and at most half a cell.
        public float OffsetX { get; }
        public float OffsetY { get; }
        public int ExitDoor { get; }
        public bool HasLeft => ExitDoor != StepResult.NoDoor;

        public DragPose(int block, Cell position, float offsetX, float offsetY, int exitDoor)
        {
            Block = block;
            Position = position;
            OffsetX = offsetX;
            OffsetY = offsetY;
            ExitDoor = exitDoor;
        }
    }
}
