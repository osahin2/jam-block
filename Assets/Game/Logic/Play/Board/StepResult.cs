using Game.Data.Levels;

namespace Game.Logic.Play
{
    public readonly struct StepResult
    {
        public const int NoDoor = -1;

        public bool Moved { get; }
        public Cell Position { get; }
        public int ExitDoor { get; }
        public bool HasLeft => ExitDoor != NoDoor;

        public StepResult(bool moved, Cell position, int exitDoor)
        {
            Moved = moved;
            Position = position;
            ExitDoor = exitDoor;
        }
    }
}
