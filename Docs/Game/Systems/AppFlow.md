# App flow

> Summary: The app's three scenes and what lives where: Boot stays loaded and holds the app-wide services, Home and Game are content scenes switched on top of it. Which level the player plays and which comes after a win.

## Code
- Folder: `Assets/Game/Runtime/Bootstrap/` (scopes, entry flows, `SceneFlow`, the scenes), `Assets/Game/Runtime/Home/`, `Assets/Game/Runtime/Levels/`
- Assembly: `Game`
- Entry points: `SceneFlow` (`Runtime/Bootstrap/Scripts/Services/SceneFlow.cs`): `ToHome()`, `ToGame()`; `LevelSequence` (`Runtime/Levels/Scripts/Services/LevelSequence.cs`): `Current`, `CompleteCurrent()`, `After(level)`

## How it works
- **Scenes.** The Build Settings list Boot, Home, Game; Core.Scenes keeps play mode starting from Boot, also in the editor. Boot is never unloaded; Home and Game are switched on top of it, one at a time, behind Core's fade ([Scenes](../../Base/Systems/Scenes.md)).
- **Boot** holds `BootScope`, the root scope: Core's Save, Audio, Haptics and Scenes, then its installers, `GameLevelsInstaller` (the catalog, Core's level progression, `LevelSequence`, `EditorPlayRequest`) and `CoinsInstaller` (the saved wallet, [Coins](Coins.md)). It also holds the app's only `AudioListener`, the only `EventSystem` (Input System UI module) and the fade overlay. `BootEntry` opens Game straight away when the level editor asked to play a level, else Home.
- **Home** holds a camera, Core's `UIRoot` and `HomeScope` (a child of `BootScope`). `HomeInstaller` registers the screen catalog; `HomeScope` registers `HomeEntry`, which opens `HomeScreen` with the player's level number and coin count; the level button calls `SceneFlow.ToGame`. A placeholder for phase 5, which adds the top bar, settings, tab bar and touch feedback ([Plan](../Plan.md)).
- **Game** holds `GameScope`, also a child of `BootScope` ([Play](Play.md)). Its `GameEntry` plays the first level and, when a level ends, the next one after a win or the same one after a fail, in the same scene. Which level is next depends on the run, chosen once when the scene starts: in the player's run a win completes their level (`LevelSequence.CompleteCurrent`); a run started from the level editor is a designer's test, so a win continues with the catalog's next level (`LevelSequence.After`) and never moves progress, also when the tested level is the player's current one.
- **SceneFlow** is the one place that names scenes. Callers only ask; it runs Core's switch with `Application.exitCancellationToken`, which outlives the scene being left as Core.Scenes requires, and does not return a task: nothing waits on a switch.
- **LevelSequence** maps Core's progression onto the `LevelCatalog`. `Current` is the catalog level at the progression's index. `CompleteCurrent()` records the player's win of their current level (saved by Core) and returns the new current level. `After(level)` returns the level that follows without touching progress:
  - a catalog level: the catalog's next level (the first after the last);
  - a level outside the catalog: the player's current level.

## Rules & invariants
- App-wide state (coins, progress, audio and haptics choices) lives only in the Boot scope; content scopes are rebuilt on every switch.
- Content scopes name `BootScope` as their parent (`parentReference`), so a content scene opened alone in play mode cannot start; Core.Scenes prevents that by starting from Boot.
- After the last catalog level the progression loops back to the first (`LevelLoop` start index 0).

## Tests
- `Tests/Levels/Services/LevelSequenceTests`: current level, completing the current level moves to the next one, the level after the player's current level, after another catalog level and after the last one (wrapping) leaves progress alone, a level outside the catalog goes back to the current level.
- `Tests/Levels/Services/EditorPlayRequestTests`: `IsPending` from a request until it is taken.
- `Tests/Bootstrap/InstallerAssetTests`: the Boot and Home installers have no unassigned reference.
- The scene switch itself is Core's, tested there; the scenes are checked by playing them in the editor.

## Related
- [Play](Play.md), [Coins](Coins.md), [Levels](../../Base/Systems/Levels.md), [Scenes](../../Base/Systems/Scenes.md), [UI](../../Base/Systems/UI.md)
