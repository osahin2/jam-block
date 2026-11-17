namespace Game.Logic.Levels
{
    public enum LevelProblemKind
    {
        InvalidSize,
        InvalidTimeLimit,
        NoBlocks,
        EmptyBlock,
        BlockOutsideBoard,
        BlocksOverlap,
        DoorOutsideSide,
        BlockCannotLeave
    }
}
