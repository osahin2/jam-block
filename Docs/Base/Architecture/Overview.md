# Core overview

> Core is an embedded package of small, game-agnostic modules (`Packages/com.base-setup.core/`). The game composes them through VContainer and talks to them through interfaces; Core never knows the game exists.

| Module | What it does | Entry point |
|---|---|---|
| [Save](../Systems/Save.md) | Typed JSON documents, synchronous, atomic writes | `ISaveService` |
| [Levels](../Systems/Levels.md) | Level progression with a loop after the last level | `ILevelProgression` |
| [UI](../Systems/UI.md) | Screens and popups opened by type from per-feature catalogs | `IUIService`, `UIScreen` |
| [Scenes](../Systems/Scenes.md) | Additive scene switch over a boot scene, transition hook | `ISceneService` |
| [Pooling](../Systems/Pooling.md) | Prefab pool over `ObjectPool<T>` | `PrefabPool<T>` |
| [Audio](../Systems/Audio.md) | Sound cues on pooled sources, music crossfade, saved volume | `IAudioService` |
| [Haptics](../Systems/Haptics.md) | Semantic presets through a platform backend, saved on/off | `IHapticService` |
| [Display](../Systems/Display.md) | Frame rate and screen on, applied at startup | `DisplaySettings` |
| [Installers](../Systems/Installers.md) | A feature's registrations as an asset | `ScriptableObjectInstaller` |

## Rules
- Root scope (boot scene, never unloaded): Display and Save first, then Audio, Haptics, Scenes, Levels. Content scene scope: `RegisterCoreUI(root)`, then the feature installers.
- `Core.Save` is the only writer of persistent data. The game keeps exactly one `AudioListener`, in the boot scene.
- Installers reference VContainer; no service class does.
- A system enters Core only if the game uses it now and it is game-agnostic.
