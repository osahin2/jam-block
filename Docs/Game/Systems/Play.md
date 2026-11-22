# Play

> The Game scene: the board built from the supplied models, blocks dragged out through their doors, the HUD and the level popups. `PlaySession` runs one level and tells its caller once when it is won or failed; `GameEntry` decides what follows.

**Code:** `Runtime/Play/`, `Runtime/Bootstrap/` (`GameScope`, `GameEntry`), `Plugins/QuickOutline/` (the outline fork). **Entry:** `PlaySession.Start(level, onWon, onFailed)`, `Pause`, `Resume`, `Use(booster, ...)`; the screens `LevelHud`, `LevelCompletePopup`, `LevelFailedPopup`; `LevelStage.Show`, shared with the level editor's preview.

## Flow
`GameEntry` has one method per step: Start (choose the run), Play, Pause (settings popup), OfferBooster (buy popup), Won (reward, then the next level), Failed (retry or home). Retry, restart and continue are played under the scene fade, the flow's only awaited work.

## Board
- `LevelStage.Show` is the one way a level is shown: layout, build, shader clip and camera framing. The game and the preview call it, so the preview matches the game.
- `BoardBuilder` takes every piece from an `IBoardPieceSource` and creates or keeps nothing. Contract of `Take(prefab, parent)`: an instance, active under the parent, with the prefab's local pose; whoever gave the source releases what was taken, its own way (pools in the game, unsaved copies in the preview). A restart instantiates nothing.
- Every piece prefab is a neutral root with the supplied model turned inside it, so no model's orientation lives in code. Walls and doors are stretched bars (the supplied pieces are straight extrusions); blocks are built from quarter pieces.
- `CameraFraming` keeps the camera's angle and moves it until the board fills the window between the HUD and the booster slots; five measuring rounds leave it within a pixel.

## Input and views
- The board takes touches through the EventSystem on one collider fitted to the board, so UI over it blocks touches by itself. `BoardInput` routes them by mode (drag, pick, busy); `BoardProjection` maps screen to cells and back; `BoardPresenter` keeps the views in step with the attempt.
- `BoardPresenter.CatchUp` brings every view to the logic's state; whoever changes the board calls it, with no event in between. It builds views for split parts and rebuilds views that lost cells.
- Mechanics show themselves through `IBlockFeature`s listed once in `BlockFeatures.Of`. `Show` puts a (possibly pooled) view into the block's state; `CatchUp` moves a shown view to the current state. A new mechanic adds a feature, a part on `Block.prefab` and one line.
- The held block is outlined; the outline keeps two material arrays per renderer, so a grab allocates nothing. Blocks are flat plates so the outline has no hidden geometry to spill.
- A leaving block snaps to its door, slides out and is cut at the frame by its shader while the door drops and bursts cubes.
- `BoardPresenter` plays the board's haptics with its visuals: `Selection` on a grab and on a snap back, `Success` on an exit.
- `BlockView` follows the finger with an exponential follow that is frame-rate independent. While following, a new target only replaces the old: re-enabling the behaviour would skip that frame's `Update` and the block would stand still at 60 fps.
- `DoorView` reads its rest pose on first `Init`, not `Awake`: the preview builds doors in edit mode, where `Awake` does not run.

## Rules
- A `Start` while a level plays abandons that attempt without calling back (restart and retry go through it).
- A level ends once, on a settled board (no block leaving, no shot playing) and never while paused, so no popup opens over a moving block or the settings popup. Once decided, restart and pause do nothing.
- The reward and progress are saved on continue, before the coins fly.
- Nothing in the drag path, the tick or the HUD's countdown allocates.
- The UI matches the screen's height (`GameUIRoot`), as the reference does.

Known gaps: no sound on buttons; in a diagonal drag a block can be drawn up to half a cell into a blocked diagonal cell.

Related: [Level attempt](LevelAttempt.md), [Boosters](Boosters.md), [Coins](Coins.md), [Pooling](../../Base/Systems/Pooling.md)
