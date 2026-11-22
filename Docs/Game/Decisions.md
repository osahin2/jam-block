# Decisions

> One short entry per choice that shapes the architecture, the scope or more than one feature, with an alternative that was really weighed: what was chosen, why, and over what. How a single feature works belongs in its system note. The README's decisions section is written from it. Newest last.

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

## Folders: feature first, code and assets apart, fixed names
- Why: a feature lives under one folder per assembly, and fixed role and asset folder names make every file's place obvious.
- Instead of: an asset tree by type, which scatters a feature; feature folders above the assemblies, which hides the four-layer split.

## The Game flow is one method per step
- Why: each screen's buttons lead to the next method, so `GameEntry` reads like the flow chart.
- Instead of: a state machine with a class per state; screens awaited with UniTask.

## Game code explains itself, without comments
- Why: names and small named steps say what the code does; contracts and engine facts live in the system notes. A hook refuses new comments in `Assets/Game`. Core, a reusable package, keeps short comments on its public contracts.
- Instead of: comments that drift from the code.

## Mechanics plug in through small role interfaces
- Why: the board asks `IMoveRule` and `IBlockGuard` at fixed points, views show a mechanic through an `IBlockFeature`, and the editor draws it through its own drawing; the level attempt builds the board and its mechanics together, as they share state. A new mechanic is a class per layer and one line per list, so the board, views and editor name no mechanic. Ice and the arrow block were added this way.
- Instead of: a method per mechanic on the board and a field and call per mechanic in the views and the editor; one interface with every hook, mostly empty; a data-driven registry.

## Views catch up by pulling the logic's state
- Why: whoever changes the board calls `BoardPresenter.CatchUp`, and each view plays the difference from the logic's state; counts such as ice left are computed from the board, so nothing has to be told a block went.
- Instead of: events, or counters lowered by events; the logic returning results the views switch on.

## A booster is a logic effect and a presentation
- Why: what it does to the board is a tested command (`IBoardEffect`); `TargetedBooster` fixes the order every aimed booster follows, so each adds only its effect and shot.
- Instead of: a method per booster on the attempt; a booster subclass per animation.

## The solver searches every arrangement breadth-first on the game's own rules
- Why: a block leaving only frees cells and melts ice, so leaving as soon as it can is never wrong; the search branches only where no block can leave, and breadth-first gives the fewest moves. Proving a level unsolvable needs every arrangement anyway, so a heuristic would not help. It runs on the game's own `Board` and mechanics, so it cannot drift from the rules; a test agrees with a plain search on random levels.
- Instead of: A* or greedy search; a copy of the rules inside the solver.

## The level editor solves after every change and asks before saving an unsolvable level
- Why: the verdict helps most while editing. A quick check bounded by time keeps editing smooth; Check fully, bounded by arrangements, decides what it leaves open. The case asks for a warning, so an unsolvable level asks before it is saved but can still be kept.
- Instead of: only a Check button; a full search after every change, which would freeze on large boards; a background thread; refusing to save.

## Texture sizes follow their size on screen
- Why: every texture came in at Unity's default 2048. Each max size is now the smallest power of two that covers it on a 1080x2400 phone, from 2048 for full-screen backgrounds to 256 for small icons; default presets give new textures the same settings.
- Instead of: one limit for all, which blurs the backgrounds; an `AssetPostprocessor` that overrules changes made in the inspector.

## Levels are designed in the editor, at the reference's scale
- Why: the case asks for "the first 5 levels" without saying they must copy the reference game's layouts. They are read as the game's opening levels: the reference's first five set the scale (4 x 5 to 5 x 6 boards, two to six blocks, five minutes, 40 coins each), and the layouts are original, made in the level editor as the case requires.
- Instead of: copying the reference's first five layouts.
