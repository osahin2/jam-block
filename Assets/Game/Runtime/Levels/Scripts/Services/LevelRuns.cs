using Core.Levels;

namespace Game.Levels
{
    public sealed class LevelRuns
    {
        private readonly LevelCatalog _catalog;
        private readonly ILevelProgression _progression;

        public LevelRuns(LevelCatalog catalog, ILevelProgression progression)
        {
            _catalog = catalog;
            _progression = progression;
        }

        public ILevelRun RunForPlayer() => new PlayerRun(_catalog, _progression);

        public ILevelRun RunForTest(LevelAsset level) => new TestRun(_catalog, _progression, level);
    }
}
