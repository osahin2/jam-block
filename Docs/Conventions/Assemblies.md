# Assemblies, features and folders

> Summary: Game code is split into four assemblies by role (data, logic, Unity runtime, editor) with one-way references, so the separation of data, logic and view is checked by the compiler. Inside each assembly code is grouped by feature, and each feature is reached through one entry class.

## Assemblies
| Assembly | Folder | Holds | References | Engine |
|---|---|---|---|---|
| `Game.Data` | `Assets/Game/Data/` | The level format (board size, blocks, doors, timer) and other saved data: serializable types with no behavior beyond validation of their own values | - | `noEngineReferences: true` |
| `Game.Logic` | `Assets/Game/Logic/` | Rules: board state, moves, exits, win and fail, timer, coin balance | `Game.Data` | `noEngineReferences: true` |
| `Game` | `Assets/Game/Runtime/` | Views, input, flows, UI screens, settings assets, lifetime scopes and installers | `Game.Data`, `Game.Logic`, `Core.*`, VContainer, UniTask | yes |
| `Game.Editor` | `Assets/Game/Editor/` | The level editor window, its drawing and tools, other editor tooling | all of the above, `UnityEditor` | editor only |
| `Game.Tests` | `Assets/Game/Tests/` | EditMode tests, folders mirror the features | what each test needs | editor only |

- References point one way: `Game.Editor -> Game -> Game.Logic -> Game.Data`. Nothing references `Game.Editor` or the tests.
- `Game.Logic` cannot reach `UnityEngine`, so a rule cannot touch a transform, a `MonoBehaviour` or `Time`. Time arrives as a parameter (`Tick(float deltaSeconds)`), randomness as an injected seed or interface.
- `Game.Data` is the only format the editor writes and the game reads. Neither side has a private copy of the format. The ScriptableObject that stores a level (`LevelAsset`) is in `Game`, since it needs the engine; it holds a `Game.Data` type and nothing else.
- `autoReferenced: false` for every game assembly except where Unity needs it; namespaces follow assembly and folder (`Game.Logic.Play`).

## Features
A feature is something added or removed as a unit: the play session, the home screen, settings, the coin wallet, the level editor. An entity (a block, a door) is not a feature.

- **One entry class per feature.** The rest of the game calls only it, e.g. `PlaySession.Start(LevelData level, Action<PlayResult> onEnded)`. Its result comes back as a return value or through the callback given to the entry; the feature does not publish events for others to find.
- **Everything else in the feature is used only inside it.** Within one assembly `internal` cannot enforce this, so it is a review rule: a class outside the feature that names anything but the entry class (and the data types it takes or returns) is a finding.
- **The flow inside a feature** is one orchestrator that owns the order of steps; it reads top to bottom (see [Logic and view](LogicAndView.md)).
- A feature's code spreads over the assemblies by role, under the same feature folder name in each: the play rules in `Logic/Play/` and their views in `Runtime/Play/`; the level format in `Data/Levels/` and its asset in `Runtime/Levels/`.

## Folders inside `Assets/Game/Runtime/`
```
Bootstrap/      lifetime scopes (composition roots)
Scenes/         scenes; they compose several features
<Feature>/      one per feature
  Presentation/   views, layouts, spawners, outcome players, input
  Flow/           the orchestrator and the entry class
  UI/             screen scripts and prefabs together, plus the feature's screen catalog
  Settings/       settings ScriptableObjects and their classes
  Prefabs/, Materials/, Content/   only when the feature has such assets
  <Feature>Installer.asset
```
- Role subfolders only when needed; no empty skeleton.
- Supplied art goes under `Assets/Art/` as delivered, so it can be told apart from code and replaced as a unit.
- Moving files: through Unity (`AssetDatabase.MoveAsset`), never on disk, so GUID references survive. Commit a move on its own.
