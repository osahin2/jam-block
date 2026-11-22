# Core package

Game-agnostic systems copied into every project. Never reference `Game` or anything under `Assets/`, and no game vocabulary (names of a specific game's entities, rules or content).

- A system enters Core only if the game uses it now and it is game-agnostic, or every game will certainly need it. No speculative abstractions.
- One folder and one asmdef per module (`Runtime/<Module>/Core.<Module>.asmdef`, namespace `Core.<Module>`, `autoReferenced: false`); module references are acyclic.
- Public surface: service interfaces, the types they mention, one installer extension (`RegisterCore<Module>`), and a backend interface where a game may supply its own platform effect. Implementations are `internal`.
- Only the installer file references VContainer.
- EditMode tests per module in `Tests/<Module>/`, reaching internals through `InternalsVisibleTo`.

Adding or extending a module: skill `add-core-module`. Module notes: `Docs/Base/`.

## Unity 2022.3 port
Copied from the base template at commit 3cbd1db (Unity 6). The code, asmdefs and prefabs are unchanged; the prefabs import with all scripts resolved (uGUI 1.0 keeps the script GUIDs of uGUI 2.0). Only these differ from the template:
- `package.json`: `com.unity.ugui` 1.0.0 instead of 2.0.0.
- `Runtime/UI/UIInstaller.cs`: `RegisterUIScreens` registers a catalog through a scoped factory instead of `RegisterInstance`, which VContainer rejected for a second catalog in one scope (`Conflict implementation type`); `Tests/UI/UIInstallerTests.cs` covers it and `Core.UI.Tests.asmdef` references VContainer. Carry the fix back to the template.
- Haptics: the Android backend (`AndroidHapticBackend`, `AndroidHapticEffect`, `Editor/Haptics/` for the `VIBRATE` permission) and the appended `HapticPreset.Selection` (also in the iOS table) are new here. Carry them back to the template.
- The project's `Packages/manifest.json` (not part of Core): URP 14.0.12, Input System 1.14.0, Test Framework 1.1.33, TextMeshPro 3.0.7 as a separate package, no `com.unity.pipeline`; the OpenUPM registry block for VContainer 1.19.0 and UniTask 2.5.11 is the template's.
