---
name: add-core-module
description: Add a new module (or a new public type) to the Core package `Packages/com.base-setup.core/` - folder, asmdef, interface, internal implementation, installer, tests, system note and index. Use when a game-agnostic system is promoted into Core or a Core module's public API grows.
---

# Adding a Core module

Core's home is the base template (`~/UnityRepos/unity-base-template`, Unity 6). This project holds a copy adapted to Unity 2022.3. A change that is not specific to 2022.3 and does not break Unity 6 is made here and then carried back to the template (compile and test it there too); a 2022.3-only change stays here and is listed in `Packages/com.base-setup.core/CLAUDE.md`.

First check the admission rule: the game uses it now and it is game-agnostic, or every game will certainly need it. If in doubt, build it in the game and promote it when a second use appears. No game vocabulary anywhere in the module or its note.

## Files
Module `Foo`, everything under `Packages/com.base-setup.core/`:
- `Runtime/Foo/Core.Foo.asmdef`: `"name": "Core.Foo"`, `"rootNamespace": "Core.Foo"`, `"autoReferenced": false`, references by assembly name (other `Core.*` modules only if acyclic, `VContainer` if it has an installer, `UniTask` only if it has an async API). Copy the shape of `Runtime/Audio/Core.Audio.asmdef`.
- `IFooService.cs` (public): the contract the game injects. Comments only for what the signature cannot say (persistence, threading, what a call does when disabled).
- `FooService.cs` (internal, sealed): one constructor, no `[Inject]`, no VContainer reference. Player settings are a `[Serializable]` document loaded through `ISaveService` in the constructor and saved on each change; setting the current value writes nothing.
- `FooInstaller.cs` (public static): `RegisterCoreFoo(this IContainerBuilder builder, ...)`, the only file that names VContainer. Validate arguments (`ArgumentNullException`).
- Tuning values: a `FooServiceSettings` ScriptableObject passed to the installer.
- A backend the game may replace (platform effects): a public `IFooBackend` and an installer overload that takes it; native backends stay internal and are chosen at compile time.
- `AssemblyInfo.cs`: `[assembly: InternalsVisibleTo("Core.Foo.Tests")]`.
- Native plugins: `Runtime/Foo/Plugins/iOS/*.mm`; check the importer's platforms with `PluginImporter` through `script eval`, never by editing the `.meta`.

## Tests
`Tests/Foo/Core.Foo.Tests.asmdef` (Editor only, `overrideReferences: true`, `nunit.framework.dll`, `UNITY_INCLUDE_TESTS`; copy `Tests/Haptics/`). EditMode, fakes for other modules (an in-memory `ISaveService` that round-trips JSON). Cover defaults, saved and restored settings, no write for an unchanged value, the installer path the game uses.

## Wiring and docs
- Register it in the game's root scope (after `RegisterCoreSave()` if it saves) and add `Core.Foo` to `Game.asmdef` references only when the game calls it. `Game.Data` and `Game.Logic` never reference Core.
- Write `Docs/Base/Systems/Foo.md` from `Docs/Templates/System.md`: what it does, entry points, how it works (with a one-line why where a choice is not obvious), rules, tests, what is not verified yet. No links into `Docs/Game/`.
- Add a line to `Docs/00-Index.md` (Core) and a row to the module table in `Docs/Base/Architecture/Overview.md`.
- Do not add the module to any CLAUDE.md.
- Compile, check for errors, run EditMode tests (skill `unity-editor`).
