namespace Game.Levels
{
    public interface ILevelRun
    {
        LevelAsset Level { get; }

        int Number { get; }

        void Advance();
    }
}
