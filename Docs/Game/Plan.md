# Plan

> Summary: The order of work for the case, the decisions still open, and the current status. Update the status at the end of every session.

## Status
- **2026-11-16, phase 0 done.** Unity 2022.3.62f2 with Android support (Rosetta 2 is required: the 2022.3 Package Manager server is x86_64); project created from the Universal 3D template; docs, conventions, hook and skills copied in. Core copied from the base template (commit 3cbd1db) with only `com.unity.ugui` 1.0.0 changed (list in `Packages/com.base-setup.core/CLAUDE.md`); its prefabs import with all scripts resolved; all 63 Core EditMode tests pass in batch mode. Manifest trimmed to the packages the game uses, plus Input System 1.14.0 (the only active input handler) and the OpenUPM registry for VContainer and UniTask. Template samples removed; Android build target, portrait only, IL2CPP ARM64, product "Rollic Case Study", bundle id `com.casestudy.blockslide`, all set by an editor script. Skill `unity-editor` rewritten for the batch-mode workflow. Git: `main` with the first commit, work continues on `develop`; no remote.
- **2026-11-17, phase 1 decisions done.** Case.md checked against the PDF (small fixes). Reference game studied through five levels on the developer's Android device over `adb`; mechanics in [Reference](Reference.md). Open decisions settled with the developer and written to [Decisions](Decisions.md): follow the reference unless the case says otherwise; free drag with whole-cell rules; exit when touching a covering door of the block's color; hand-written motion; levels as ScriptableObjects with engine-free data; Boot + Home + Game scenes; lives, booster popup and out-of-time offer left out as the case allows.
- **Next:** finish phase 1, in this order:
  1. Copy the supplied assets (the case's Drive folder, already downloaded by the developer) into `Assets/Art/` with an editor script and review them; problems go to the README feedback section.
  2. Create the four game assemblies and the test assembly.
  3. Start phase 2 with the level format and the move rules, tests first.

## Phases
Each phase ends with compile, EditMode tests green, notes and index updated, a `--no-ff` merge into `develop`.

0. **Setup**: project, Core port, docs, skills.
1. **Scope and decisions** (with the developer): settle the open decisions below; download the supplied assets into `Assets/Art/` and review them (problems found go to the README feedback section); write the first entries in [Decisions](Decisions.md); create the four game assemblies and the test assembly.
2. **Data and logic** (`Game.Data`, `Game.Logic`): level format (board size, blocks with shape and color, doors with side, position, width and color, timer), board state, moves and collisions, exits through a matching door, win when the board is empty, fail when the timer reaches zero, coin wallet rules. EditMode tests first.
3. **Level editor** (`Game.Editor`): an editor window that draws the board; set width and height, paint blocks, place doors on the edges, set the timer; create, open, save, play from the editor; undo. Warn when a block fits no door of its color (it could never leave). Author the 5 levels with it. Usability check: make a level from scratch without reading code.
4. **Gameplay** (`Game`): board and block views, drag input, door views, slide and exit animations, HUD (timer, coins, level number), pause with resume, restart, home; fail popup (restart, home); win then next level; coins saved through Core.Save.
5. **Meta screens**: home (top UI with coins and lives placeholder, level button, settings button, placeholder tab bar, touch feedback), settings (sound, music, vibration toggles bound to Core.Audio and Core.Haptics, close). Check layout at 1080 x 1920 and 20:9.
6. **Delivery**: Android build (IL2CPP, ARM64), a short recording of the full flow, README written from `Decisions.md`, `WorkLog.md` and the known issues, final pass for dead code and style. Docs tidy-up, assuming reviewers read everything including the git history: the README is the entry point and links only the few docs a reviewer needs (decisions, conventions, reference, editor guide); the status in this plan moves to the README's known problems, work time and LLM use move from `WorkLog.md` to the README; then this plan and the work log are trimmed to reviewer-facing text or removed; no personal paths or device details anywhere; `CLAUDE.md` and `.claude/` stay as the record of how the LLM tools were used.
7. **Optional, only if 1-6 are complete**: level validation in the editor (solver warns before saving), then one optional mechanic.

## Open decisions
Settled decisions are in [Decisions](Decisions.md). Still open:

| Decision | Options | Leaning |
|---|---|---|
| Submission | Private repository shared with the reviewers, or an archive | Decide at delivery |

## Risks
- The editor is the second most weighted criterion; budget real time for its usability, not only for its function.
- The live `unity command` bridge does not work on 2022.3; every editor action is a batch-mode run or an editor script, and batch mode cannot open the project while the editor has it open.
- Supplied assets may not match the expected grid or colors; review them in phase 1.
