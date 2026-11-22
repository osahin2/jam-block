# rollic-case-study

A vertical slice of a block-sliding color puzzle for Android (portrait, 1080 x 1920): home screen, settings, five levels with timer, coins, pause, restart and fail popup, and a visual level editor for designers. The case's optional scope is in too: ice blocker, arrow block, functional boosters and the unsolvable-level warning.

<!-- Draft: GIF of the full flow, links to the APK and the recording. -->

## Open and run
- Unity **2022.3.62f2** with Android Build Support. Packages resolve on first open (VContainer, UniTask and UI Particle come from the OpenUPM registry listed in `Packages/manifest.json`).
- Open `Assets/Game/Runtime/Bootstrap/Scenes/Boot.unity` and press Play. Boot loads the app-wide services and opens Home; the level button starts the current level.
- Set the Game view to 1080 x 1920 (or any portrait size; 20:9 and tablets are supported).
- Builds: menu **Build > Android APK (Release)** or **Build > Android APK (Development)**; the build logs its size and its largest assets.
- Tests: **Window > General > Test Runner > EditMode > Run All**.

## Scope
<!-- Draft: table of required (each acceptance criterion), optional and left out, after the acceptance check on device. -->

## Level editor
<!-- Draft: open (Game > Level Editor, or double-click a level asset), make a level step by step, tools and shortcuts, problems and the solver, Play from the editor, scene preview. Screenshot. Written after the solver is merged. -->

## Architecture
<!-- Draft: assembly diagram, the path of one level (PlaySession -> LevelAttempt -> views), how a new mechanic or booster is added. -->

## Decisions
<!-- Draft: 10 to 12 decisions, each as decision, reason, what was given up. -->

### The five levels are designed in the editor, at the reference's scale
The case asks for "the first 5 levels" without saying whether they must copy the reference game's first five layouts. They are read as the game's opening levels: the reference's first five set their scale (4 x 5 to 5 x 6 boards, two to six blocks, five minutes, 40 coins each), and the layouts were designed from scratch in the level editor, which the case requires the five levels to come from.

## Performance
<!-- Draft: device, frame rate, APK size, what keeps the update loop allocation-free. -->

## Known problems and incomplete parts
<!-- Draft: collected during the acceptance check. -->

## LLM tools
<!-- Draft: Claude Code, which parts it did and which the developer did. -->

## Work time
<!-- Draft: one row per day. -->

## Third-party code
- [Quick Outline](https://github.com/chrisnolet/QuickOutline) by Chris Nolet, MIT license, in `Assets/Plugins/QuickOutline/` with its license file. The first commit that adds it holds the original files; the changes made for this project follow in their own commits and are listed at the top of `Outline.cs`.
- [UI Particle](https://github.com/mob-sakai/ParticleEffectForUGUI) by mob-sakai, MIT license, version 4.14.2 from OpenUPM (`com.coffee.ui-particle`): draws the win screen's confetti, a ParticleSystem, inside the UI canvas.
- [Lilita One](https://fonts.google.com/specimen/Lilita+One) by Juan Montoreano, SIL Open Font License 1.1, in `Assets/Plugins/LilitaOne/` with its license file and the TextMesh Pro font asset made from it: the font of every screen, as the supplied assets have none.

## Feedback on the supplied assets
- **The block models are too tall.** The pieces are about 1.6 units deep, while a block reads as a plate a quarter of a cell thick. Sunk into the ground they look right from above, but the held block's outline is drawn around the whole mesh, so it also outlined the part below the board and no longer matched the reference; scaled down to a plate's height instead, the studs on top turn flat. The project makes flat plates from the supplied pieces in Blender (`Tools/Blender/make_block_plates.py`).
- **The models do not share one orientation.** Block pieces and arrows face `-Z`, the ground tile `+Y`, the frame bars `-Y`, so each file needs its own rotation to lie on the board.
- **The door arrow sits inside the door.** `Door_Arrow` is placed 1.85 units below the door's top, inside the door body, so it never shows; it has to be moved onto the door's top.

## Documentation
`Docs/00-Index.md` maps the rest: case requirements, conventions, the Core package's systems and one note per game system.
