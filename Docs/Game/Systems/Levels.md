# Levels

> Summary: The level format shared by the level editor and the game: board size, blocks with shape and color, doors on the board's sides, time limit. Plain C# types in `Game.Data`, stored in one `LevelAsset` ScriptableObject per level.

## Code
- Folder: `Assets/Game/Data/Levels/` (format), `Assets/Game/Runtime/Levels/` (asset)
- Assembly: `Game.Data` (`noEngineReferences: true`), `Game`
- Entry points: `LevelData` (`Data/Levels/LevelData.cs`), `LevelAsset` (`Runtime/Levels/LevelAsset.cs`)

## How it works
| Type | Fields |
|---|---|
| `LevelData` | `Width`, `Height` in cells; `TimeLimitSeconds`; `Blocks`; `Doors` |
| `BlockData` | `ColorId`; `Position`, the block's anchor cell; `Shape`, its cells relative to `Position` |
| `DoorData` | `ColorId`; `Side` (`Top`, `Bottom`, `Left`, `Right`); `Start`, the first covered cell along the side; `Length` in cells |
| `Cell` | `X` column from the left, `Y` row from the top; value equality and `+` |

- A shape is any set of cells: rectangles and L shapes are the same type.
- `Start` counts columns on `Top` and `Bottom`, rows on `Left` and `Right`.
- A color is an id; the game's color settings map ids to materials.
- `LevelAsset` holds one `LevelData` in a serialized field; the editor writes it, the play session reads `Level` and builds its own state from it.

## Rules & invariants
- The data types have public fields: Unity serializes them without an engine reference. Nothing changes them at runtime; the logic copies what it needs.
- Validity (blocks inside the board and not overlapping, doors within their side) is checked by the logic's `Board` when it is built; the editor will warn before saving.
- `Side` values are stored as ints in assets: never reorder them.

## Tests
- `Tests/Levels/LevelAssetTests`: a level survives Unity serialization.
- `Tests/Play/LevelBuilder`: builds levels for the rule tests.

## Related
- [Board](Board.md), [Decisions](../Decisions.md)
