# Plan

> Summary: The order of work for the case, the decisions still open, and the current status. Update the status at the end of every session.

## Status
- **2026-11-16, phase 0 done.** Unity 2022.3.62f2 with Android support (Rosetta 2 is required: the 2022.3 Package Manager server is x86_64); project created from the Universal 3D template; docs, conventions, hook and skills copied in. Core copied from the base template (commit 3cbd1db) with only `com.unity.ugui` 1.0.0 changed (list in `Packages/com.base-setup.core/CLAUDE.md`); its prefabs import with all scripts resolved; all 63 Core EditMode tests pass in batch mode. Manifest trimmed to the packages the game uses, plus Input System 1.14.0 (the only active input handler) and the OpenUPM registry for VContainer and UniTask. Template samples removed; Android build target, portrait only, IL2CPP ARM64, product "Rollic Case Study", bundle id `com.casestudy.blockslide`, all set by an editor script. Skill `unity-editor` rewritten for the batch-mode workflow. Git: `main` with the first commit, work continues on `develop`; no remote.
- **Next:** Phase 1, in a new session, in this order:
  1. Read the case PDF and check `Case.md` against it.
  2. Study the reference game on the developer's iPhone. App Store apps do not run in the iOS Simulator, so the phone is mirrored: the macOS iPhone Mirroring app shows and controls it, and Claude drives that window with computer use (screenshots, clicks, drags). Fallback: the developer screen-records on the phone and sends the clip; frames are extracted on the Mac. Look at movement, collisions, doors and exits, timer, win and fail flow, home and settings screens. Notes on mechanics only go to `Docs/Game/Reference.md`; no art, names or levels are copied.
  3. Settle the open decisions below with the developer and write them to [Decisions](Decisions.md); then the assemblies and game code.

## Phases
Each phase ends with compile, EditMode tests green, notes and index updated, a `--no-ff` merge into `develop`.

0. **Setup**: project, Core port, docs, skills.
1. **Scope and decisions** (with the developer): settle the open decisions below; download the supplied assets into `Assets/Art/` and review them (problems found go to the README feedback section); write the first entries in [Decisions](Decisions.md); create the four game assemblies and the test assembly.
2. **Data and logic** (`Game.Data`, `Game.Logic`): level format (board size, blocks with shape and color, doors with side, position, width and color, timer), board state, moves and collisions, exits through a matching door, win when the board is empty, fail when the timer reaches zero, coin wallet rules. EditMode tests first.
3. **Level editor** (`Game.Editor`): an editor window that draws the board; set width and height, paint blocks, place doors on the edges, set the timer; create, open, save, play from the editor; undo. Author the 5 levels with it. Usability check: make a level from scratch without reading code.
4. **Gameplay** (`Game`): board and block views, drag input, door views, slide and exit animations, HUD (timer, coins, level number), pause with resume, restart, home; fail popup (restart, home); win then next level; coins saved through Core.Save.
5. **Meta screens**: home (top UI with coins and lives placeholder, level button, settings button, placeholder tab bar, touch feedback), settings (sound, music, vibration toggles bound to Core.Audio and Core.Haptics, close). Check layout at 1080 x 1920 and 20:9.
6. **Delivery**: Android build (IL2CPP, ARM64), a short recording of the full flow, README written from `Decisions.md`, `WorkLog.md` and the known issues, final pass for dead code and style.
7. **Optional, only if 1-6 are complete**: level validation in the editor (solver warns before saving), then one optional mechanic.

## Open decisions (ask before phase 2)
Recommendations are a starting point for the discussion, not settled.

| Decision | Options | Leaning |
|---|---|---|
| Level storage | One ScriptableObject asset per level holding `Game.Data` types, or one JSON file per level | ScriptableObject: undo, inspector and references come free in the editor; the data types stay plain and engine-free |
| `Game.Data` engine references | `noEngineReferences: true` like Logic, or engine types allowed (`Vector2Int`, `Color`) | Engine-free with own `Cell` and a color id; the view maps ids to the supplied art |
| Movement model | Free drag cell by cell along free cells (as in the reference genre), or swipe to slide until blocked | Decide with the supplied assets and level samples |
| Motion | A tweening library (DOTween), or a few hand-written no-alloc motions in views | Hand-written: the motions are few (slide, exit, popup) and allocation stays under control |
| Scenes | Boot + Home + Game scenes through Core.Scenes, or one content scene with screens | Boot + Home + Game: restart and leaving a level rebuild cleanly |
| Coin rules | Reward per level from level data or one global setting | One global setting unless the assets suggest otherwise |
| Android vibration | Core.Haptics has no Android backend (silent there): add one to Core (Android `Vibrator` through JNI), or let the toggle only show and save its state | Add the backend to Core if phase 5 has time; the case only requires the toggle state |
| Delivery | `.apk`, a recording, or both | Both if time allows; the recording is cheap once the build runs |
| Submission | Private repository shared with the reviewers, or an archive | Decide at delivery |

## Risks
- The editor is the second most weighted criterion; budget real time for its usability, not only for its function.
- The live `unity command` bridge does not work on 2022.3; every editor action is a batch-mode run or an editor script, and batch mode cannot open the project while the editor has it open.
- Supplied assets may not match the expected grid or colors; review them in phase 1.
