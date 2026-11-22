using Core.Levels;

namespace Game.Levels
{
    public sealed class PlayerRun : ILevelRun
    {
        private readonly LevelCatalog _catalog;
        private readonly ILevelProgression _progression;

        public LevelAsset Level => _catalog.Levels[_progression.CurrentLevelIndex];
        public int Number => _progression.CurrentLevelNumber;

        public PlayerRun(LevelCatalog catalog, ILevelProgression progression)
        {
            _catalog = catalog;
            _progression = progression;
        }

        public void Advance() => _progression.CompleteCurrentLevel();
    }
}
