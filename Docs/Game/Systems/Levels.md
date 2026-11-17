# Levels

> Summary: The level format shared by the level editor and the game: board size, blocks with shape and color, doors on the board's sides, time limit. Plain C# types in `Game.Data`, stored in one `LevelAsset` ScriptableObject per level, checked by `LevelValidator`.

## Code
- Folder: `Assets/Game/Data/Levels/` (format), `Assets/Game/Logic/Levels/` (validation), `Assets/Game/Runtime/Levels/` (asset)
- Assembly: `Game.Data` (`noEngineReferences: true`), `Game.Logic`, `Game`
- Entry points: `LevelData` (`Data/Levels/LevelData.cs`), `LevelValidator` (`Logic/Levels/LevelValidator.cs`), `LevelAsset` (`Runtime/Levels/LevelAsset.cs`)

## How it works
| Type | Fields |
|---|---|
| `LevelData` | `Width`, `Height` in cells; `TimeLimitSeconds`; `Blocks`; `Doors`; `IsOnBoard(cell)` |
| `BlockData` | `ColorId`; `Position`, the block's anchor cell; `Shape`, its cells relative to `Position` |
| `DoorData` | `ColorId`; `Side` (`Top`, `Bottom`, `Left`, `Right`); `Start`, the first covered cell along the side; `Length` in cells; `End` and `Covers(along)`, the one place that says which cells a door covers; `FitsSide(width, height)`, `Overlaps(side, start, end)` and `SpanFits`, the one place for the door rules the validator and the level editor both apply |
| `Cell` | `X` column from the left, `Y` row from the top; value equality, `+`, `-`, `Touches` (shares an edge), `IsInside(width, height)` and `IndexIn(width)`, used by the board and the validator alike |

- A shape is any set of cells: rectangles and L shapes are the same type.
- `Start` counts columns on `Top` and `Bottom`, rows on `Left` and `Right` (`SideExtensions.Along`).
- A color is an id; the game's color settings map ids to materials.
- `LevelAsset` holds one `LevelData` in a serialized field; the editor writes it, the play session reads `Level` and builds its own state from it.

## Validation
`LevelValidator.Validate(level)` returns a `LevelReport`: every `LevelProblem` found (kind, severity, the part it is about as a `LevelPart` — the level, a block or a door — with that block's or door's index, and the cell at fault when there is one) and `HasErrors`.

| Kind | Severity | Means |
|---|---|---|
| `InvalidSize` | error | width or height below 1; nothing else is checked |
| `InvalidTimeLimit` | error | time limit below one second |
| `NoBlocks` | error | a level without blocks (it is won when its last block leaves, so it could only run out of time) |
| `EmptyBlock` | error | a block without cells (it could never leave, the level never ends) |
| `BlockOutsideBoard` | error | a block cell outside the board, with the cell |
| `BlocksOverlap` | error | a block cell already taken by an earlier block, with the cell |
| `DisconnectedBlock` | error | a block whose cells are not one group of edge-touching cells (it is moved and built as one piece) |
| `DoorOutsideSide` | error | a door shorter than one cell or past the end of its side |
| `DoorsOverlap` | error | a door covering a cell of the same side that an earlier door covers |
| `BlockStartsAtItsDoor` | error | a block that already touches a covering door of its color, so it would leave before the player moves anything; decided by the board's own exit rule, checked only when the level has no other errors |
| `BlockCannotLeave` | warning | no door of the block's color is as wide as the block's extent along the door's side |

- Errors make a level unplayable and are not saved; warnings are shown but do not stop saving.
- `BlockCannotLeave` checks widths only; whether the block can reach the door is the solver's question (phase 3).
- The level editor ([Level editor](LevelEditor.md)) shows the report and saves only without errors; `ProjectLevelsTests` fails when any `LevelAsset` in the project has an error.

## Rules & invariants
- The data types have public fields: Unity serializes them without an engine reference. Nothing changes them at runtime; the logic copies what it needs.
- What a valid level is lives only in `LevelValidator`. Levels are checked when they are made, not when they are played.
- `Side` values are stored as ints in assets: never reorder them.

## Tests
- `Tests/Levels/LevelAssetTests`: a level survives Unity serialization.
- `Tests/Levels/LevelValidatorTests`: each problem kind (door without cells or before its side included), warnings versus errors, every problem reported by kind.
- `Tests/Levels/DoorDataTests`: a door fits its side and overlaps only on shared cells of the same side.
- `Tests/Levels/ProjectLevelsTests`: there are level assets, every one has no errors, and every color id in them has a material in the `ColorPalette` (`ColorPalette.Has`, the one check the editor uses too).
- `Tests/Levels/LevelBuilder`: builds levels for the tests.

## Related
- [Board](Board.md), [Decisions](../Decisions.md)
