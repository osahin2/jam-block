# Board

> Summary: The move rules of a level in whole cells: whether a block can step one cell in a direction, the step itself, the exit through a door of the block's color, and whether the board is cleared. Plain C# in `Game.Logic`, built from a `LevelData`.

## Code
- Folder: `Assets/Game/Logic/Play/Board/`, `Assets/Game/Logic/Play/Drag/`
- Assembly: `Game.Logic`
- Entry points: `Board` (`Logic/Play/Board/Board.cs`), `BlockDrag` (`Logic/Play/Drag/BlockDrag.cs`); `Direction`, `StepResult`, `DragPose`, `BoardBlock` are their parameter and result types

## How it works
- `new Board(level)` copies the blocks into an occupancy grid (one block index or empty per cell).
- `CanStep(block, direction)`: every cell of the block, moved one cell, is inside the board and empty or the block's own. A block that has left cannot step.
- `Step(block, direction)` moves the block one cell when the step is free and returns a `StepResult`: `Moved`, the block's `Position` and `ExitDoor`, the index of the door it left through or `StepResult.NoDoor`. A step that is not free changes nothing and returns `Moved` false.
- `TryLeave(block, out exitDoor)` checks the block where it stands against the doors in level order. It leaves through the first door that has its color, lies on a side the block touches, spans the block's whole extent along that side, and has no other block between it and any of the block's cells, since the block slides out straight through it (an L whose arm touches the door stays while another block is under its other part). The drag direction does not matter. The block's cells are freed, `HasLeft` is set and `RemainingBlocks` drops, and it returns true with the door (false with `StepResult.NoDoor` when the block stays, and always for a block that has already left). `Step` calls it after every step, and the level attempt on every touch.
- `IsCleared` is true when every block has left.
- `BlockAt(cell)` returns the block on a cell, or `Board.NoBlock` for an empty cell or one outside the board.

## Drag
`BlockDrag` turns a finger position into whole-cell steps and a sub-cell offset, in cells from the board's top-left corner.
- `Begin(block, x, y)` keeps where the block was grabbed; `MoveTo(x, y)` moves the block toward where the finger wants its anchor, one step at a time, and returns a `DragPose`: the block, its cell, the offset in each axis and the exit door. `DragPose.None` is the pose of no move; `default` would read as having left through door 0. The pose names its block because a block that leaves ends the drag. `End()` stops the drag.
- A step is taken when the finger is more than half a cell past the block's cell; the offset follows the finger up to half a cell toward a free neighbour and stays 0 toward a blocked one.
- Steps alternate between the axes, so a finger past an obstacle takes the block around it; a fast move steps several cells in one call, never through a block.
- When a step makes the block leave, the pose reports the door and the drag ends.

## Rules & invariants
- Rules are whole cells; the sub-cell drag offset is the view's.
- A blocked step is an ordinary event: the finger can run ahead of a block that is stopped. `CanStep` is for the view's sub-cell offset; the decision is in `Step` either way.
- A door of another color is wall: steps past the board's edge are never free, so a block can only leave through its own door.
- Only the moved or touched block is checked for an exit: a block placed touching its door at the start, or held at its door by a block in the way that moved off later, leaves on the next tap, as in the reference; nothing leaves without the player touching it.
- `Step` and `CanStep` do not allocate.

## Tests
- `Tests/Play/Board/BoardStepTests`: free and blocked steps, edges, other blocks, L shapes, a blocked step that changes nothing.
- `Tests/Play/Drag/BlockDragTests`: see [Play](Play.md).
- `Tests/Play/Board/BoardExitTests`: exit through a door on each side (which also checks `Direction`'s table indexed by `Side`), exit on touch from any direction, other color is wall, partial cover stays, extent along the side only, L shapes, an L held by a block between its other part and the door, `TryLeave` for a block at its door, away from it, after the block in its way moved off and for a block that has already left, two doors of one color, cleared after the last block.

## Related
- [Level attempt](LevelAttempt.md), which the play feature uses instead of `Board` and `BlockDrag` directly
- [Levels](Levels.md), [Reference](../Reference.md), [Decisions](../Decisions.md)
