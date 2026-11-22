# Rocket

> The booster that takes one cell off a block; what remains splits into blocks wherever its cells no longer touch. On an iced block it takes one ice instead. Crosshairs mark every block cell while aiming; the rocket flies up to the cell and bursts in the block's color.

**Code:** `Logic/Play/Effects/RocketEffect.cs`, `Board.HitCell` and `CellGroups` ([Board](Board.md)), `Runtime/Play/Scripts/Presentation/Boosters/` (`RocketShot`, crosshairs in `AimingOverlay`). **Entry:** none of its own; a `TargetedBooster` registered by `PlayInstaller` ([Boosters](Boosters.md)).

## Rules
- The crosshairs stand at the height the rocket hits (`BoosterMotionSettings.TargetLift`), so the two cannot drift apart. They are made once per level's largest need and reused.
- The burst color is read from the views before they catch up (`BoardPresenter.TryFindColorShownAt`), so it is right for a cell taken, a last cell and an iced block alike. The burst belongs to the shot, not to `CatchUp`.
- The shot calls `onLanded` once, at the hit; until then the board is busy and not settled, so a level won by the rocket ends after the hit.

Related: [Boosters](Boosters.md), [Board](Board.md)
