# Play

> Summary: Shows a level on the board and lets the player drag blocks out through their doors. The play session builds the board from the supplied models and runs the level's time; the board interaction turns the pointer into board positions, asks the level attempt for whole-cell steps and a sub-cell offset, and moves the block views. When the level is won or failed the session tells its caller once; the Game scene continues with the next level or plays the same one again. No HUD, win screen or fail popup yet.

## Code
- Folder: `Assets/Game/Runtime/Play/` (`Scripts/Flow/`, `Scripts/Presentation/` with `Board/`, `Blocks/`, `Input/`, `Scripts/Settings/`; assets in `Installers/`, `Settings/`, `Materials/`), `Assets/Game/Runtime/Bootstrap/` (`GameScope`, `GameEntry`, `Scenes/Game.unity`)
- Assembly: `Game`
- Entry points: `PlaySession.Start(level, onWon, onFailed)` (`Play/Scripts/Flow/PlaySession.cs`); `PlayInstaller` (`Play/Installers/PlayInstaller.asset`). `BoardInteraction` (`Play/Scripts/Flow/BoardInteraction.cs`) is used only by the session.

## How it works
- **Scene.** `Game.unity` holds the camera, a light and `GameScope`, a child of the Boot scope ([App flow](AppFlow.md)), which registers the camera and runs its installer list (`PlayInstaller`). `GameEntry` is the scene's flow: it starts the level the level editor asked to play (`EditorPlayRequest`, taken once; never in a build), else the player's current level (`LevelSequence.Current`), and decides what follows a level: a win plays the next level ([App flow](AppFlow.md): the player's run completes their level, a run from the level editor never moves progress), a fail plays the same level again. Both happen in this scene, without a scene switch.
- **Layout.** `BoardLayout` puts the board on the XZ plane centered on the origin, row 0 at the far edge (+Z). It converts cells and drag poses to world positions and world points to board positions in cells.
- **Building.** `BoardBuilder` creates one `Board` root with a ground tile per cell, the frame (two half-cell wall pieces per edge cell, a door piece per cell a door covers, a wall piece on each corner) and one `BlockView` per block. Every piece is placed in the root's local space, so the board can be built under a parent (the level editor's scene preview does). `BlockPieceBuilder` builds a block from the supplied quarter pieces ([Supplied assets](../SuppliedAssets.md)): the piece of each quarter follows from its side and diagonal neighbours, and one inner-corner piece covers the three quarters around a concave corner. Model files are turned per file (blocks lie flat with a 90 degree turn around X, frame pieces turn their cap up).
- **Input.** `BoardPointer` (a VContainer `ITickable`) reads `Pointer.current`, so mouse and touch work the same, and casts a ray onto the plane of the blocks' tops. It calls back on press, move and release.
- **Session.** `PlaySession.Start(level, onWon, onFailed)` builds a new `LevelAttempt` ([Level attempt](LevelAttempt.md)), the layout and the board from the level's data, and hands them to `BoardInteraction`. Each frame it ticks the attempt with the frame time and reads the outcome it returns. The first frame that outcome is no longer `Playing` is the one place a level ends, for a win (set by the touch that takes the last block out) and a fail (set by the tick that reaches zero) alike: the session forgets the attempt, the interaction puts a held block back, and the session calls `onWon` or `onFailed` once. It forgets the attempt before calling back, so the callback can start the next level.
- **Restart and next level.** Every `Start` builds the level again from its data: a new attempt, new block views, and `BoardBuilder.Build` destroys the board it built before. Nothing from the previous attempt stays. Phase 4 step 3 moves the board's lifetime from the builder to its caller, with pooled pieces.
- **Drag.** `BoardInteraction` talks only to the attempt. On press it passes the finger position in cells (`Grab`), which picks the block and starts the timer, or makes a block at its door leave at once; on each move it gives the returned pose to the block view as its target; on release the attempt returns the held block, which snaps onto its cell with a short ease, as the reference does. When a pose, from a move or a press, says the block left, one DOTween sequence snaps it onto the cell in front of the door, holds there for a moment, slides it out through the door by the leave distance of the motion settings and hides it.
- **Motion.** `BlockView` approaches its target with `ExponentialFollow`: each frame it covers `1 - e^(-ln2 · dt / halfLife)` of the remaining distance, so the distance halves every half-life whatever the frame rate, and a long frame never passes the target. Only the drag uses it. The view puts itself on the target when closer than 0.001 units and stops updating; a new target starts it again. The one-shot motions are DOTween tweens linked to the block's object; starting any motion stops the one before, so grabbing a block mid-snap takes it over at once.

## Rules & invariants
- Decisions are in `Game.Logic` (`LevelAttempt` over `Board` and `BlockDrag`); the views only show poses.
- The board is built once per attempt; nothing in the drag path or the per-frame tick allocates.
- A level ends once: after the callback the session ticks no attempt until the next `Start`.
- Colors come from `ColorPalette` (one material per color id), sizes and meshes from `BoardArtSettings`, the drag half-life, the snap time and ease, the hold time and the slide out from `BlockMotionSettings`, given to each block view through `Init`.

## Known gaps
- No outline on the held block, no door animation; a leaving block slides out whole instead of being broken apart by the door.
- The camera is fixed for a 5 x 6 board; it is not framed per level.
- During a diagonal drag a block can be drawn up to half a cell into the cell diagonal to it, when both straight neighbours are free but the diagonal one is not.
- A door longer than one cell is a row of one-cell door pieces with a visible seam.
- `BoardPointer` polls `Pointer.current` and ignores the EventSystem, so a press on UI over the board also reaches the board; it moves to EventSystem handlers on one board-sized collider with the HUD (phase 4).
- The timer runs but is not shown; there is no HUD, win screen, fail popup or pause yet (phase 4 step 5), and no way back to Home from a level.
- A win builds the next level at once, so the last block's slide out is cut short; the win screen will come between them.

## Tests
- `Tests/Play/Drag/BlockDragTests`: offsets toward free and blocked cells, steps past half a cell, several cells at once, stopping at a block, going around an obstacle, the grab point, leaving through a door.
- `Tests/Play/Presentation/Blocks/ExponentialFollowTests`: half the distance after one half-life, two short frames equal one long frame, a long frame does not pass the target.
- `Tests/Play/Presentation/Blocks/BlockViewTests`: a view that stopped starts again on a new target; `SetPose` while following puts the block there and stops.
- `Tests/Bootstrap/InstallerAssetTests`: every installer and the settings it references have no unassigned reference.
- The session's end and the Game scene's flow are checked in the editor: press Play, win a level (the next one is built), let the time run out (the same level starts again).
- Views and input are checked in the editor: open `Game.unity` and press Play.

## Related
- [Level attempt](LevelAttempt.md), [Board](Board.md), [Levels](Levels.md), [Supplied assets](../SuppliedAssets.md)
