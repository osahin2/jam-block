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

## Core
Systems of the Core package (`Packages/com.base-setup.core/`), one note each.
- [Overview](Base/Architecture/Overview.md) - modules, what each does, dependencies, how a game registers them
- [Save](Base/Systems/Save.md) - typed JSON documents under `persistentDataPath`, synchronous, atomic writes
- [Levels](Base/Systems/Levels.md) - level progression: current number and index, completion, loop rule
- [UI](Base/Systems/UI.md) - screens and popups opened by type from per-feature catalogs, UI root and screen template, safe area fitted once
- [Scenes](Base/Systems/Scenes.md) - additive scene switch on top of a boot scene that stays loaded, transition hook with progress, ready fade overlay, play mode starts from the boot scene
- [Pooling](Base/Systems/Pooling.md) - `PrefabPool<T>` over `ObjectPool<T>`: prewarm inactive under a root, get under a parent with the prefab's pose, release, max idle size
- [Audio](Base/Systems/Audio.md) - `SfxCue` one-shots on pooled sources, looping music with crossfade, volume and mute per channel saved through Save
- [Haptics](Base/Systems/Haptics.md) - semantic presets through a platform backend (own iOS plugin, silent elsewhere), on/off saved through Save, game-supplied backend possible
- [Installers](Base/Systems/Installers.md) - `ScriptableObjectInstaller`: a feature's settings and services registered from one asset, scopes run a list of them
- [VContainer for Zenject users](Base/Architecture/VContainer-for-Zenject-users.md) - concept map and the differences that matter

## Templates
- [System](Templates/System.md) - system note
