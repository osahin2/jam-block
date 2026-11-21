# rollic-case-study

A 7-day game developer case study: a vertical slice of a block-sliding color puzzle for Android (portrait, 1080x1920) with a home screen, settings, five levels (timer, coins, fail popup, pause, restart) and a visual level editor for designers. Requirements: `Docs/Game/Case.md`. Plan and status: `Docs/Game/Plan.md`. Reviewers weigh, in order: architecture, editor usability, code quality, correctness, performance, scope control.

Everything written into the repository is English.

## Stack
Unity 2022.3.62f2 (required by the case; never upgrade), URP 14, Android first. Input System, uGUI 1.0 with TextMeshPro, VContainer 1.19.0 and UniTask 2.5.11 (OpenUPM, pinned), DOTween 1.3.045 (`Assets/Plugins/Demigiant/`), UI Particle 4.14.2 (OpenUPM, pinned; ParticleSystems in the UI), Unity Test Framework (EditMode first). Core: `Packages/com.base-setup.core/`, copied from the base template.

## Architecture
- Four game assemblies, dependencies one way: `Game.Editor -> Game -> Game.Logic -> Game.Data` (`Docs/Conventions/Assemblies.md`).
  - `Game.Data`: the level format and other saved data; the editor writes it, the game reads it. References no other Game assembly.
  - `Game.Logic`: rules, plain C#, `noEngineReferences: true`, references only `Game.Data`.
  - `Game`: views, input, flows, UI, scopes and installers; the only runtime assembly that references Core.
  - `Game.Editor`: the level editor and other editor tooling, editor-only.
- Each feature has one entry class that the rest of the game calls, e.g. `PlaySession.Start(level, onEnded)`; the result comes back as a return value or the callback passed to it. Everything else in the feature is used only inside it.
- MonoBehaviours only present state and forward input; decisions live in `Game.Logic`.
- Systems talk through return values and calls on injected contracts. No static singletons, no service locator. An event bus only for facts that unrelated observers watch, never for flow (`Docs/Conventions/Communication.md`).
- Dependencies arrive through one constructor without `[Inject]`; `[Inject]` methods only on MonoBehaviours.
- Tuning values and content references live in ScriptableObject settings assets, one per concern, registered by the feature's `ScriptableObjectInstaller` (Core.Installers).
- A class has one reason to change; no classes that only forward. No dead or commented-out code.
- Async (UniTask) only where something is really awaited.
- Member order, one type per file, naming, interface contracts (Liskov) and extension points (open-closed): `Docs/Conventions/CodeStyle.md`. Comments only for why (CodeStyle), and Claude adds none to game code: a hook refuses any edit under `Assets/Game/` that adds a comment; a needed one is for the developer to add. Core, a package, keeps short comments on its public contracts. C# files change only through Edit and Write, never shell commands, so the hooks see every change; they check the member order and the remaining comments after each edit; fix what they report.

## Hard rules
- Never edit `.unity`, `.prefab`, `.asset`, `.meta` or `ProjectSettings/` by hand; use editor scripts (skill `unity-editor`). Never create `.meta` files.
- Never read or search `Library/`, `Temp/`, `obj/`, `Logs/`, `Build/`, `UserSettings/`.
- Only APIs that exist in Unity 2022.3 and the pinned package versions.
- Art comes from the supplied case assets only; the one addition is the UI font, Lilita One (SIL OFL, `Assets/Plugins/LilitaOne/`), as none was supplied.
- No allocations in per-frame code (new, LINQ, string concatenation, boxing, closures); cache component lookups; pool objects spawned often.

## Workflow
- Editor work: skill `unity-editor` (batch mode and editor scripts; the live `unity command` bridge needs Unity 6). After script changes compile, check for errors and run EditMode tests.
- Git: work on `develop` or a short-lived branch from it, merged into `develop` with `--no-ff` once tests pass. Merge into `main` or push only when asked. The repository stays private.
- A choice a reviewer could ask about gets a short entry in `Docs/Game/Decisions.md`; the README's decisions section is written from it.
- When a system's behavior changes, update its note and the index. At the end of a session, update the status in `Docs/Game/Plan.md` and add a line to `Docs/Game/WorkLog.md`.
- CLAUDE.md files hold only rules that are always true. Task workflows are skills in `.claude/skills/`; system details are in the docs.

## Docs
`Docs/00-Index.md`: case requirements, plan, decisions, conventions, Core systems, game systems.
