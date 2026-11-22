# Decisions

> One short entry per choice a reviewer could ask about: what was chosen, why, and what it was chosen over. The README's decisions section is written from it. Newest last.

## Four game assemblies: Data, Logic, Runtime, Editor
- Why: the compiler checks the separation of data, logic and view; `Game.Logic` has no engine reference, so rules run in EditMode tests without a scene, and the editor and game share one level format.
- Instead of: one assembly with folders, where a rule could reach a transform unchecked.

## One entry class per feature
- Why: other features know one class and its data types, so a feature changes inside without touching callers and coupling shows in a few signatures.
- Instead of: features reaching into each other's services, or an event bus that hides who reacts.

## A base package (Core) reused from earlier work
- Why: save, scenes, UI, pooling, audio and haptics are not what the case evaluates; a tested package keeps the time for the game and the editor.
- Instead of: writing them again.

## Dependency injection with VContainer
- Why: constructors state what a class needs, each scene is composed in one place, tests build classes directly. VContainer is maintained, small, resolves without allocating and never scans the scene. Only the composition root references it, so a container change touches scopes and installers only.
- Instead of: a service locator or singletons, which hide dependencies; Zenject, unreleased since 2021 and much larger than needed.

## No event bus, with a rule for when one fits
- Why: a bus pays off for unrelated observers (analytics, quests); the case has none. Flow has one orchestrator, the coin balance one owner. The rule and the planned shape are in the conventions.
- Instead of: a bus from day one, which hides who reacts and leaves order to subscriptions.

## DOTween for one-shot motions
- Why: most Unity developers know it, so a motion is a few readable lines.
- Instead of: hand-written timers in every view.

## Levels as ScriptableObject assets holding engine-free data
- Why: assets give references, the inspector and Unity's undo; the level types are plain C# with public fields, so logic and tests never load the engine.
- Instead of: JSON files with their own undo and format check; `Vector2Int` and `Color` in the data.

## Boot, Home and Game scenes
- Why: Core switches additive scenes over a boot scene, so leaving a level rebuilds cleanly; next level and restart stay inside Game, as in the reference.
- Instead of: one scene with screens, where level leftovers are easier to miss.

## A block is a set of cells relative to an anchor
- Why: one type covers every shape; a step changes only the anchor; the view builds from the same cells.
- Instead of: width, height and a shape kind, with a case per shape everywhere.

## Our own Android haptics backend in Core
- Why: Core had only an iOS backend, so the vibration switch changed nothing on Android. The platform API is small (`Vibrator`, `VibrationEffect`) and its pitfalls are known (the permission, API levels, JNI exceptions); Core's `IHapticBackend` keeps a package swappable later. API 24-25 stay silent to keep one code path.
- Instead of: a third-party haptics package; the free ones are small hobby projects with uncertain upkeep, and the paid ones cannot be shipped in the repository.

## Levels are validated when made, not when played
- Why: levels come only from the editor, which shows every problem with its block, door or cell; a test checks every level asset.
- Instead of: a second check in the board, or exceptions that report only the first problem.

## One level attempt is the session's only rule object
- Why: the board, drag, timer and the rules between them are tested without a scene; a restart is a new attempt.
- Instead of: the session calling each part, which puts win, fail and pause rules in view code.

## Board input through the EventSystem on one board-sized collider
- Why: UI takes a press first, so HUD and popups block the board for free; which block is touched stays a logic question.
- Instead of: polling the pointer, which ignores UI; a collider per block, which moves the lookup into physics.

## The level editor is UI Toolkit only
- Why: bound fields follow undo by themselves, states live in USS, layout in UXML, and `ListView` stays quick with thousands of levels.
- Instead of: IMGUI, where hover, selection and undo sync are hand-written; or both models in one window.

## Levels are previewed in the Game scene, built by the game, never saved
- Why: the designer sees the level as the game frames it without entering play mode, through the same `LevelStage`; the scene file never changes.
- Instead of: saving levels into the scene; an in-window preview that lacks the game camera.

## The held block's outline is a fork of Quick Outline
- Why: it outlines the block's outer shape only, at an even screen width, in URP as published. Forked (MIT, original and changes in separate commits) for pooled pieces and allocation-free grabs. Blocks are flat plates made from the supplied pieces in Blender, as the deep supplied pieces spilled the outline below the ground.
- Instead of: an inverted hull (lines on seams), a ribbon mesh (fragile geometry), a full-screen outline pass every frame on mobile.

## The coin wallet saves itself through a port
- Why: every change must reach the disk; the interface sits in the logic, the Save implementation in `Game`, tests use memory.
- Instead of: a wrapper that saves after each call, which a direct caller could bypass.

## A level ends in one place and calls back won or failed
- Why: the session sees a touch's win and the timer's fail the same way; two callbacks give each reaction its own method in the flow.
- Instead of: one `onEnded(outcome)` that pushes a branch into every caller.

## Folders: feature first, code and assets apart, fixed names
- Why: a feature lives under one folder per assembly, and fixed role and asset folder names make every file's place obvious.
- Instead of: an asset tree by type, which scatters a feature; feature folders above the assemblies, which hides the four-layer split.

## Board pieces come from a source the caller gives
- Why: the game pools pieces and the editor preview makes unsaved copies, with the same builder. Pools fill on the first level: prewarming 150 small objects would only move the work.
- Instead of: the builder owning the board's lifetime; prewarmed pools sized in settings.

## The Game flow is one method per step
- Why: each screen's buttons lead to the next method, so `GameEntry` reads like the flow chart.
- Instead of: a state machine with a class per state; screens awaited with UniTask.

## The played run is a strategy chosen once
- Why: the player's run and a designer's test differ in level, number and what a win does to progress; `ILevelRun` holds all three, so the flow never branches on it.
- Instead of: methods per kind with the choice in the flow.

## Game code explains itself, without comments
- Why: names and small named steps say what the code does; contracts and engine facts live in the system notes. A hook refuses new comments in `Assets/Game`. Core, a reusable package, keeps short comments on its public contracts.
- Instead of: comments that drift from the code.

## Mechanics plug into the board through role interfaces
- Why: the board asks `IMoveRule` and `IBlockGuard` at fixed points; a new mechanic is a class, a level field, one line in `LevelMechanics.For` and maybe one validator check.
- Instead of: a method per mechanic on the board; one interface with every hook, mostly empty; a data-driven registry.

### The level attempt makes the board and its mechanics together
- Why: they share state, so they come in pairs; internal constructors let the compiler hold the boundary and keep views away from guards.
- Instead of: the session building them, which let a second board share mechanics.

## Ice counts the blocks gone; views catch up by pulling
- Why: ice left is computed from the board's one removal count, so nothing has to be told a block went. Whoever changes the board calls `CatchUp`, and each view plays the difference.
- Instead of: counters lowered by events; the logic returning results the views switch on.

## A booster is a logic effect and a presentation
- Why: what it does to the board is a tested command (`IBoardEffect`); `TargetedBooster` fixes the order every aimed booster follows, so each adds only its effect and shot.
- Instead of: a method per booster on the attempt; a booster subclass per animation.

## The board's touch path is input, presenter, projection
- Why: one class had six jobs and changed with every booster and mechanic; now each changes for one reason, and boosters get only what they use.
- Instead of: one board interaction, or a facade that only forwards.

### Board touches go through modes: drag, pick, busy
- Why: aiming picks a cell instead of grabbing, and during a shot the logic is ahead of the views; one mode at a time replaces the listeners.
- Instead of: flags in every handler.

## A mechanic shows itself on a block through a block feature
- Why: the view, builder and presenter no longer name any mechanic; a new one adds a feature, a prefab part and one line in `BlockFeatures.Of`.
- Instead of: a field and a call per mechanic in three classes.

### The editor draws mechanics through one drawing each, in two layers
- Why: each mechanic's drawing is its own class, and the layers keep tint, arrow and badge readable together.
- Instead of: an if per mechanic in the board view.

## The solver searches every arrangement breadth-first, leaving blocks at once, with no heuristic
- Why: a block leaving only frees cells and melts ice, so leaving as soon as it can is never wrong; the search branches only where no block can leave, and breadth-first gives the fewest moves. Proving a level unsolvable needs every arrangement anyway, so a heuristic would not help. It runs on the game's own `Board` and mechanics, so it cannot drift from the rules; a test agrees with a plain search on random levels.
- Instead of: A* or greedy search; a copy of the rules inside the solver.

## The solver counts alike blocks once and does not ask the block just moved
- Why: on open boards the cost is the number of arrangements. Swapping alike blocks gives the same arrangement, and the block just moved cannot newly reach a door. Both are exact; the second rests on every step being reversible, written as a rule in the solver's note.
- Instead of: re-asking only blocks whose reach touched freed cells, which measured no gain for a hundred lines and a fragile assumption.

## The level editor solves after every change and asks before saving an unsolvable level
- Why: the verdict helps most while editing. A quick check bounded by time keeps editing smooth; Check fully, bounded by arrangements, decides what it leaves open. The case asks for a warning, so an unsolvable level asks before it is saved but can still be kept.
- Instead of: only a Check button; a full search after every change, which would freeze on large boards; a background thread; refusing to save.

## The freeze bar reads the timer's frozen seconds
- Why: the freeze is the timer's state, so the bar under the time box reads `FrozenSeconds` each frame as the time label reads the remaining seconds; pause stops the count in the logic, not through `timeScale`, so a bar with its own clock would keep draining under the pause popup. The logic gives seconds and the bar takes the first value it sees as full.
- Instead of: a 10-second tween started by the booster; a global tick manager that stops everything, when a freeze stops only the timer; a progress ratio in the logic.

## Texture sizes follow their size on screen; presets set new textures
- Why: every texture was imported at Unity's default 2048, so the booster icons and the large backgrounds took far more memory than their size on screen needs. Each texture's max size is now the smallest power of two that covers its largest size on screen on a 1080x2400 phone (the canvas scales by height, 1.25 times its 1920 reference): 2048 for the full-screen backgrounds, 1024 for the popup frames and the tab background, 512 for the booster icons and the rest of the UI, 256 for small icons; the ice texture keeps the 512 it was given with the ice. Compression stays Automatic, which is ASTC on Android. Two default presets in the Preset Manager, `UISprite` for `Assets/Art/UI` and `SurfaceTexture` for `Assets/Art/Textures`, give a new texture the same settings when it is first imported; they leave out the sprite border, pivot and sprite sheet, so no texture takes another's nine-slice borders.
- Instead of: one 512 limit for all, which would blur the full-screen backgrounds; an `AssetPostprocessor` that rewrites the settings on every import and overrules changes made in the inspector; per-platform overrides, which the Android default format already covers.
