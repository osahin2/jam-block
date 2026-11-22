# Levels

> Level progression only: the player's level number, the index in the game's level list it maps to (looping after the last), and recording a completion.

**Code:** `Core.Levels`. **Entry:** `ILevelProgression` (`CurrentLevelNumber`, `CurrentLevelIndex`, `CompleteCurrentLevel`); `RegisterCoreLevels(LevelLoop)`.

## Rules
- The only state is the completed count, saved on every completion. Number = completed + 1; the index cycles through `LoopStartIndex..LevelCount-1` once content runs out.
- The index is derived, not stored, so levels added later reach players who had looped.
