using Core.Levels;

namespace Game.Levels
{
    // A designer's test started from the level editor: it plays the tested level and then on through the catalog's
    // order (the first after the last), numbered by the catalog, and never moves the player's progress. A level
    // outside the catalog has no place of its own, so it takes the player's: their number, then their level.
    public sealed class TestRun : ILevelRun
    {
        private readonly LevelCatalog _catalog;
        private readonly ILevelProgression _progression;

        public LevelAsset Level { get; private set; }
        public int Number
        {
            get
            {
                var index = _catalog.IndexOf(Level);
                return index < 0 ? _progression.CurrentLevelNumber : index + 1;
            }
        }

        public TestRun(LevelCatalog catalog, ILevelProgression progression, LevelAsset level)
        {
            _catalog = catalog;
            _progression = progression;
            Level = level;
        }

        public void Advance()
        {
            var index = _catalog.IndexOf(Level);
            Level = index < 0
                ? _catalog.Levels[_progression.CurrentLevelIndex]
                : _catalog.Levels[(index + 1) % _catalog.Levels.Count];
        }
    }
}
