# Scenes

> Summary: Switches content scenes additively on top of a boot scene that stays loaded: covers the screen through a transition hook, unloads the current scene, loads the next one with progress and makes it active, then uncovers. Async, because it waits on the engine.

## Code
- Folder: `Packages/com.base-setup.core/Runtime/Scenes/`, `Packages/com.base-setup.core/Editor/Scenes/`
- Assembly: `Core.Scenes` (references UniTask, and VContainer in the installer only); `Core.Scenes.Editor` (editor only)
- Entry points:
  - `ISceneService` ([ISceneService.cs](../../../Packages/com.base-setup.core/Runtime/Scenes/ISceneService.cs)): `CurrentScene`, `UniTask SwitchToAsync(sceneName, cancellationToken)`.
  - `ISceneTransition` ([ISceneTransition.cs](../../../Packages/com.base-setup.core/Runtime/Scenes/ISceneTransition.cs)): the hook a game implements for what the player sees during a switch: `CoverAsync`, `ReportProgress(0..1)`, `UncoverAsync`.
  - `ScreenFadeTransition` ([ScreenFadeTransition.cs](../../../Packages/com.base-setup.core/Runtime/Scenes/ScreenFadeTransition.cs)) and `ScreenFadeTransition.prefab`: a ready transition, a black overlay canvas (sort order 1000) that fades through a `CanvasGroup` and ignores progress.
  - `ScenesInstaller` ([ScenesInstaller.cs](../../../Packages/com.base-setup.core/Runtime/Scenes/ScenesInstaller.cs)): `RegisterCoreScenes(transition = null)`, in the root scope.
  - `PlayFromFirstScene` ([PlayFromFirstScene.cs](../../../Packages/com.base-setup.core/Editor/Scenes/PlayFromFirstScene.cs)): editor only, see below.

## How it works
- **Additive model.** The scene that holds the root `LifetimeScope` (the boot scene, build index 0) is never unloaded, so app-wide services and their objects need no `DontDestroyOnLoad`. Content scenes are loaded additively and made the active scene, so objects they create and their lighting belong to them.
- **At most one content scene.** "Additive" is relative to the boot scene only: every switch unloads the previous content scene, so the loaded set is always the boot scene plus one content scene (Boot + Meta, then Meta → Level gives Boot + Level). Each content scene's scope is a child of the root scope, so app-wide services are shared across them. Keeping one content scene loaded under another (stacking) is not supported; it would need separate additive load and unload methods, added when a game needs it. Often a UI screen is enough instead.
- **Unload before load.** The old scene is unloaded before the new one loads, so two large scenes are never in memory together and the peak stays low on mobile. The switch therefore takes unload plus load time, hidden behind the covered screen. A seamless switch would load first and unload after, at the cost of a higher memory peak.
- **A switch** runs in this order: `CoverAsync` → unload `CurrentScene` (skipped on the first switch), then `Resources.UnloadUnusedAssets` → `ReportProgress(0)` → load additively, reporting the engine's load progress → set active, `ReportProgress(1)` → `UncoverAsync`. `CurrentScene` is the scene name passed in.
- **Child scopes.** A content scene's `LifetimeScope` names the root scope's type in its `parentReference` field; VContainer finds the root scope among the loaded scenes when the content scene awakes.
- **Without a transition** (`RegisterCoreScenes()` with no argument) an internal no-op transition is used and scenes change uncovered.
- **`ScreenFadeTransition`** moves its alpha towards the target at a fixed speed in unscaled time, so a paused game still fades. Covering also blocks raycasts, so the old scene's UI cannot be clicked mid-switch. The prefab starts covered, so the boot scene is never visible and the first switch only fades out. Its fade duration is on the prefab as part of its visual design, like a screen's layout.
- **Play mode starts from the boot scene.** `PlayFromFirstScene` sets `EditorSceneManager.playModeStartScene` to the first enabled build scene, and again whenever the build list changes. Pressing Play with a content scene open therefore boots normally instead of failing to find the parent scope.

## Rules & invariants
- One switch at a time: `SwitchToAsync` while another switch runs throws `InvalidOperationException`. The caller waits for the first one.
- **The token must outlive the scene being left.** A switch unloads the caller's scene; a token tied to that scene would cancel the switch halfway. Pass the root scope's token or `Application.exitCancellationToken`.
- Cancellation stops between steps and leaves the screen covered; a failed load (for example a scene missing from the build list, which throws `ArgumentException`) does the same. Both release the one-switch lock, so the next switch works.
- A scene must be in the build list to be loaded. The boot scene must be the first enabled entry.
- No Addressables: plain `SceneManager` until a game needs remote content.
- A PlayMode test run also starts from the boot scene while `playModeStartScene` is set; no PlayMode tests exist yet. When the first one is added, clear the start scene for test runs.

## Tests
`Packages/com.base-setup.core/Tests/Scenes/` (`SceneServiceTests`), with a fake loader and a fake transition behind the internal `ISceneLoader` seam: the first switch covers, loads and uncovers without unloading; the next switch unloads the current scene before loading; progress reaches the transition; a switch during a switch throws and the first one still finishes; a failed load lets the next switch run.

The real engine path was checked in play mode: Play opens the boot scene, which switches to the content scene (both loaded, the content scene active, the overlay transparent and not blocking, the scene's UI and objects present), and a second switch to the same scene from the running game covered the screen, reloaded the scene with a fresh child scope and uncovered, with no console output.

## Related
- The game starts the first switch from an entry point registered in the root scope. It is the only place a game needs async code for Core: a scene switch really waits on the engine.
