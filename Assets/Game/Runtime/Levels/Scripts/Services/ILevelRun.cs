namespace Game.Levels
{
    public interface ILevelRun
    {
        LevelAsset Level { get; }

        // The level's number as the player sees it.
        int Number { get; }

        void Advance();
    }
}
