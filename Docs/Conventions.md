# Conventions

> How the game code is organized and written: assemblies, features and folders; logic and view; communication and dependency injection; settings and installers; code style.

## Assemblies, features and folders

| Assembly | Folder | Holds | References |
|---|---|---|---|
| `Game.Data` | `Assets/Game/Data/` | The level format and saved data; no engine | - |
| `Game.Logic` | `Assets/Game/Logic/` | Rules; no engine | `Game.Data` |
| `Game` | `Assets/Game/Runtime/` | Views, input, flows, UI, settings, scopes, installers | the above, Core, VContainer, UniTask |
| `Game.Editor` | `Assets/Game/Editor/` | Level editor and tooling | all of the above |
| `Game.Tests` | `Assets/Game/Tests/` | EditMode tests, mirroring the code's folders | what each test needs |

- References point one way: `Game.Editor -> Game -> Game.Logic -> Game.Data`. Logic gets time and randomness as parameters. `LevelAsset` is in `Game`, holding only a `Game.Data` type.
- **A feature** is something added or removed as a unit (play session, home, coins), not an entity. It has **one entry class** the rest of the game calls; the result comes back as a return value or a callback. A feature's screens, a component other prefabs nest (`CoinCounter`, `PressFeedback`) and a presentation service other flows call (`CoinFlight`) are entry points too. Anything else named from outside the feature is a review finding.
- **Folders:** feature first, code and assets apart, names from fixed lists.
  - `Data/<Feature>/`, `Logic/<Feature>/`: code only, with concept subfolders when a feature holds more than one concept.
  - `Runtime/<Feature>/`, `Editor/<Feature>/`: only `Scripts/<Role>/` and asset folders. Roles: `Scopes`, `Flow`, `Services`, `Presentation`, `UI`, `Settings`, `Content`, `Installers` (editor features use their own, e.g. `Window`, `Tools`, `Edits`). Asset folders: `Scenes`, `Installers`, `Settings`, `Prefabs`, `Materials`, `Shaders`, `Levels`, `Layouts`, `Styles`; a new kind is added to this list first.
  - Concept subfolders inside a role or asset folder take the code's concept names (`Presentation/Board/`, `Prefabs/Board/`). A split folder holds no loose files.
  - `Bootstrap/` holds each scene's scope, entry flow and the scene files. Namespaces stop at the feature (`Game.Play`).
  - Unity's special folder names are never used inside a feature. Supplied art stays under `Assets/Art/`.
  - Files move only through `AssetDatabase.MoveAsset`, in a commit of their own.

## Logic and view
- Logic decides (moves, outcomes, balances) and reports through return values and queryable state. It never reads a clock, transform or screen.
- Views show and forward input; they hold no rule and nothing sets their transforms from outside.
- One flow class per feature owns the order of steps and reads top to bottom.
- **Data before presentation:** what the player earns or completes is saved on the action, before any animation; never in a tween's completion.
- Behavior that differs by kind lives with the kind (polymorphism or a strategy looked up once), never a switch over types. No machinery before a second case exists.
- Restart builds new logic from the level data.
- Dependencies: a spawned prefab view gets `Init(...)` from its spawner; a scene view gets `[Inject] Construct(...)`; plain classes get a constructor.

## Communication and dependency injection
Take the first that fits: (1) return value; (2) a feature's entry class with a callback; (3) a call on an injected contract; (4) one orchestrator for a multi-step flow; (5) a notification on the owning contract, for state with several readers; (6) an event bus, only for past-tense facts watched by unrelated observers (analytics, achievements). Never a bus for flow, requests, single-owner state or per-frame data. None exists yet; when needed it is a small typed Core bus, per scope.

- One constructor, no `[Inject]` on it; `[Inject]` methods only on MonoBehaviours.
- No static singletons, no service locator; `IObjectResolver` only inside factories registered in a scope.
- A root scope in Boot for app-wide services; a child scope per content scene runs the feature installers.
- Interfaces at boundaries (Core services, ports); inside the game, concrete classes until a second implementation or a test fake exists.
- Async (UniTask with a token) only where something is really awaited.

## Settings and installers
- Tuning values and content references live in a settings ScriptableObject per concern (what a designer tunes as a unit), class in `Scripts/Settings/`, asset in `Settings/`. A prefab's own children stay on the prefab; scene-only objects on the scene's scope.
- Each feature has one `ScriptableObjectInstaller` that registers its settings and services; a scene adds a feature by listing its installer.
- Spawned views get settings through `Init`; UI screens, created by the container, may use `[Inject]`.
- **Read when used:** code reads a setting each time it acts, so play-mode changes show on the next action. Tweens are built when they play and stopped through their target, never through a kept handle.
- A test loads every installer and fails on an unassigned reference.
- DOTween does not run in EditMode tests, so motion is checked in the editor; the state a motion ends in is tested.

## Code style
**Member order** (checked by `.claude/hooks/check_member_order.py` after every C# edit):
1. Constants and static members
2. Events
3. Serialized fields
4. Other fields
5. Properties
6. Constructors, `[Inject] Construct`, `Init`
7. Unity messages, in lifecycle order (`Awake`, `OnEnable`, `Reset`, `Start`, `FixedUpdate`, `Update`, `LateUpdate`, `OnDisable`, `OnDestroy`), then the others
8. Public, internal and protected methods
9. Private methods

Run it by hand on a file: `python3 .claude/hooks/check_member_order.py Assets/Path/To/File.cs`.

- One top-level type per file, named after it. Private fields `_camelCase`, locals and parameters `camelCase`, the rest `PascalCase`. Classes not designed for inheritance are `sealed`.
- **No comments in game code:** names and small named steps explain it; a contract, invariant or outside fact goes into the system note, design choices into Decisions. A hook refuses added comments. Core keeps short comments (two lines at most) on its public contracts.
- **Keep the contract (Liskov):** every implementation keeps the promise its contract states; no type checks on implementations, no `NotSupportedException`, no member that silently does nothing.
- **Open where it changes:** where kinds change (more are planned or a second exists), a new kind is a new class looked up once. A closed enum the rules own may stay a table with a test that every value has an entry. No extension point nobody asked for.
