# Docs index

> Entry point for the project documentation: the case, the plan, the conventions, the Core systems and the game systems.

## Case
- [Requirements](Game/Case.md) - what the case asks for, acceptance criteria, evaluation order, README contents
- [Reference game](Game/Reference.md) - how the reference game plays: free drag, door exit rule, timer, win and fail flow, home and settings
- [Supplied assets](Game/SuppliedAssets.md) - what the case supplied, how the models fit the 2-unit cell, problems found
- [Plan](Game/Plan.md) - phases, open decisions, status
- [Decisions](Game/Decisions.md) - short notes per choice; the README's decisions section comes from here
- [Work log](Game/WorkLog.md) - time and LLM use per session

## Conventions
- [Assemblies, features and folders](Conventions/Assemblies.md) - Game.Data / Game.Logic / Game / Game.Editor, one entry class per feature, folder layout
- [Logic and view](Conventions/LogicAndView.md) - logic decides and returns what happened, views only show and forward input, one flow per feature
- [Communication](Conventions/Communication.md) - how systems reach each other, when an event bus fits, dependency injection rules
- [Settings and installers](Conventions/SettingsAndInstallers.md) - settings assets per concern, one installer asset per feature
- [Code style](Conventions/CodeStyle.md) - member order (checked by a hook), one type per file, naming

## Game
Game systems get a note each in `Game/Systems/` as they are built.
- [Levels](Game/Systems/Levels.md) - level format: board size, blocks as shapes with a color id, doors on the sides, time limit; one `LevelAsset` per level; `LevelValidator` errors and warnings from a list of checks, overlapping doors and blocks in pieces included
- [Board](Game/Systems/Board.md) - move rules in whole cells: `CanStep`, `Step`, exit through a covering door of the block's color, `Hit` and `HitCell` (guards first), a block split into 4-connected parts by a cell hit, `GoneCount`, cleared board; mechanics through `IMoveRule` and `IBlockGuard`, built by `LevelMechanics.For(level)`, ice (`IceMechanic`); `BlockDrag` turns a finger into steps and a sub-cell offset
- [Ice](Game/Systems/Ice.md) - the ice blocker: a count on a block that locks it until that many blocks are gone, `IceNeverMelts` in the validator, a locked block shaking instead of being held, the editor's Ice field, tint and badge, `IceCover` turning a block to ice with a property block, cracking and melting as the views catch up
- [Arrow block](Game/Systems/Arrow.md) - a block that moves only along its arrow's axis, both ways: `ArrowMechanic` as a second move rule, drawn on the block with the editor's Arrow tool and stored in the level, the leave warning aware of the axis, the supplied arrows where they were drawn
- [Solver](Game/Systems/Solver.md) - whether a level can be cleared without boosters: breadth-first over the arrangements the player can reach, every block that can reach its door leaving at once, alike blocks counted once, the fewest rearrangements and a solution to play back; `Unsolvable` only when every arrangement was tried, `Unknown` when the caller stops the search
- [Level attempt](Game/Systems/LevelAttempt.md) - one try at a level: board, drag and timer behind `LevelAttempt`; timer starts on the first touch, pauses (`IsPaused`), fails at zero; won when the last block is gone; booster effects (`IBoardEffect`) through `Aim` and `TryApply`
- [Coins](Game/Systems/Coins.md) - `CoinWallet`: the coin balance, earned only, never negative, saved after every change through an `ICoinStore` port; `CoinCounter` and `CoinFlight`: a counter prefab for any screen and one app-wide flight that shows paid coins flying into it
- [App flow](Game/Systems/AppFlow.md) - Boot, Home and Game scenes, the Boot scope's app-wide services, `SceneFlow`, the run chosen once per Game scene (`ILevelRun`: the player's run completes progress, a test run from the level editor never does)
- [Play](Game/Systems/Play.md) - the Game scene: board built from pooled prefabs of the supplied models, blocks from quarter pieces, one bar per door and wall run, a block leaving through its door cut away at the door line by its shader while the door drops and cubes burst out, the held block outlined by a fork of Quick Outline, the camera framed to each level, one `LevelStage` shared with the level editor's preview, board input through the EventSystem on one board-sized collider, the play session that connects the level attempt and views, pauses, and ends a level once on a settled board; the HUD (level, countdown without allocations, coins, restart, pause), the level complete popup with the coin reward and the level failed popup (retry, home), all driven by `GameEntry`, one method per step
- [Rocket](Game/Systems/Rocket.md) - the booster that takes one cell (`RocketEffect`, `Board.HitCell`) and splits what remains; crosshairs on every block cell while aiming, the rocket flying up to the cell and bursting in the block's color, views rebuilt and built for the split
- [Boosters](Game/Systems/Boosters.md) - Freeze, Rocket, Hammer and Vacuum: saved stock with a free start (`BoosterInventory`), buying a pack with coins (`BoosterPurchase`, stock saved first), slots under the board, the aiming overlay dimming around the board, `TargetedBooster` fixing aim, apply, stock, shot, catch up; effects in logic (`HammerEffect`, `VacuumEffect`)
- [Home](Game/Systems/Home.md) - the home screen after the reference: top bar, level path, level button and a five-tab nav bar whose middle tabs switch panels with an animated selection
- [Preferences](Game/Systems/Preferences.md) - the settings popup: sound, music and vibration switches written to Core's audio and haptics, opened from Home's gear and as pause in the Game scene (with a home button)
- [Buttons](Game/Systems/Buttons.md) - `PressFeedback`: every button shrinks while pressed and springs back, tuned in one settings asset in the Boot scope
- [Popups](Game/Systems/Popups.md) - `PopupEntrance`: every popup's dimmer fades in and its parts grow in one after the other, tuned in one settings asset in the Boot scope
- [Level editor](Game/Systems/LevelEditor.md) - UI Toolkit window over a working copy of a level: a searchable level list (open, reorder by drag, duplicate, delete), New and Duplicate, a new level becoming an asset on its first save, Play saving and starting the game with that level; board drawn from above, select, block, door and erase tools as strategies, rebindable shortcuts, problems marked on the board and listed in plain words, the solver's verdict after every change (a quick check, Check fully with a progress bar for what it leaves undecided) with the solution played back on the board and a question before saving an unsolvable level, the level previewed in the Game scene, undo per stroke, save only without errors

## Core
Systems of the Core package (`Packages/com.base-setup.core/`), one note each.
- [Overview](Base/Architecture/Overview.md) - modules, what each does, dependencies, how a game registers them
- [Save](Base/Systems/Save.md) - typed JSON documents under `persistentDataPath`, synchronous, atomic writes
- [Levels](Base/Systems/Levels.md) - level progression: current number and index, completion, loop rule
- [UI](Base/Systems/UI.md) - screens and popups opened by type from per-feature catalogs (registered through a scoped factory, so a scope holds several), UI root and screen template, safe area fitted once at run time
- [Scenes](Base/Systems/Scenes.md) - additive scene switch on top of a boot scene that stays loaded, transition hook with progress, ready fade overlay, play mode starts from the boot scene
- [Pooling](Base/Systems/Pooling.md) - `PrefabPool<T>` over `ObjectPool<T>`: prewarm inactive under a root, get under a parent with the prefab's pose, release, max idle size
- [Audio](Base/Systems/Audio.md) - `SfxCue` one-shots on pooled sources, looping music with crossfade, volume and mute per channel saved through Save
- [Haptics](Base/Systems/Haptics.md) - semantic presets through a platform backend (own iOS plugin, silent elsewhere), on/off saved through Save, game-supplied backend possible
- [Installers](Base/Systems/Installers.md) - `ScriptableObjectInstaller`: a feature's settings and services registered from one asset, scopes run a list of them
- [VContainer for Zenject users](Base/Architecture/VContainer-for-Zenject-users.md) - concept map and the differences that matter

## Templates
- [System](Templates/System.md) - system note
