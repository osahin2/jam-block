---
name: add-game-feature
description: Add a feature, a rule, a UI screen, a settings asset or a level-editor tool to the case game - which assembly it goes in, its entry class, folders, installer registration, tests and docs. Use when building or extending game code under `Assets/Game/`.
---

# Adding to the game

Read the conventions first: `Docs/Conventions/Assemblies.md`, `LogicAndView.md`, `SettingsAndInstallers.md`, `Communication.md`. Check `Docs/Game/Plan.md` for the phase and its scope; do not build optional scope early.

## Where code goes
| It is... | Assembly | Folder |
|---|---|---|
| Level format or saved data | `Game.Data` | `Assets/Game/Data/<Feature>/` |
| A rule, a state change, a result type | `Game.Logic` | `Assets/Game/Logic/<Feature>/` |
| A view, input, a flow, a screen, settings, an installer | `Game` | `Assets/Game/Runtime/<Feature>/<Role>/` |
| Level editor or other editor tooling | `Game.Editor` | `Assets/Game/Editor/<Feature>/` |
| A test | `Game.Tests` | `Assets/Game/Tests/<Feature>/` |

If `Game.Logic` needs something from `UnityEngine`, the design is wrong: pass the value in (time, a random seed) or move the code to `Game`.

## A new feature
1. Name its entry class first and write its signature: what it takes (data types), what it returns or calls back with. Callers will know only this class.
2. Data types in `Game.Data`, rules in `Game.Logic` with EditMode tests before any view.
3. Settings: one ScriptableObject class per concern in `Runtime/<Feature>/Settings/`; create the asset with an editor script (skill `unity-editor`).
4. Installer: a sealed `ScriptableObjectInstaller` (Core.Installers) that registers the settings (`RegisterInstance`) and the feature's services; create the asset and add it to the scene scope's installer list with an editor script. The installer asset test then checks its references.
5. Views per `LogicAndView.md`: they show state and forward input, decisions stay in the logic; prefab views get settings through `Init`, scene views through `[Inject] Construct`.
6. A note in `Docs/Game/Systems/<Feature>.md` (template `Docs/Templates/System.md`) and its line in `Docs/00-Index.md`. A choice a reviewer could ask about: an entry in `Docs/Game/Decisions.md`.

## A new UI screen
A prefab variant of Core's `UIScreenTemplate`, script and prefab together in the feature's `UI/`, added to the feature's `UIScreenCatalog`. Buttons the flow reacts to are named callbacks in the screen's `Show` method. Every button gives visual feedback on touch.

## A new level
Made in the level editor only, never by editing the saved file. Format: the level data note in `Docs/Game/Systems/`.

## Always
- Compile, check for errors, run EditMode tests (skill `unity-editor`).
- No allocations in per-frame code; pool what spawns often.
- Anything another game could use unchanged belongs in Core (skill `add-core-module`).
