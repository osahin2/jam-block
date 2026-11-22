# App flow

> Three scenes: Boot stays loaded with the app-wide services; Home and Game switch on top of it. Which level is played and what follows a win.

**Code:** `Runtime/Bootstrap/`, `Runtime/Home/`, `Runtime/Levels/`. **Entry:** `SceneFlow` (`ToHome`, `ToGame`), `LevelRuns` (`RunForPlayer`, `RunForTest(level)`).

## Rules
- App-wide state (coins, progress, audio and haptics choices) lives only in the Boot scope; content scopes are rebuilt on every switch and name `BootScope` as parent, so play mode always starts from Boot.
- Boot holds the only `AudioListener`, `EventSystem` and the fade, which is also the Game scene's `ISceneTransition`.
- `SceneFlow` is the one place that names scenes. It uses `Application.exitCancellationToken`, which outlives the scene being left, and returns no task: nothing waits on a switch.
- The run is chosen once per Game scene (`ILevelRun`: `Level`, `Number`, `Advance`). `Advance` is called after a win; a run that keeps progress has saved it when `Advance` returns. `PlayerRun.Advance` completes the player's level. `TestRun` (from the level editor) never touches progress; it moves to the catalog's next level, and a level outside the catalog continues with the player's level.
- After the last catalog level the progression loops to the first.

Related: [Play](Play.md), [Home](Home.md), [Scenes](../../Base/Systems/Scenes.md)
