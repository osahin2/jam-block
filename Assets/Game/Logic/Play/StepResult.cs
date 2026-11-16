using Game.Data.Levels;

namespace Game.Logic.Play
{
    public readonly struct StepResult
    {
        public const int NoDoor = -1;

        public Cell Position { get; }
        public int ExitDoor { get; }
        public bool HasLeft => ExitDoor != NoDoor;

        public StepResult(Cell position, int exitDoor)
        {
            Position = position;
            ExitDoor = exitDoor;
        }
    }
}
