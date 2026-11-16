# Board

> Summary: The move rules of a level in whole cells: whether a block can step one cell in a direction, the step itself, the exit through a door of the block's color, and whether the board is cleared. Plain C# in `Game.Logic`, built from a `LevelData`.

## Code
- Folder: `Assets/Game/Logic/Play/`
- Assembly: `Game.Logic`
- Entry points: `Board` (`Logic/Play/Board.cs`); `Direction`, `StepResult`, `BoardBlock` are its parameter and result types

## How it works
- `new Board(level)` copies the blocks into an occupancy grid (one block index or empty per cell).
- `CanStep(block, direction)`: every cell of the block, moved one cell, is inside the board and empty or the block's own. A block that has left cannot step.
- `Step(block, direction)` moves the block one cell when the step is free and returns a `StepResult`: `Moved`, the block's `Position` and `ExitDoor`, the index of the door it left through or `StepResult.NoDoor`. A step that is not free changes nothing and returns `Moved` false.
- After each step the moved block is checked against the doors in level order. It leaves through the first door that has its color, lies on a side the block touches, and spans the block's whole extent along that side. The drag direction does not matter. The block's cells are freed, `HasLeft` is set and `RemainingBlocks` drops.
- `IsCleared` is true when every block has left.

## Rules & invariants
- Rules are whole cells; the sub-cell drag offset is the view's.
- A blocked step is an ordinary event: the finger can run ahead of a block that is stopped. `CanStep` is for the view's sub-cell offset; the decision is in `Step` either way.
- A door of another color is wall: steps past the board's edge are never free, so a block can only leave through its own door.
- Only the moved block is checked for an exit; a block placed touching its door at the start does not leave until it moves.
- `Step` and `CanStep` do not allocate.

## Tests
- `Tests/Play/BoardStepTests`: free and blocked steps, edges, other blocks, L shapes, a blocked step that changes nothing.
- `Tests/Play/BoardExitTests`: exit on touch from any direction, other color is wall, partial cover stays, extent along the side only, L shapes, two doors of one color, cleared after the last block.

## Related
- [Levels](Levels.md), [Reference](../Reference.md), [Decisions](../Decisions.md)
