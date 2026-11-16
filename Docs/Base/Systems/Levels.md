# Levels

> Summary: Level progression only: which level number the player is on, which index of the game's level list that maps to (looping after the last level), and recording a completion. It knows nothing about level content or flow.

## Code
- Folder: `Packages/com.base-setup.core/Runtime/Levels/`
- Assembly: `Core.Levels` (references `Core.Save`, and VContainer for the installer only)
- Entry points:
  - `ILevelProgression` ([ILevelProgression.cs](../../../Packages/com.base-setup.core/Runtime/Levels/ILevelProgression.cs)): `CurrentLevelNumber`, `CurrentLevelIndex`, `CompleteCurrentLevel()`.
  - `LevelLoop`: the game's level count and the index the content loops back to.
  - `RegisterCoreLevels(LevelLoop)` ([LevelsInstaller.cs](../../../Packages/com.base-setup.core/Runtime/Levels/LevelsInstaller.cs)); needs `ISaveService` registered (`RegisterCoreSave`).

## How it works
The only state is the number of completed levels, stored through `ISaveService` under the key `level-progress` and saved on every `CompleteCurrentLevel()`.

- `CurrentLevelNumber` = completed + 1. It keeps growing after the content runs out.
- `CurrentLevelIndex` = completed while that is below `LevelCount`; afterwards it cycles through `LoopStartIndex .. LevelCount - 1`.

Retrying a level needs nothing from this module: the game simply loads `CurrentLevelIndex` again.

## Rules & invariants
- `LoopStartIndex` is below `LevelCount`; the game passes valid values.
- Adding levels to the game later moves players who had looped back into the new levels, because the index is derived from the completed count, not stored.

## Tests
`Packages/com.base-setup.core/Tests/Levels/LevelProgressionTests.cs` (an in-memory `ISaveService` that round-trips JSON): fresh start, advancing, saving and restoring, and the loop rule for several loop starts.

## Related
- [Save](Save.md)
