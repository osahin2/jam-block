using Game.Data.Levels;

namespace Game.Logic.Play
{
    public readonly struct DragPose
    {
        public static readonly DragPose None = new DragPose(Board.NoBlock, default, 0f, 0f, StepResult.NoDoor, false);

        public int Block { get; }
        public Cell Position { get; }
        public float OffsetX { get; }
        public float OffsetY { get; }
        public int ExitDoor { get; }
        public bool IsLocked { get; }
        public bool HasLeft => ExitDoor != StepResult.NoDoor;

        private DragPose(int block, Cell position, float offsetX, float offsetY, int exitDoor, bool isLocked)
        {
            Block = block;
            Position = position;
            OffsetX = offsetX;
            OffsetY = offsetY;
            ExitDoor = exitDoor;
            IsLocked = isLocked;
        }

        public static DragPose At(int block, Cell position, float offsetX, float offsetY) =>
            new DragPose(block, position, offsetX, offsetY, StepResult.NoDoor, false);

        public static DragPose Left(int block, Cell position, int exitDoor) =>
            new DragPose(block, position, 0f, 0f, exitDoor, false);

        public static DragPose Locked(int block, Cell position) =>
            new DragPose(block, position, 0f, 0f, StepResult.NoDoor, true);
    }
}
