namespace Game.Logic.Levels
{
    public enum LevelProblemKind
    {
        InvalidSize,
        InvalidTimeLimit,
        EmptyBlock,
        BlockOutsideBoard,
        BlocksOverlap,
        DoorOutsideSide,
        BlockCannotLeave
    }
}
