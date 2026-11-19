using Core.Levels;

namespace Game.Levels
{
    // Makes the run a Game scene plays: the player's own, whose wins move their progress, or a designer's test from
    // the level editor, which plays on through the catalog and leaves the progress alone. Which level comes next is
    // the run's.
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
