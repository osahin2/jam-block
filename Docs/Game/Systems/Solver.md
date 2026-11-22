# Solver

> Whether a level can be cleared without boosters, and in the fewest moves when it can: `Solvable` with a solution to play back, `Unsolvable` when no reachable arrangement gets every block out, `Unknown` when the caller stops the search first. Plain C# on the board's own rules.

**Code:** `Logic/Levels/Solving/`, `Board` (internal arrangement methods, [Board](Board.md)). **Entry:** `LevelSolver.Solve(level, stateBudget)` or `Solve(level, keepSearching)`, returning a `LevelSolution` (`Verdict`, `Moves`, `Rearrangements`, `ExploredStates`).

## Rules
- A move is one drag: a block goes to any place it reaches through free cells (`BlockReach`).
- Every block that can reach a door leaves at once; only arrangements where none can leave are searched, breadth-first, so the first clear board has the fewest rearrangements.
- A block that fits no door of its color makes the level `Unsolvable` without a search (`BlocksCanLeaveCheck.EveryBlockFitsADoor`).
- Alike blocks (same color, shape, ice, arrow) are one in `ArrangementKey`; a test fails when `BlockData` or `ArrowData` gains a field, so a new property cannot be left out.
- After a rearrangement the moved block is not asked again. This rests on every step being reversible; a one-way move rule must drop this shortcut.
- `Unsolvable` is a proof for the rules without boosters; `Unknown` is never shown as unsolvable. The time limit plays no part.
- The level must have no validator errors. A solver board is never split or hit.
- Large open boards can take minutes to prove unsolvable, which is why the editor bounds its checks ([Level editor](LevelEditor.md)). The solver allocates freely; it never runs per frame. A test checks every level asset is solvable.

Related: [Board](Board.md), [Ice](Ice.md), [Arrow block](Arrow.md)
