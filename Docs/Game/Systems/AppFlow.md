# App flow

> Summary: The app's three scenes and what lives where: Boot stays loaded and holds the app-wide services, Home and Game are content scenes switched on top of it. Which level the player plays and which comes after a win.

## Code
- Folder: `Assets/Game/Runtime/Bootstrap/` (scopes, entry flows, `SceneFlow`, the scenes), `Assets/Game/Runtime/Home/`, `Assets/Game/Runtime/Levels/`
- Assembly: `Game`
- Entry points: `SceneFlow` (`Runtime/Bootstrap/Scripts/Services/SceneFlow.cs`): `ToHome()`, `ToGame()`; `LevelRuns` (`Runtime/Levels/Scripts/Services/LevelRuns.cs`): `RunForPlayer()`, `RunForTest(level)`, each returning an `ILevelRun` (`Level`, `Number`, `Advance()`)

## How it works
- **Scenes.** The Build Settings list Boot, Home, Game; Core.Scenes keeps play mode starting from Boot, also in the editor. Boot is never unloaded; Home and Game are switched on top of it, one at a time, behind Core's fade ([Scenes](../../Base/Systems/Scenes.md)).
- **Boot** holds `BootScope`, the root scope: Core's Save, Audio, Haptics and Scenes, then its installers, `GameLevelsInstaller` (the catalog, Core's level progression, `LevelRuns`, `EditorPlayRequest`), `CoinsInstaller` (the saved wallet and the coin flight, [Coins](Coins.md)), `ButtonsInstaller` (the press feedback settings every button uses, [Buttons](Buttons.md)) and `PopupsInstaller` (the entrance settings every popup uses, [Popups](Popups.md)). It also holds the app's only `AudioListener`, the only `EventSystem` (Input System UI module) and the fade overlay, registered also as `ISceneTransition` so the Game scene covers a level change with the same fade ([Play](Play.md)). `BootEntry` opens Game straight away when the level editor asked to play a level, else Home.
- **Home** holds a camera, the game's UI root (`GameUIRoot`, a variant of Core's `UIRoot` that scales the UI to the screen's width, [Decisions](../Decisions.md)) and `HomeScope` (a child of `BootScope`). `HomeInstaller` registers the screen catalog; `HomeScope` registers `HomeEntry`, which opens `HomeScreen` with the player's level number and coin count ([Home](Home.md)); the level button calls `SceneFlow.ToGame`, the gear opens the settings popup ([Preferences](Preferences.md)). A placeholder for phase 5, which adds the top bar, settings, tab bar and touch feedback ([Plan](../Plan.md)).
- **Game** holds `GameScope`, also a child of `BootScope` ([Play](Play.md)), with its own `GameUIRoot`. Its `GameEntry` plays the first level and, when a level ends, opens the win or fail popup; continue plays the next level, retry the same one, home (on the fail popup and the settings popup) goes back to Home. Which level is played and what follows a win is the run's, chosen once when the scene starts (`ILevelRun`, a strategy, so no win asks where the run came from):
  - Every `ILevelRun` gives its level and the number the player sees; `Advance`, called after a win, moves on to the next level, and a run that keeps progress has saved it when `Advance` returns.
  - `PlayerRun` (`LevelRuns.RunForPlayer()`): the player's current level and number; `Advance` completes it (saved by Core), so the next level is the progression's next.
  - `TestRun` (`LevelRuns.RunForTest(level)`): a run started from the level editor is a designer's test. It plays the tested level, numbered by its place in the catalog, and `Advance` moves to the catalog's next level (the first after the last) without touching progress, also when the tested level is the player's current one. A level outside the catalog has no place, so it shows the player's number and advances to the player's current level.
- **SceneFlow** is the one place that names scenes. Callers only ask; it runs Core's switch with `Application.exitCancellationToken`, which outlives the scene being left as Core.Scenes requires, and does not return a task: nothing waits on a switch.
- **LevelRuns** makes the two runs over the `LevelCatalog` and Core's progression; it holds no state and no rule of its own.

## Rules & invariants
- App-wide state (coins, progress, audio and haptics choices) lives only in the Boot scope; content scopes are rebuilt on every switch.
- Content scopes name `BootScope` as their parent (`parentReference`), so a content scene opened alone in play mode cannot start; Core.Scenes prevents that by starting from Boot.
- After the last catalog level the progression loops back to the first (`LevelLoop` start index 0).

## Tests
- `Tests/Levels/Services/LevelRunsTests`: the player's run plays the level at the progression's index and its advance completes it; a test run plays the tested level with its catalog number, advances from the player's current level, from another catalog level and from the last one (wrapping) without progress; a level outside the catalog shows the player's number and advances to their level without progress.
- `Tests/Levels/Services/EditorPlayRequestTests`: `IsPending` from a request until it is taken.
- `Tests/Bootstrap/InstallerAssetTests`: the Boot, Home and Game installers have no unassigned reference.
- The scene switch itself is Core's, tested there; the scenes are checked by playing them in the editor.

## Related
- [Play](Play.md), [Coins](Coins.md), [Levels](../../Base/Systems/Levels.md), [Scenes](../../Base/Systems/Scenes.md), [UI](../../Base/Systems/UI.md)
