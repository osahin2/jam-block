# Rocket

> Summary: The second booster: aimed at one cell, it takes that cell off its block, and what remains of the block splits into separate blocks wherever its cells no longer touch side to side; on an iced block it takes one ice instead. While aiming, a crosshair marks every cell of every block; the rocket flies up from the bottom of the screen to the cell and bursts there in the block's color, and the block views catch up with the split.

## Code
- Folder: `Assets/Game/Logic/Play/Effects/` (`RocketEffect`), `Assets/Game/Logic/Play/Board/` (`Board.HitCell`, `CellGroups`), `Assets/Game/Runtime/Play/Scripts/Presentation/Boosters/` (`RocketShot`, the crosshairs in `AimingOverlay`), `Assets/Game/Runtime/Play/Scripts/Presentation/Board/BlockViewBuilder.cs`; assets `Play/Settings/RocketBooster.asset`, `Play/Prefabs/UI/RocketShot.prefab`, the `Crosshair` template under `Aiming` in `Play/Prefabs/UI/LevelHud.prefab`
- Assembly: `Game.Logic`, `Game`
- Entry points: none of its own. `PlayInstaller` registers it between Freeze and Hammer as `TargetedBooster(_rocket, new RocketEffect(), inventory)`, so it follows the booster flow in [Boosters](Boosters.md): aim, `TryApply`, stock down, shot, catch up.

## How it works
- **Effect.** `RocketEffect.CanTarget(board, cell)`: any cell of a block, iced or not (an empty cell is no target, so a tap on it cancels). `Apply(board, cell)` is `IBoard.HitCell(cell)`: the board gives the hit to a guard first (one ice taken, the cell kept), takes the block whole when the cell was its last, and otherwise takes the cell and splits what remains ([Board](Board.md), "Split"). A split is not a block gone; a block gone through the rocket's hit melts ice like any other.
- **Definition.** `RocketBooster.asset` is a `TargetedDefinition` (id `rocket`, `Rocket_Purple_Icon`, 2 free, 3 for 100 coins) with `MarksTargets` on and `RocketShot.prefab` as its shot. The HUD's second slot shows it.
- **Crosshairs.** For a definition with `MarksTargets`, `TargetedBooster` asks the overlay to `MarkTargets(effect, attempt.Board, board)` right after showing it. The overlay walks the blocks on the board and puts a crosshair on every cell its effect `CanTarget`: a copy of the `Crosshair` template (the supplied `SquareRoundedMask`, white at about half opacity, not a raycast target, so a tap reaches the board's collider), centered on the cell's screen point at `BoosterMotionSettings.TargetLift` cells above the board (the height the rocket hits, so the two cannot drift apart; the overlay gets the settings through its `Construct`) and sized to `_crosshairSize` of the cell as seen. Copies are made the first time a level has more targets than before and kept, so a later aim makes none; `Show` hides them all first, so the Hammer and the Vacuum show none. They hide with the overlay.
- **Shot.** `RocketShot.Build`: the rocket (`Body`, `Rocket_Purple_Icon`, whose nose points up and right, so it is turned by the heading less `_noseAngle`) starts just below the screen's bottom edge, under the active slot, and flies straight to the cell's screen point at `TargetLift` cells above the board, where its crosshair was, in `RocketFlight` seconds with `RocketFlightEase`. On arrival it hides, the burst plays at the cell and `onLanded` is called, so `TargetedBooster` catches the views up at once (the split's pieces rebuilt, its parts built from the pools, [Play](Play.md)) while the burst still plays; the shot ends `RocketBurstHold` seconds later.
- **Burst.** A ParticleSystem drawn in the UI by UI Particle (made from the Confetti prefab's emitter): one burst of small squares from a circle at once, with no start delay (the views have already caught up, so the cell would stand empty), braked by a velocity limit, shrinking away, no gravity. Its start color is the hit block's color, read from the views before they catch up: `BoardPresenter.TryFindColorShownAt(cell)` finds the view that still shows the cell (each `BlockView` keeps the cells it was built from, relative to its block's `Position`, which a split leaves unchanged) and returns its pieces' material color ([Decisions](../Decisions.md)). So the burst has the right color on every hit: a cell taken, a block's last cell and an iced block alike.

## Rules & invariants
- The rocket can aim at an iced block; it takes one ice and the block keeps the cell.
- The burst belongs to the shot, not to `CatchUp` ([Decisions](../Decisions.md)).
- The shot calls `onLanded` once, at the hit; until then the board is `Busy` and not settled, so a level won by the rocket ends after the hit.
- Nothing here runs per frame; the crosshairs and the burst allocate only when a level first needs more of them.

## Tests
- `Tests/Play/Effects/RocketEffectTests`: any cell of a block is a target, iced or not, an empty cell is not; on an unlocked block the cell is taken and the rest splits; on a locked block one ice is taken and the cell kept.
- `Tests/Play/Board/BoardHitCellTests`: the hit and the split ([Board](Board.md)).
- `Tests/Play/Presentation/Blocks/BlockViewTests`: a view shows its own cells only and none while it leaves.
- `Tests/Play/Presentation/Board/BlockViewBuilderTests`: views rebuilt and built for a split ([Play](Play.md)).
- `Tests/Play/Settings/BoosterDefinitionsTests` and `Tests/Bootstrap/InstallerAssetTests`: the rocket's definition is complete, the HUD has a slot for it, the installer references it.
- Checked in the editor: the Rocket's slot is second; aiming shows a crosshair on every cell of every block, iced ones too, and none for the Hammer after it; a tap on an empty cell or off the board cancels; the rocket flies up from below the screen to the cell and bursts in the block's color; the middle of a line leaves two blocks that move on their own, the corner of a three-cell L two single cells; a block's last cell takes it and melts one ice elsewhere; an iced block loses one ice and keeps its cell; the last block taken by the rocket wins at the hit, the burst still playing as the popup opens.

## Related
- [Boosters](Boosters.md), [Board](Board.md), [Play](Play.md), [Level attempt](LevelAttempt.md), [Reference](../Reference.md) ("Boosters and ice"), [Decisions](../Decisions.md)
