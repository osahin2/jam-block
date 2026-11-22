# Assemblies, features and folders

> Summary: Game code is split into four assemblies by role (data, logic, Unity runtime, editor) with one-way references, so the separation of data, logic and view is checked by the compiler. Inside each assembly code is grouped by feature, and each feature is reached through one entry class.

## Assemblies
| Assembly | Folder | Holds | References | Engine |
|---|---|---|---|---|
| `Game.Data` | `Assets/Game/Data/` | The level format (board size, blocks, doors, timer) and other saved data: serializable types with no behavior beyond queries on their own geometry (`Cell.IsInside`, `DoorData.Covers`, `FitsSide`, `Overlaps`) | - | `noEngineReferences: true` |
| `Game.Logic` | `Assets/Game/Logic/` | Rules: board state, moves, exits, win and fail, timer, coin balance | `Game.Data` | `noEngineReferences: true` |
| `Game` | `Assets/Game/Runtime/` | Views, input, flows, UI screens, settings assets, lifetime scopes and installers | `Game.Data`, `Game.Logic`, `Core.*`, VContainer, UniTask | yes |
| `Game.Editor` | `Assets/Game/Editor/` | The level editor window, its drawing and tools, other editor tooling | all of the above, `UnityEditor` | editor only |
| `Game.Tests` | `Assets/Game/Tests/` | EditMode tests, folders mirror the features | what each test needs | editor only |

- References point one way: `Game.Editor -> Game -> Game.Logic -> Game.Data`. Only the tests reference `Game.Editor` (for its edit operations and tools); nothing references the tests.
- `Game.Logic` cannot reach `UnityEngine`, so a rule cannot touch a transform, a `MonoBehaviour` or `Time`. Time arrives as a parameter (`Tick(float deltaSeconds)`), randomness as an injected seed or interface.
- `Game.Data` is the only format the editor writes and the game reads. Neither side has a private copy of the format. The ScriptableObject that stores a level (`LevelAsset`) is in `Game`, since it needs the engine; it holds a `Game.Data` type and nothing else.
- `autoReferenced: false` for every game assembly except where Unity needs it; namespaces follow assembly and folder (`Game.Logic.Play`).

## Features
A feature is something added or removed as a unit: the play session, the home screen, settings, the coin wallet, the level editor. An entity (a block, a door) is not a feature.

- **One entry class per feature.** The rest of the game calls only it, e.g. `PlaySession.Start(LevelData level, Action onWon, Action onFailed)`. Its result comes back as a return value or through the callback given to the entry; the feature does not publish events for others to find.
- **Everything else in the feature is used only inside it.** Within one assembly `internal` cannot enforce this, so it is a review rule: a class outside the feature that names anything but the entry class (and the data types it takes or returns) is a finding. A feature's UI screens, and the settings asset whose values they show, are entry points too: a scene's flow opens them through Core's `IUIService` and passes what they show and the callbacks of their buttons (`GameEntry` opening `LevelHud`, `HomeEntry` opening `HomeScreen`). So are a component that other features' prefabs nest (`CoinCounter`, `PressFeedback`, `PopupEntrance`) and a presentation service that other features' flows call (`CoinFlight`).
- **The flow inside a feature** is one orchestrator that owns the order of steps; it reads top to bottom (see [Logic and view](LogicAndView.md)).
- A feature's code spreads over the assemblies by role, under the same feature folder name in each: the play rules in `Logic/Play/` and their views in `Runtime/Play/`; the level format in `Data/Levels/` and its asset in `Runtime/Levels/`.

## Folders
Feature first; inside a feature, code and assets apart; folder names from fixed lists, so where a file goes is never a choice. The examples are the play feature.

```
Data/<Feature>/, Logic/<Feature>/       code only
  <Concept>/                              when the feature holds more than one concept (Logic/Play/Board/, Drag/, Attempt/)
Runtime/<Feature>/, Editor/<Feature>/   nothing at the feature root but these:
  Scripts/                                code, always in role folders
    <Role>/                                 Scopes, Flow, Services, Presentation, UI, Settings, Content, Installers
      <Concept>/                              when the role holds more than one concept (Presentation/Board/, Blocks/, Input/)
  Scenes/ Installers/ Settings/ Prefabs/ Materials/ Shaders/ Levels/ Layouts/ Styles/   assets by Unity type, only those present
    <Concept>/                              when the asset folder holds more than one concept, named after the code's (Prefabs/Board/, Blocks/, UI/)
Tests/<Feature>/                        the folders of the code under test, without Scripts/ (Tests/Play/Attempt/)
```
- **Roles** in `Scripts/`: `Scopes` lifetime scopes; `Flow` entry classes and orchestrators; `Services` plain classes registered in a scope that other code calls; `Presentation` views, builders, layouts, input; `UI` screen scripts; `Settings` settings ScriptableObject classes; `Content` ScriptableObject classes whose assets are authored content (`LevelAsset`); `Installers` installer classes. Editor features use their own roles (the level editor: `Window`, `Views`, `Document`, `Tools`, `Selection`, `Edits`, `Levels`, `Play`, `Preview`, `Solving`, and `Settings` as above; model import: `Importers`).
- **Asset folders**: `Scenes`, `Installers`, `Settings` (settings and catalog assets), `Prefabs`, `Materials`, `Shaders`, `Levels` (level assets), `Layouts` (UXML), `Styles` (USS). A new kind of asset is added to this list before its folder is made.
- **Concepts in asset folders** take the name of the code's concept folder whose views the assets make up (`Prefabs/Board/` for the ground and frame pieces of `Presentation/Board/`, `Prefabs/Blocks/` for the block and its pieces of `Presentation/Blocks/`), and `UI/` for the screens of `Scripts/UI/`, so an asset's concept is never a new name. An asset folder with one concept stays flat.
- A folder split into concepts holds no files of its own; a concept folder may hold one file. Role folders in `Scripts/` exist even for one file.
- A scene's lighting sits next to it in `Scenes`: its lighting settings (`<Scene>Lighting.lighting`) and the folder Unity names after the scene for the lighting it generates (`Scenes/Game/`), whose place Unity fixes.
- `Bootstrap/` is the scenes' composition: each scene's scope (`Scopes`), the flow that starts it (`Flow`), the scene switch (`Services`), the scene files (`Scenes`) and the UI root the scenes share (`Prefabs`). Feature installers register only their feature's services.
- An assembly folder holds only its asmdef and feature folders.
- Unity's special folder names (`Editor`, `Resources`, `Plugins`, `Gizmos`, `StreamingAssets`) are never used inside a feature: they change what is compiled or built. The assembly folder `Editor/` is governed by its asmdef.
- Namespaces stop at the feature (`Game.Play`, `Game.Logic.Play`); deeper folders are for finding files only.
- Supplied art goes under `Assets/Art/` as delivered, so it can be told apart from code and replaced as a unit.
- Moving files: through Unity (`AssetDatabase.MoveAsset`), never on disk, so GUID references survive. Commit a move on its own.
