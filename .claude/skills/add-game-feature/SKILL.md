---
name: add-game-feature
description: Add a feature, a rule, a UI screen, a settings asset or a level-editor tool to the case game - which assembly it goes in, its entry class, folders, installer registration, tests and docs. Use when building or extending game code under `Assets/Game/`.
---

# Adding to the game

Read the conventions first: `Docs/Conventions/Assemblies.md`, `LogicAndView.md`, `SettingsAndInstallers.md`, `Communication.md`. Check `Docs/Game/Plan.md` for the phase and its scope; do not build optional scope early.

## Where code goes
| It is... | Assembly | Folder |
|---|---|---|
| Level format or saved data | `Game.Data` | `Assets/Game/Data/<Feature>/` |
| A rule, a state change, a result type | `Game.Logic` | `Assets/Game/Logic/<Feature>/` |
| A view, input, a flow, a screen, settings, an installer | `Game` | `Assets/Game/Runtime/<Feature>/<Role>/` |
| Level editor or other editor tooling | `Game.Editor` | `Assets/Game/Editor/<Feature>/` |
| A test | `Game.Tests` | `Assets/Game/Tests/<Feature>/` |

If `Game.Logic` needs something from `UnityEngine`, the design is wrong: pass the value in (time, a random seed) or move the code to `Game`.

## A new feature
1. Name its entry class first and write its signature: what it takes (data types), what it returns or calls back with. Callers will know only this class.
2. Name where it varies before writing it: several kinds of one thing (editor tools, block types) or several states (paused, won, failed). Choose the pattern there: a strategy per kind looked up once, one state value or type, never a switch over kinds. Where nothing varies, write plain code. Use what Unity and Core already provide (Unity's `Undo`, Core's pools and screens) instead of writing it again.
3. Data types in `Game.Data`, rules in `Game.Logic` with EditMode tests before any view.
4. Settings: one ScriptableObject class per concern in `Runtime/<Feature>/Settings/`; create the asset with an editor script (skill `unity-editor`).
5. Installer: a sealed `ScriptableObjectInstaller` (Core.Installers) that registers the settings (`RegisterInstance`) and the feature's services; create the asset and add it to the scene scope's installer list with an editor script. The installer asset test then checks its references.
6. Views per `LogicAndView.md`: they show state and forward input, decisions stay in the logic; prefab views get settings through `Init`, scene views through `[Inject] Construct`.
7. A note in `Docs/Game/Systems/<Feature>.md` (template `Docs/Templates/System.md`) and its line in `Docs/00-Index.md`. A choice a reviewer could ask about: an entry in `Docs/Game/Decisions.md`.

## A new UI screen
A prefab variant of Core's `UIScreenTemplate`, script and prefab together in the feature's `UI/`, added to the feature's `UIScreenCatalog`. Buttons the flow reacts to are named callbacks in the screen's `Show` method. Every button gives visual feedback on touch.

## A new level
Made in the level editor only, never by editing the saved file. Format: the level data note in `Docs/Game/Systems/`.

## Editor windows (UI Toolkit on 2022.3)
Editor windows are UI Toolkit only: layout in UXML, style in USS, code in C#. Never mix in IMGUI (`IMGUIContainer`, `OnGUI`); if UI Toolkit cannot do something, the whole window goes back to IMGUI, decided with the developer.

Examples online are mostly Unity 6. These do not exist in 2022.3 and do not compile:
| Unity 6 | Use instead |
|---|---|
| `[UxmlElement]`, `[UxmlAttribute]` | build custom elements in C# and add them to an empty container from the UXML; `UxmlFactory`/`UxmlTraits` only if it must be placed from UXML |
| runtime binding: `dataSource`, `SetBinding`, `[CreateProperty]` | editor binding to a `SerializedObject`: `Bind`, `BindProperty`, `bindingPath` |
| `ToggleButtonGroup` | `Button`s with a selected USS class set by a few lines that keep one selected (more visible than `ToolbarToggle`) |
| `TabView`, `Tab` | plain containers and buttons |
| `RegisterCallbackOnce` | `RegisterCallback`, then `UnregisterCallback` in the handler |
| USS `aspect-ratio` | size square cells in a `GeometryChangedEvent` handler |

Available in 2022.3: `Painter2D`, `ListView` and `MultiColumnListView` (`selectionChanged`), `HelpBox`, `DropdownField`, `ToolbarToggle`, `TrackSerializedObjectValue`, `TrackPropertyValue`, `CapturePointer`, USS transitions and variables, `[Shortcut]`, `EditorWindow.hasUnsavedChanges`. When unsure whether an API exists, search the installed editor's docs before writing it: `UnityEngine.UIElementsModule.xml`, `UnityEditor.UIElementsModule.xml`, `UnityEditor.CoreModule.xml` in `/Applications/Unity/Hub/Editor/2022.3.62f2/Unity.app/Contents/Managed/UnityEngine/`.

Behavior to plan for:
- Domain reload rebuilds the window through `CreateGUI`: keep its state (open asset, tool, color, working copy) in serialized fields; a working-copy ScriptableObject with `HideFlags.DontSave` survives reloads and keeps its undo history. Not `HideAndDontSave`: it includes `NotEditable`, and fields bound to the object refuse edits.
- Editor binding polls the `SerializedObject`: after a change made from code, refresh the view yourself; undo and bound fields reach it through `TrackSerializedObjectValue`. One refresh method draws everything that depends on the data.
- Drag painting: pointer events on one container element with `CapturePointer`, converting the local position to a cell; per-element handlers stop receiving events while the pointer is captured. Hover per element through USS.
- Keyboard shortcuts through `[Shortcut(id, typeof(TheWindow), KeyCode.X)]`, not `KeyDownEvent`, which only reaches the focused element; designers can rebind them in the Shortcuts window.
- Resizable panes: `TwoPaneSplitView` from UXML did not lay out in the level editor (panes stacked in a column, no drag line); use a thin handle element with a `PointerManipulator` that sets the pane's `style.width` or `style.height` and keeps the size in a serialized field (`PaneResizer`).
- Batch mode does not draw windows: edit operations live in a plain class with EditMode tests; the window only draws and forwards input, and the developer checks it in the editor.

## Always
- Compile, check for errors, run EditMode tests (skill `unity-editor`).
- No allocations in per-frame code; pool what spawns often.
- Anything another game could use unchanged belongs in Core (skill `add-core-module`).
- When the feature is done, ask the developer whether to review it (skill `review-code`).
