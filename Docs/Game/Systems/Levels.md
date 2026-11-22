# Levels

> The level format the editor writes and the game reads: board size, blocks (shape, color, ice, arrow), doors on the sides, time limit. Plain types in `Game.Data`, one `LevelAsset` per level, checked by `LevelValidator`.

**Code:** `Data/Levels/` (format), `Logic/Levels/Validation/` and `Report/`, `Runtime/Levels/`. **Entry:** `LevelData`, `LevelValidator.Validate(level)` (returns a `LevelReport`), `LevelAsset`.

## Rules
- `Cell` is X from the left, Y from the top. A door's `Start` counts columns on top and bottom, rows on left and right. `DoorData.Covers`, `FitsSide` and `Overlaps` are the one place for door geometry, used by the validator and the editor.
- Enums stored as ints in assets (`Side`, `ArrowAxis`) are never reordered.
- What a valid level is lives only in `LevelValidator`: a list of `ILevelCheck`s run after the size check. A mechanic adds its rule as one check. Errors block saving; warnings (`BlockCannotLeave`) only show. Levels are checked when made, not when played; a test fails on any level asset with an error.
- The data has public fields so Unity serializes it without an engine reference; nothing changes it at run time, the logic copies what it needs.

Related: [Board](Board.md), [Level editor](LevelEditor.md)
