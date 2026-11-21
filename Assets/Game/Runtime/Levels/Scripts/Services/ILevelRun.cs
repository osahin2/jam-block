namespace Game.Levels
{
    public interface ILevelRun
    {
        LevelAsset Level { get; }

        // The level's number as the player sees it.
        int Number { get; }

        // After a win: moves on to the next level; a run that keeps progress has saved it when this returns.
        void Advance();
    }
}
