# Board

> Summary: The move rules of a level in whole cells: whether a block can step one cell in a direction, the step itself, the exit through a door of the block's color, and whether the board is cleared. Plain C# in `Game.Logic`, built from a `LevelData`.

## Code
- Folder: `Assets/Game/Logic/Play/Board/`, `Assets/Game/Logic/Play/Drag/`
- Assembly: `Game.Logic`
- Folder: `Assets/Game/Logic/Play/Mechanics/` (the mechanics plugged into the board)
- Entry points: `Board` (`Logic/Play/Board/Board.cs`), `BlockDrag` (`Logic/Play/Drag/BlockDrag.cs`), `LevelMechanics.For(level)` (`Logic/Play/Board/LevelMechanics.cs`); `Direction`, `StepResult`, `DragPose`, `BoardBlock` (its `Shape` read-only outside `Game.Logic`) are their parameter and result types; `IMoveRule` and `IBlockGuard` are the board's roles; `IReadOnlyBoard` (read) and `IBoard` (read and hit) are what the board shows those outside it

## How it works
- `new Board(level, rules, guards)` copies the blocks into a list and an occupancy grid (one block index or empty per cell). The list has room for one block per cell of the level's blocks, so blocks appended at run time (a split) never grow it. The rules and guards come from `LevelMechanics.For(level)`.
- `CanStep(block, direction)`: every move rule allows the step, and every cell of the block, moved one cell, is inside the board and empty or the block's own. A block that is gone cannot step.
- `Step(block, direction)` moves the block one cell when the step is free and returns a `StepResult`: `Moved`, the block's `Position` and `ExitDoor`, the index of the door it left through or `StepResult.NoDoor`. A step that is not free changes nothing and returns `Moved` false.
- `TryLeave(block, out exitDoor)` checks the block where it stands against the doors in level order. It leaves through the first door that has its color, lies on a side the block touches, spans the block's whole extent along that side, is allowed by every move rule asked with the direction out through the door, and has no other block between it and any of the block's cells, since the block slides out straight through it (an L whose arm touches the door stays while another block is under its other part). The drag direction does not matter. The block is removed (below) and it returns true with the door (false with `StepResult.NoDoor` when the block stays, and always for a block that is already gone). `Step` calls it after every step, and the level attempt on every touch.
- `Hit(block)` is a booster's hit on a whole block: when a guard guards the block, the first such guard takes the hit and the block stays; otherwise the block is removed (below). A hit on a block already gone changes nothing. The board owns this order, so no effect can break a guarded block: there is no public way to remove a block past its guards.
- `HitCell(cell)` is a booster's hit on one cell (the rocket's), in the same order: an empty cell changes nothing; when a guard guards the block on the cell, the first such guard takes the hit and the block keeps the cell; otherwise the cell is taken off its block. A block's last cell taken means the block is removed (below), so `GoneCount` rises. Otherwise what remains splits (below).
- **Split.** After a cell is taken, the block's cells left are grouped into 4-connected groups (`CellGroups`, a board-internal class with buffers sized once for the level's largest block). One group: the block only shrinks. More: the hit block keeps its index for the first group (the one holding its first cell left, in shape order), the others are appended to `Blocks` as new blocks of the same color, `RemainingBlocks` rises by the new blocks and the occupancy grid follows. A split is not a block gone: `GoneCount` does not change. The hit block and its parts keep the hit block's `Position`, each with its own cells relative to it (`BoardBlock.Shape`), so the parts' views stand where the block stood with no move ([Decisions](../Decisions.md)); a block's `Position` is the anchor of its shape and need not be one of its cells. The block list never grows past its first capacity: a live block has at least one cell and every gone block took at least one cell with it, so there are never more blocks than the level's cells. A block's shape changes only by losing cells, inside `Game.Logic` (`BoardBlock.RemoveCell`, `Reshape`); outside it the shape is read-only.
- **Removal.** Every way a block leaves the board entirely ends in one private removal: its cells are freed, `BoardBlock.IsGone` is set, `RemainingBlocks` drops and `GoneCount` rises. A block that is gone keeps its index in `Blocks`.
- `IsCleared` is true when every block is gone.
- `BlockAt(cell)` returns the block on a cell, or `Board.NoBlock` for an empty cell or one outside the board.
- `BoardBlock.ExtentAcross(side)` is how far the block reaches from that side, for the leave animation.

## Mechanics
The board knows no mechanic ([Decisions](../Decisions.md)). It asks two role interfaces at fixed points:
- `IMoveRule.Allows(board, block, direction)`: whether the block may step one cell that way. `CanStep` asks it for every step, and the exit asks it with the direction out through the door, so a rule that forbids a direction also forbids leaving that way. Contract: answers from the board's state and its own, changes nothing, and is asked often (every drag frame), so it must not allocate.
- `IBlockGuard.Guards(board, block)` and `TakeHit(block)`: whether the block is guarded, and taking a hit for it. `Board.IsGuarded(block)` asks every guard; `Board.Hit` gives the hit to the first guard that guards the block. Contract: `TakeHit` is called only by the board, only on a block the guard guards, and a guarded block is never removed by the hit. `IceMechanic` implements `TakeHit` explicitly, so whoever holds the ice to show it cannot melt it.
- The board is shown by what may be done with it, never as the concrete `Board` outside its owner: `IReadOnlyBoard` (`GoneCount`, `Blocks`, `BlockAt`, `IsGuarded`) for whoever only reads (the roles, the play feature's views through `LevelAttempt.Board`, an effect's `CanTarget`); `IBoard` (`IReadOnlyBoard` plus `Hit`, and the rocket's `HitCell`) for an effect's `Apply`; `Board` itself, which also steps and lets blocks leave, only inside the level attempt.
- `LevelMechanics.For(level)` is the one place that builds a level's mechanics: the rule and guard lists for the board, and each mechanic by name for whoever shows it (`Ice`). The game, the level editor and the solver build through it, so none can miss a mechanic. A new mechanic is a class, a field in the level format, a line there and, when it has a level rule, a check in `LevelValidator`'s list ([Levels](Levels.md)).

### Ice
`IceMechanic` (`Mechanics/Ice/`) plays both roles. It keeps each block's starting ice (`BlockData.Ice`) and the hits each block took, and computes a block's ice left when asked: the starting ice minus `GoneCount` minus its hits, never below zero. So every block gone, through a door or broken, melts one ice on every iced block, and nothing has to tell the ice that a block went. A block with ice left is not allowed to move or leave and is guarded; a hit on it takes one ice. At zero it plays as any other block. A block appended at run time (a split) has no starting ice, so its ice left is 0.

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
- `Step`, `CanStep`, `Hit` and `IsGuarded` do not allocate. `HitCell` allocates only the new blocks of a split (once per rocket hit, never per frame); the grouping reuses its buffers.
- A block's index never changes: the drag, the views and the leave animation refer to blocks by index.

## Tests
- `Tests/Play/Board/BoardStepTests`: free and blocked steps, edges, other blocks, L shapes, a blocked step that changes nothing.
- `Tests/Play/Drag/BlockDragTests`: see [Play](Play.md).
- `Tests/Play/Board/BoardExitTests`: exit through a door on each side (which also checks `Direction`'s table indexed by `Side`), exit on touch from any direction, other color is wall, partial cover stays, extent along the side only, L shapes, an L held by a block between its other part and the door, `TryLeave` for a block at its door, away from it, after the block in its way moved off and for a block that has already left, two doors of one color, cleared after the last block.
- `Tests/Play/Board/BoardRemovalTests`: a hit on an unguarded block frees its cells and clears the board on the last block, a hit on a block already gone changes nothing, `GoneCount` counts blocks through a door and hit but not a touch that leaves nothing, `BoardBlock.ExtentAcross`.
- `Tests/Play/Board/BoardHitCellTests`: a hit on a block's only cell removes it, on an end cell shrinks it without a split, on the middle of a line splits it in two that move on their own, on the corner of a three-cell L leaves two single cells, on the middle of an L keeps the index for the first part and appends the other, on the center of a plus leaves four blocks; an empty cell changes nothing; on an iced block it takes one ice and keeps the cell; a split is not a block gone, but each part gone counts and melts ice (a part has no ice of its own); a part leaves through its door and the last part gone clears the board.
- `Tests/Play/Board/BoardRoleTests`: a move rule stops a step in the direction it forbids only, the exit asks the rules with the direction out through the door, with no guards a hit takes the block. `Boards.Of(level)` builds a board through `LevelMechanics` for the tests.
- `Tests/Play/Mechanics/Ice/IceMechanicTests`: the level's ice at the start, an iced block cannot step or leave, one ice melts per block gone through a door or hit, a hit takes one ice and keeps the block, a hit once the ice is gone takes the block, a block whose ice is gone plays as any other, never below zero. `Tests/Play/Board/LevelMechanicsTests`: the ice is handed to the board as a rule and a guard.

## Related
- [Level attempt](LevelAttempt.md), which the play feature uses instead of `Board` and `BlockDrag` directly
- [Levels](Levels.md), [Reference](../Reference.md), [Decisions](../Decisions.md)
