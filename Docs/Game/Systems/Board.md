# Board

> The move rules in whole cells: steps, exits through a door of the block's color, booster hits, splits and a cleared board. Plain C# in `Game.Logic`; mechanics plug in through two role interfaces.

**Code:** `Logic/Play/Board/`, `Drag/`, `Mechanics/`. **Entry:** `Board` (only inside the level attempt), `BlockDrag`, `LevelMechanics.For(level)`; outside the attempt the board is `IReadOnlyBoard` (read) or `IBoard` (read and hit).

## Rules
- A block leaves through the first door, in level order, that has its color, spans its whole extent on that side, is allowed by every move rule and has no block between it and any of the block's cells. Only the moved or touched block is checked, so a block freed by another leaves on the next tap.
- `Hit` and `HitCell` give the hit to a guard first; a guarded block is never removed. There is no public way past the guards.
- Every way out of the board ends in one private removal, which raises `GoneCount`. A rocket's split is not a block gone: the hit block keeps its index for its first part, the others are appended. The block list is sized once for the level's cells and never grows; a block's index never changes.
- `IMoveRule.Allows` answers from state, changes nothing and does not allocate (asked every drag frame). It is also asked with the direction out through a door, so a rule that forbids a direction forbids leaving that way. `IBlockGuard.TakeHit` is called only by the board, on a block the guard guards.
- `LevelMechanics.For` is the one place a level's mechanics are built, for the game, the editor and the solver.
- `BlockDrag` steps when the finger is more than half a cell past, alternates axes so a block goes round an obstacle, never steps through a block. The sub-cell offset is the view's.

Related: [Level attempt](LevelAttempt.md), [Ice](Ice.md), [Arrow block](Arrow.md), [Levels](Levels.md)
