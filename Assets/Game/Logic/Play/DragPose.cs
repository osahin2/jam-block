using Game.Data.Levels;

namespace Game.Logic.Play
{
    public readonly struct DragPose
    {
        public Cell Position { get; }
        // Sub-cell offset in cells, toward a free neighbour only and at most half a cell.
        public float OffsetX { get; }
        public float OffsetY { get; }
        public int ExitDoor { get; }
        public bool HasLeft => ExitDoor != StepResult.NoDoor;

        public DragPose(Cell position, float offsetX, float offsetY, int exitDoor)
        {
            Position = position;
            OffsetX = offsetX;
            OffsetY = offsetY;
            ExitDoor = exitDoor;
        }
    }
}
