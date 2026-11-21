# Settings and installers

> Summary: Tuning values and content references live in ScriptableObject settings assets, one per concern. Each feature has one ScriptableObject installer that registers its settings and services; the scene scope only runs the list of installers.

## What goes where
| Kind of data | Where |
|---|---|
| Which prefab to spawn, speeds, sizes, spacings, colors, feedback strengths | A settings asset per concern (`EnemySettings`, `BoardSettings`), the class in the feature's `Scripts/Settings/`, the asset in its `Settings/` ([Assemblies](Assemblies.md)) |
| A prefab's references to its own children (`_body`, `_label`) | The prefab |
| A screen's layout, fonts, colors | The screen prefab |
| Objects that exist only in the scene (camera, composite views, UI root) | The scene's `LifetimeScope` |

A concern is what a designer tunes as a unit. No single `GameSettings` asset with everything; no asset holding one unrelated number.

## How settings reach code
- **Feature installer.** A ScriptableObject deriving from Core's `ScriptableObjectInstaller` ([Installers](../Base/Systems/Installers.md)). It references the feature's settings assets, registers them with `RegisterInstance` and registers the feature's services. Adding a feature to a scene is adding its installer asset to the scope's list.
- **Services** take settings through their constructor.
- **Spawned prefab views** get settings from their spawner through `Init(..., settings)`. They are not created through the container and have no `[Inject]` method, so spawning works the same with pooling and the view's needs are visible in `Init`.
- **UI screens** are the exception: Core.UI creates them through the container once, so they may take game services through an `[Inject]` method.
- **Core modules** that have tuning take a settings asset in their installer (`RegisterCoreAudio(settings)`).

- **Read when used.** Code reads a setting each time it acts on it, so a value changed in the inspector during play mode shows on the next action. Tweens are built when they play from the settings as they are then; no tween is built once and replayed, which would keep the values of its first play. The tween before is stopped through its target (`DOKill`, or `SetTarget` on a sequence), not through a kept handle, which may point at a tween already finished and, with DOTween's recycling on, reused by another object.

- **Level content** is not a setting: levels are `Game.Data` written by the level editor ([Assemblies](Assemblies.md)).

## Checks
`Game.Tests` loads every installer asset and fails when the installer or one of its settings assets has an unassigned reference, so a missing prefab fails in the test run instead of at startup.
