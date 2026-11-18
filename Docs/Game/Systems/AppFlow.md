# App flow

> Summary: The app's three scenes and what lives where: Boot stays loaded and holds the app-wide services, Home and Game are content scenes switched on top of it. Which level the player plays and which comes after a win.

## Code
- Folder: `Assets/Game/Runtime/Bootstrap/`, `Assets/Game/Runtime/Home/`, `Assets/Game/Runtime/Levels/`, `Assets/Game/Runtime/Scenes/`
- Assembly: `Game`
- Entry points: `SceneFlow` (`Runtime/Bootstrap/SceneFlow.cs`): `ToHome()`, `ToGame()`; `LevelSequence` (`Runtime/Levels/LevelSequence.cs`): `Current`, `Advance(won)`

## How it works
- **Scenes.** The Build Settings list Boot, Home, Game; Core.Scenes keeps play mode starting from Boot, also in the editor. Boot is never unloaded; Home and Game are switched on top of it, one at a time, behind Core's fade ([Scenes](../../Base/Systems/Scenes.md)).
- **Boot** holds `BootScope`, the root scope: Core's Save, Audio, Haptics and Scenes, then its installers, `LevelsInstaller` (the catalog, Core's level progression, `LevelSequence`, `EditorPlayRequest`) and `CoinsInstaller` (the saved wallet, [Coins](Coins.md)). It also holds the app's only `AudioListener`, the only `EventSystem` (Input System UI module) and the fade overlay. `BootEntry` opens Game straight away when the level editor asked to play a level, else Home.
- **Home** holds a camera, Core's `UIRoot` and `HomeScope` (a child of `BootScope`). `HomeInstaller` registers the screen catalog and `HomeEntry`, which opens `HomeScreen` with the player's level number and coin count; the level button calls `SceneFlow.ToGame`. A placeholder for phase 5, which adds the top bar, settings, tab bar and touch feedback ([Plan](../Plan.md)).
- **Game** holds `GameScope`, also a child of `BootScope` ([Play](Play.md)).
- **SceneFlow** is the one place that names scenes. Callers only ask; it runs Core's switch with `Application.exitCancellationToken`, which outlives the scene being left as Core.Scenes requires, and does not return a task: nothing waits on a switch.
- **LevelSequence** maps Core's progression onto the `LevelCatalog`. `Current` is the catalog level at the progression's index. `Advance(won)` records a win and returns the level to play next:
  - the player's current level: completes it (saved by Core) and returns the new current level;
  - another catalog level, played from the level editor: returns the catalog's next level (the first after the last), progress unchanged;
  - a level outside the catalog: returns the player's current level, progress unchanged.

## Rules & invariants
- App-wide state (coins, progress, audio and haptics choices) lives only in the Boot scope; content scopes are rebuilt on every switch.
- Content scopes name `BootScope` as their parent (`parentReference`), so a content scene opened alone in play mode cannot start; Core.Scenes prevents that by starting from Boot.
- After the last catalog level the progression loops back to the first (`LevelLoop` start index 0).

## Tests
- `Tests/Levels/LevelSequenceTests`: current level, a win of the current level completes it, a win of another catalog level moves to the next one (wrapping) without progress, a level outside the catalog goes back to the current level.
- `Tests/Levels/EditorPlayRequestTests`: `IsPending` from a request until it is taken.
- `Tests/Bootstrap/InstallerAssetTests`: the Boot and Home installers have no unassigned reference.
- The scene switch itself is Core's, tested there; the scenes are checked by playing them in the editor.

## Related
- [Play](Play.md), [Coins](Coins.md), [Levels](../../Base/Systems/Levels.md), [Scenes](../../Base/Systems/Scenes.md), [UI](../../Base/Systems/UI.md)
