# Play

> Summary: Shows a level on the board and lets the player drag blocks out through their doors. The play session builds the board from the supplied models, turns the pointer into board positions, asks the drag rule for whole-cell steps and a sub-cell offset, and moves the block views. First built as a visual spike: no timer, HUD, win or fail flow yet.

## Code
- Folder: `Assets/Game/Runtime/Play/` (`Flow/`, `Presentation/`, `Settings/`), `Assets/Game/Runtime/Bootstrap/`, `Assets/Game/Runtime/Scenes/Game.unity`
- Assembly: `Game`
- Entry points: `PlaySession.Start(level)` (`Play/Flow/PlaySession.cs`); `PlayInstaller` (`Play/PlayInstaller.asset`)

## How it works
- **Scene.** `Game.unity` holds the camera, a light and `GameScope`, which registers the camera and runs its installer list (`LevelsInstaller`, `PlayInstaller`). `GameEntry` starts the first level of the `LevelCatalog`. The scene has no parent scope yet; it gets one when the Boot scene exists.
- **Layout.** `BoardLayout` puts the board on the XZ plane centered on the origin, row 0 at the far edge (+Z). It converts cells and drag poses to world positions and world points to board positions in cells.
- **Building.** `BoardBuilder` creates one `Board` root with a ground tile per cell, the frame (two half-cell wall pieces per edge cell, a door piece per cell a door covers, a wall piece on each corner) and one `BlockView` per block. `BlockPieceBuilder` builds a block from the supplied quarter pieces ([Supplied assets](../SuppliedAssets.md)): the piece of each quarter follows from its side and diagonal neighbours, and one inner-corner piece covers the three quarters around a concave corner. Model files are turned per file (blocks lie flat with a 90 degree turn around X, frame pieces turn their cap up).
- **Input.** `BoardPointer` (a VContainer `ITickable`) reads `Pointer.current`, so mouse and touch work the same, and casts a ray onto the plane of the blocks' tops. It calls back on press, move and release.
- **Drag.** On press the session finds the block under the finger (`Board.BlockAt`) and starts `BlockDrag`; on each move it passes the finger position and shows the returned pose; on release it puts the block back on its cell. When the pose says the block left, the view slides out through the door's side and hides itself.

## Rules & invariants
- Decisions are in `Game.Logic` (`Board`, `BlockDrag`); the views only show poses.
- The board is built once per level; nothing in the drag path allocates.
- Colors come from `ColorPalette` (one material per color id), sizes and meshes from `BoardArtSettings`, the leave motion from `BlockMotionSettings`.

## Known gaps
- No settle motion on release (the block snaps to its cell) and no break effect on leaving.
- During a diagonal drag a block can be drawn up to half a cell into the cell diagonal to it, when both straight neighbours are free but the diagonal one is not.
- A door longer than one cell is a row of one-cell door pieces with a visible seam.
- No timer, HUD, win, fail, pause or restart; the scene always opens the first level.

## Tests
- `Tests/Play/BlockDragTests`: offsets toward free and blocked cells, steps past half a cell, several cells at once, stopping at a block, going around an obstacle, the grab point, leaving through a door.
- `Tests/Bootstrap/InstallerAssetTests`: every installer and the settings it references have no unassigned reference.
- Views and input are checked in the editor: open `Game.unity` and press Play.

## Related
- [Board](Board.md), [Levels](Levels.md), [Supplied assets](../SuppliedAssets.md)
