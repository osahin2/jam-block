using Core.Levels;

namespace Game.Levels
{
    // Which level comes next. The player's progress moves only when they win the level they are on in their own run;
    // a run started from the level editor is a designer's test that plays on through the catalog and leaves the
    // progress alone.
    public sealed class LevelSequence
    {
        private readonly LevelCatalog _catalog;
        private readonly ILevelProgression _progression;

        public LevelAsset Current => _catalog.Levels[_progression.CurrentLevelIndex];

        public LevelSequence(LevelCatalog catalog, ILevelProgression progression)
        {
            _catalog = catalog;
            _progression = progression;
        }

        // The player won their current level: records it and returns the new current level.
        public LevelAsset CompleteCurrent()
        {
            _progression.CompleteCurrentLevel();
            return Current;
        }

        // The catalog's level after the given one (the first after the last), or the player's current level when the
        // given one is not in the catalog. The player's progress stays as it is.
        public LevelAsset After(LevelAsset level)
        {
            var index = IndexOf(level);
            return index < 0 ? Current : _catalog.Levels[(index + 1) % _catalog.Levels.Count];
        }

        private int IndexOf(LevelAsset level)
        {
            for (var i = 0; i < _catalog.Levels.Count; i++)
            {
                if (_catalog.Levels[i] == level)
                    return i;
            }
            return -1;
        }
    }
}
