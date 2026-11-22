# Scenes

> Switches content scenes additively on top of a boot scene that stays loaded, behind a transition hook. Async, because it waits on the engine.

**Code:** `Core.Scenes`, `Core.Scenes.Editor`. **Entry:** `ISceneService.SwitchToAsync(sceneName, token)`, `ISceneTransition`, `ScreenFadeTransition.prefab`; `RegisterCoreScenes(transition)`.

## Rules
- The boot scene (build index 0) is never unloaded, so app-wide services need no `DontDestroyOnLoad`. At most one content scene is loaded; the old one is unloaded before the new one loads, to keep the memory peak low.
- Order: cover, unload, load with progress, set active, uncover.
- One switch at a time. The token must outlive the scene being left (the root scope's or `Application.exitCancellationToken`). A cancel or a failed load leaves the screen covered and frees the lock.
- `PlayFromFirstScene` sets the editor's play mode start scene to the first build scene, so Play from a content scene still boots.
