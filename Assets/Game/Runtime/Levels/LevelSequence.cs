using Core.Levels;
using Game.Levels.Settings;

namespace Game.Levels
{
    // Which level comes next. The player's progress moves only when they win the level they are on; a level played
    // from the level editor leaves it alone and continues with the catalog's next level, or with the player's level
    // when it is not in the catalog.
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

        // Records the win of the given level and returns the level to play next.
        public LevelAsset Advance(LevelAsset won)
        {
            if (won == Current)
            {
                _progression.CompleteCurrentLevel();
                return Current;
            }
            var index = IndexOf(won);
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
