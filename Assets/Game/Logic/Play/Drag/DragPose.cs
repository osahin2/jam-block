using Game.Data.Levels;

namespace Game.Logic.Play
{
    public readonly struct DragPose
    {
        // No block moved; use it instead of default, where ExitDoor 0 would read as having left through door 0.
        public static readonly DragPose None = new DragPose(Board.NoBlock, default, 0f, 0f, StepResult.NoDoor);

        public int Block { get; }
        public Cell Position { get; }
        // In cells, toward a free neighbour only, at most half a cell.
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

        public static DragPose Left(int block, Cell position, int exitDoor) => new DragPose(block, position, 0f, 0f, exitDoor);
    }
}
