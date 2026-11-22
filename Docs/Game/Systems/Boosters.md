# Boosters

> Freeze, Rocket, Hammer and Vacuum under the board: saved stock with a free start, a buy popup for packs, and one flow for every aimed booster (aim with the screen dimmed, tap a target, stock down, shot, views catch up). What a booster does to the board is a logic effect; this feature only presents it.

**Code:** `Logic/Boosters/` (stock, purchase), `Logic/Play/Effects/`, `Data/Boosters/`, `Runtime/Play/Scripts/` (`Services/` boosters, `Presentation/Boosters/` slots, overlay, shots, `UI/BoosterPurchasePopup`). **Entry:** `PlaySession.Use(booster, ...)` and `IsAvailable(booster)` from the HUD; `IBooster` with `TargetedBooster` and `FreezeBooster`, one registration line each in `PlayInstaller`.

## Rules
- The stock goes down only when an effect has been applied, and is saved at once. A purchase saves the stock before spending the coins, so a crash between the two leaves boosters unpaid rather than coins lost. `IBoosterStore.Save` returns once persisted.
- `IBooster`: `IsAvailable` changes and allocates nothing (read every frame); `Use` calls `onDone` exactly once however the use ends; `Cancel` ends a use in progress without firing and does nothing otherwise.
- `TargetedBooster` fixes the order: release the held block, aim, pick, `TryApply`, stock down, busy, shot, catch up, drag. A booster adds only its effect and its shot.
- `BoosterShot.Build` runs after the effect is applied and before the views catch up, so views still show the board at the tap; it calls `onLanded` once. Its sequence is targeted at the shot, so a replay or stop ends it.
- Vacuum reads the locks as they are at the tap. Hammer on an iced block takes one ice.
- Time runs while aiming and while a shot plays; the buy popup pauses it. Slots are off before the first touch and once the level is decided.
- Freeze shows a bar under the time box (`Hud/FreezeBar`, `FreezeBarSettings`) that empties with `PlaySession.FrozenSeconds`, so it stops when the timer is paused and closes on a restart or a decided level.
- Slots are placed in the HUD prefab, not made at run time, because their press feedback is injected when Core creates the screen.

Related: [Rocket](Rocket.md), [Level attempt](LevelAttempt.md), [Coins](Coins.md), [Play](Play.md)
