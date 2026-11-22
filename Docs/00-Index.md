# Docs index

> Entry point for the project documentation: the case, the conventions, the game systems and the Core package.

## Case
- [Requirements](Game/Case.md) - what the case asks for and how it is evaluated
- [Reference game](Game/Reference.md) - how the reference game plays and looks
- [Decisions](Game/Decisions.md) - each choice, why, and what it was chosen over
- [Conventions](Conventions.md) - assemblies and folders, logic and view, communication, settings, code style

## Game
- [Levels](Game/Systems/Levels.md) - level format and validation
- [Board](Game/Systems/Board.md) - move rules, exits, hits and splits, mechanics as role interfaces
- [Ice](Game/Systems/Ice.md) - the ice blocker
- [Arrow block](Game/Systems/Arrow.md) - a block that moves along one axis
- [Level attempt](Game/Systems/LevelAttempt.md) - one try at a level, its timer and booster effects
- [Coins](Game/Systems/Coins.md) - the saved balance, counter and coin flight
- [App flow](Game/Systems/AppFlow.md) - Boot, Home and Game scenes, the run played
- [Play](Game/Systems/Play.md) - the Game scene: board, input, views, HUD and popups
- [Rocket](Game/Systems/Rocket.md) - the booster that takes one cell
- [Boosters](Game/Systems/Boosters.md) - stock, purchase and the aimed booster flow
- [Home](Game/Systems/Home.md) - the home screen
- [Preferences](Game/Systems/Preferences.md) - the settings popup
- [Buttons](Game/Systems/Buttons.md) - press feedback on every button
- [Popups](Game/Systems/Popups.md) - the shared popup entrance
- [Level editor](Game/Systems/LevelEditor.md) - the designers' level editor window
- [Solver](Game/Systems/Solver.md) - whether a level can be cleared without boosters
- [Player build](Game/Systems/PlayerBuild.md) - APK and Xcode builds, mobile settings

## Core
- [Overview](Base/Architecture/Overview.md) - modules and how a game registers them
- [Save](Base/Systems/Save.md), [Levels](Base/Systems/Levels.md), [UI](Base/Systems/UI.md), [Scenes](Base/Systems/Scenes.md), [Pooling](Base/Systems/Pooling.md), [Audio](Base/Systems/Audio.md), [Haptics](Base/Systems/Haptics.md), [Display](Base/Systems/Display.md), [Installers](Base/Systems/Installers.md)
