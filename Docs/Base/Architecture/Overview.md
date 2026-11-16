# Core overview

> Core is an embedded package of small, independent, game-agnostic modules (`Packages/com.base-setup.core/`). The game composes them through VContainer and talks to them through their interfaces; Core never knows the game exists.

## Layers

```
Game            Assets/Game/ (asmdef: Game)
                composition roots, game rules, views, UI screens
   |  references (one-way)
   v
Core modules    Packages/com.base-setup.core/Runtime/<Module>/ (asmdef: Core.<Module>)
   |
   v
Third party     VContainer, UniTask (only where needed)      Unity 2022.3 (base template: Unity 6)
```

## Modules

| Module | What it does | Depends on | Entry point |
|---|---|---|---|
| [Save](../Systems/Save.md) | Typed JSON documents under `persistentDataPath`, synchronous, atomic writes | - | `ISaveService`, `RegisterCoreSave()` |
| [Levels](../Systems/Levels.md) | Level progression: current number and index, completion, loop after the last level | Save | `ILevelProgression`, `RegisterCoreLevels(loop)` |
| [UI](../Systems/UI.md) | Screens and popups opened by type from per-feature catalogs, UI root and screen template, safe area fitted once | uGUI | `IUIService`, `UIScreen`, `RegisterCoreUI(root)`, `RegisterUIScreens(catalog)` |
| [Scenes](../Systems/Scenes.md) | Additive scene switch on top of a boot scene that stays loaded, transition hook with progress, ready fade overlay | UniTask | `ISceneService`, `RegisterCoreScenes(transition)` |
| [Pooling](../Systems/Pooling.md) | `PrefabPool<T>`: prewarm, get under a parent with the prefab's pose, release | - | `PrefabPool<T>` (no service) |
| [Audio](../Systems/Audio.md) | Sound effect cues on pooled sources, looping music with crossfade, volume and mute saved | Pooling, Save | `IAudioService`, `SfxCue`, `RegisterCoreAudio(settings)` |
| [Haptics](../Systems/Haptics.md) | Semantic haptic presets through a platform backend (own iOS plugin), on/off saved, replaceable backend | Save | `IHapticService`, `RegisterCoreHaptics()` |
| [Installers](../Systems/Installers.md) | `ScriptableObjectInstaller`: an asset that registers one feature's settings and services; a scope runs a list of them | - | `ScriptableObjectInstaller` |

Every installer except Pooling's references VContainer; no service class does.

## Using Core in a game
- **Root scope** (boot scene, never unloaded): `RegisterCoreSave()` first, then the app-wide modules that need it: `RegisterCoreAudio`, `RegisterCoreHaptics`, `RegisterCoreScenes`, `RegisterCoreLevels`.
- **Content scene scope** (child of the root scope): `RegisterCoreUI(root)` with the scene's `UIRoot`, then each feature's `ScriptableObjectInstaller` from a serialized list; a feature installer calls `RegisterUIScreens(catalog)` for its screens.
- The game keeps exactly one `AudioListener`, in the boot scene.
- `Core.Save` is the only writer of persistent data; modules that keep player settings (Audio, Haptics, Levels) save through it.
- Code style for everything in the project: [Code style](../../Conventions/CodeStyle.md).

## Adding to Core
A system enters Core only if the game uses it now and it is game-agnostic, or every game will certainly need it. Not in Core yet, because their shape depends on choices a game has not made: ads, IAP, analytics, remote config, localization, economy, tutorials, level flow orchestration.
