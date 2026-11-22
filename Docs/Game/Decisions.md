# Decisions

> Summary: One short entry per choice a reviewer could ask about. The README's "architecture decisions" section is written from this file, so each entry says what was chosen and why in a few lines. Newest last.

Format:
```
## <Decision as a statement>
- Why: <the reason, one or two lines>
- Instead of: <the alternative and why it lost>
```

## Four game assemblies: Data, Logic, Runtime, Editor
- Why: the separation of data, logic and view is checked by the compiler, not only by convention. `Game.Logic` has no engine reference, so rules cannot touch a view and run in EditMode tests without a scene. The editor and the game share one data assembly, so there is only one level format.
- Instead of: one `Game` assembly with folders, where a rule could reach a transform and nothing would stop it.

## One entry class per feature
- Why: other features know one class and its data types (`PlaySession.Start(level, onEnded)`), so a feature can change inside without touching its callers, and the coupling between features is visible in a few signatures.
- Instead of: features reaching into each other's services, or an event bus where it is not visible who reacts.

## A base package (Core) reused from earlier work
- Why: save, scene switching, UI screens, pooling, audio and haptics are not what the case evaluates; reusing a tested, game-agnostic package keeps the time for the game and the editor.
- Instead of: writing these systems again inside the case.

## VContainer for dependency injection, constructor injection only
- Why: dependencies are visible in constructors, composition happens in one place per scene, and tests construct classes directly.
- Instead of: singletons or `FindObjectOfType`, which hide dependencies and make tests need a scene.

## Only the packages the game uses
- Why: the Universal 3D template brings Ads, Analytics, Purchasing, Visual Scripting, Timeline, Tilemap, Navigation and XR helpers; none is in the case scope. Removing them shortens compile and import times and the build, and leaves fewer things a reviewer has to wonder about.
- Instead of: keeping the template's package list.

## No event bus in the case, with a rule for when one fits
- Why: a bus pays off when unrelated observers (analytics, quests, tutorials) watch facts the publisher must not know about. The case has no such observer: flow is owned by one orchestrator, the coin balance has one owner, sound and haptics are called from presentation. The rule and the planned shape (a small typed Core bus, struct messages, synchronous, per scope) are in the Communication convention, so it can be added when a real observer appears.
- Instead of: a bus from day one, which hides who reacts and leaves order to subscription order; or forbidding it outright, which forces cross-cutting observers into every publisher's constructor.

## Follow the reference game unless the case says otherwise
- Why: the reviewers know the reference game; a difference reads as a misunderstanding unless the case asked for it or allows it. Each difference below names the case line behind it. Observations: [Reference](Reference.md).
- Instead of: simplifying mechanics where it would save time, which changes the game being evaluated.

## Free drag, with the rules in whole cells
- Why: the reference moves a block continuously under the finger in both axes, stopped by walls and other blocks. The logic keeps whole cells (`Step(block, direction)`, a check whether a step is free) so rules and tests stay discrete; the view adds a sub-cell offset of up to half a cell toward a free neighbour cell and none toward a blocked one, commits a step when the finger passes half a cell, and settles the block on its cell on release. The move result is the block's new cell and, when it left, the door it left through.
- Instead of: swipe-to-slide until blocked, which is a different game; or continuous positions in the logic, which makes collisions and tests float-based.

## A block leaves when it touches a door of its color that covers it
- Why: observed rule. A block leaves as soon as it touches a side of the board where a door of its own color covers its whole extent along that side, from any drag direction; a door of another color is wall. Checked in one place in the logic after each step. The block slides out straight through the door, so another block between the door and any of its cells holds it on the board (found by the developer on an L shape while authoring the levels; to be confirmed in the reference); it leaves on the next touch once the way is free.
- Instead of: requiring a drag toward the door, which the reference does not.

## DOTween for one-shot motions, a hand-written follow for the drag
- Why: the drag follows a target that changes every frame, which a tween cannot do without restarting each frame, so it stays a hand-written exponential follow. Everything else is a short, often sequenced motion: the snap on release, the snap in front of a door with a hold and the slide out, and later the break-apart, the door, coins flying to the counter, popups and touch feedback on every button. DOTween expresses each as a few lines (`Sequence`, `AppendInterval`, eases), is widely known, and each tween is linked to its object (`SetLink`) so a rebuilt level leaves none behind. Its allocations happen per motion, not per frame. Version 1.3.045, in `Assets/Plugins/Demigiant/`.
- Instead of: hand-written timers and state fields in every view for each motion, which grew with every motion the reference shows; or a tween for the drag.

## Levels as ScriptableObject assets holding engine-free data
- Why: one asset per level gives asset references, the inspector and Unity's undo (the level editor edits an in-memory ScriptableObject copy with it and writes the copy back only when it has no errors, so a dirty asset with errors is never saved by Ctrl+S); the level types in `Game.Data` are plain C# with an own `Cell` and a color id, so logic and tests never touch the engine, and the view maps color ids to the supplied art.
- Instead of: JSON files, which need own undo, file handling and a second format check; or `Vector2Int` and `Color` in the data, which ties the data assembly to the engine.

## Boot, Home and Game scenes
- Why: Core.Scenes switches additive scenes on top of a boot scene; leaving a level rebuilds cleanly. Continue and restart rebuild the level inside the Game scene without a scene switch, as the reference goes straight to the next level.
- Instead of: one content scene with screens, where leftovers from a level are easier to miss.

## Timer starts on the first touch and stops under popups
- Why: observed in the reference; one flag in the logic. Every level of the reference starts at 05:00; the value is set per level in the editor.
- Instead of: starting when the level opens.

## Coin reward as one global setting
- Why: the reference paid 40 coins on each of the first levels; one value in a settings asset, paid when the player continues from the win screen.
- Instead of: a reward per level in the level data, which the reference does not show for these levels.

## Lives shown as a placeholder, no life-loss confirmations
- Why: the case says the life system can be a placeholder. The home screen shows a lives count; restart and home act at once. The reference's "you will lose 1 life" confirmations only make sense when lives are spent, and spending them needs refill, a zero-lives state and a play block, which is the system the case left out.
- Instead of: showing confirmations that warn about a loss that never happens.

## Level button starts the level directly
- Why: the case's acceptance criterion is "the level button starts the current level"; the reference's pre-level popup exists to pick boosters, which the case leaves without function.
- Instead of: the pre-level booster popup.

## Fail goes straight to the level-failed popup
- Why: the case asks for a fail popup with restart and home. The reference's "out of time, 20 more seconds for coins" step and its purchase offer are booster and monetization features the case does not ask for.
- Instead of: the extra out-of-time offer before the fail popup.

## Pause opens the settings popup with a home button
- Why: as in the reference; the same three toggles as on the home screen plus home, and the timer stops while it is open. Restart stays a HUD button.
- Instead of: a separate pause menu.

## Vibration toggle shows and saves its state; Android backend only if time allows
- Why: the case requires the toggle state only. Core.Haptics has no Android backend; adding one (Android `Vibrator` through JNI) is a phase 5 extra.
- Instead of: building the backend before the required scope is done.

## Deliver both an APK and a recording
- Why: the recording is cheap once the build runs and shows the flow even if a reviewer cannot install the APK. How to submit (private repository or archive) is decided at delivery.
- Instead of: only one of the two.

## Level data as plain classes with public fields, the asset class in Game
- Why: Unity serializes public fields of a `[Serializable]` type without an engine reference, so `Game.Data` stays engine-free (`noEngineReferences: true`) and the logic and its tests never load the engine. `LevelAsset`, the ScriptableObject, is in `Game` and holds one `LevelData`. Nothing changes the data at runtime: `Board` copies what it needs into its own state.
- Instead of: private `[SerializeField]` fields with getters, which need `UnityEngine` in `Game.Data`; or a second, read-only copy of every type.

## A block is a set of cells relative to an anchor
- Why: one type covers rectangles, L shapes and any later shape; a step changes only the anchor; the view builds the block from the same cells. The exit check uses the block's extent along the door's side, which works for every shape.
- Instead of: width and height plus a shape kind, which needs a case per shape in the rules, the editor and the view.

## Levels are validated when they are made, not when they are played
- Why: levels come only from the level editor. `LevelValidator` returns every problem with the block, door or cell it is about, split into errors (cannot be played, not saved) and warnings (probably a mistake, shown); the editor uses it before saving and a test checks every level asset in the project. The board takes a level as given.
- Instead of: checks in the `Board` constructor as well, a second guard for data that cannot reach the game without passing the editor and the test; or exceptions as the validation, which give the editor only the first problem as text.

## A blocked step is a result, not an exception
- Why: the finger can run ahead of a stopped block, so trying a step into a wall or another block is an ordinary event. `Step` decides and returns `Moved` false without changing anything; `CanStep` stays for the view's sub-cell offset.
- Instead of: throwing when the step is not free, which makes an ordinary event an error and makes every caller check first.

## Blocks are built from the supplied quarter pieces at load time
- Why: the models are made as a modular kit (center, edge, outer and inner corner per quarter cell), so one builder covers every shape a designer paints, and a new shape needs no art or prefab. It runs once per level, not per frame.
- Instead of: one prefab per block shape, which has to be made by hand for every shape and goes stale when the editor allows a new one.

## The drag rule is logic, not view code
- Why: when a step is taken, how far the block may lean toward a neighbour and how a fast or diagonal drag is resolved decide the game's feel and correctness; in `Game.Logic` (`BlockDrag`) they are tested without a scene. The view only converts the pointer to board cells and shows the returned pose.
- Instead of: computing steps in the input or view code, where it can only be checked by hand in play mode.

## Blocks follow their target with frame-rate independent exponential smoothing
- Why: the block should feel attached to the finger: `1 - e^(-ln2 · dt / halfLife)` of the remaining distance per frame has no inertia, so starts and reversals answer at once, needs no velocity state to reset between drags, behaves the same at any frame rate and cannot pass the target on a long frame. The half-life is tuned in the motion settings. Placing a block on release and in front of its door is a short eased snap (DOTween), as the reference snaps it there.
- Instead of: `Vector3.SmoothDamp`, a critically damped spring whose velocity state eases every start and carries through reversals; or `Lerp(a, b, k · dt)`, which changes with the frame rate and overshoots when `k · dt > 1`.

## One level attempt is the play session's only rule object
- Why: the session needs one answer per touch and per frame: is this touch taken, where is the block, is the level over. `LevelAttempt` owns the board, the drag and the timer and the rules between them (the first touch starts the timer, a popup stops both, the outcome is set once), so they are tested without a scene and the session only shows what it returns. A restart is a new attempt, so nothing survives from the last try.
- Instead of: the session calling `Board`, `BlockDrag` and a timer itself, which puts the win, fail and pause rules in view code.

## The timer starts on any touch inside the board
- Why: the reference starts it on the first touch on the board; a touch on an empty cell counts too, so the player cannot study the level with the clock stopped by touching around the blocks. A touch outside the board (HUD, margins) does not start it.
- Instead of: starting only when a block is grabbed.

## A held block can always be released
- Why: when the time runs out or a popup opens under the finger, the board takes no more moves, but the view still needs the held block back on its cell. `Release` works in every state and returns the block.
- Instead of: ending the drag inside the attempt, which leaves the view with a block between cells and no call that tells it so.

## The coin wallet only earns
- Why: the case asks for a coin inventory whose count stays correct, and nothing in its required scope spends coins: the reference spends them on boosters, the shop, lives and the out-of-time offer, all left out above. `CoinWallet` keeps the balance non-negative and rejects negative amounts as programmer errors. The first spend planned is buying boosters, optional after the required scope (phase 7 in the plan), which adds `CoinWallet.TrySpend`.
- Instead of: a spend rule nothing calls, which is dead code; or building the out-of-time offer before the editor, which the evaluation weighs more.

## Board input through the EventSystem on one board-sized collider
- Why: the EventSystem sends a press to the UI first, so HUD buttons and popups block the board without extra checks, and it is the input path the UI uses anyway. One collider covers the board, so touches outside it never arrive, and which block is under the finger stays a logic question (`Board.BlockAt`), not a physics one. Drag positions are projected onto the touch plane, which also works when the finger leaves the collider. The visual spike polled `Pointer.current` to move fast; it was replaced in phase 4 step 4. Only the first pointer and the primary button drag (a second finger or a right click moves nothing), and the EventSystem's drag threshold is turned off on the board, so a block follows from the first movement as before. The projection runs in the board root's local space, the space the board is built and framed in, so the board root can be moved without breaking touches. The camera's `PhysicsRaycaster` raycasts into a fixed array (`maxRayIntersections` 4); at its default of 0 it calls `RaycastAll` every frame and allocates.
- Instead of: polling the pointer each frame, which ignores UI; or a collider and handler per block, which moves the block lookup into physics and needs colliders rebuilt as blocks move.

## The play session runs the level; the board's touch path runs the touches
- Why: they change for different reasons. `PlaySession` owns the level's lifetime and time (build, tick, and in phase 4 the HUD, win, fail, restart and next level); the touch path (pointer to attempt to block views, now `BoardInput`, `BoardPresenter` and `BoardProjection`) changes with input and motion. Splitting before phase 4 keeps the flow from growing inside the touch code.
- Instead of: one session class that both connects the views and runs the flow.

## The level editor window is UI Toolkit only
- Why: the editor is evaluated for usability, and UI Toolkit gives it for little code: fields bound to the working copy follow undo by themselves, a problem list, toolbar toggles, hover and selected states in USS, layout in UXML apart from the code. Everything the window needs exists in 2022.3; the Unity 6 APIs that do not are listed with their replacements in the `add-game-feature` skill. No IMGUI inside it: if UI Toolkit blocks something, the whole window moves to IMGUI.
- Instead of: IMGUI, stable and quick for painting a grid, but hover, selection states and the sync between fields and undo are hand-written; or a mix, a board in an `IMGUIContainer` inside a UI Toolkit window, which leaves two UI models in one window.

## Levels are previewed in the Game scene, built by the game and never saved
- Why: the designer sees a level in the Scene and Game views as the game will build it, framing included, without entering play mode. The preview shows the level through the game's own `LevelStage` (layout, build, camera framing), so there is no second way of drawing or framing a level, and its objects are `DontSave` and not editable, so the level stays in its asset only and the scene file never changes. It shows the level open in the level editor, live, or else the level asset last selected in the Project window; following the selection while no window is open needs an `[InitializeOnLoad]` editor hook, a static class called only by the window (the play starter is the other one, for the same reason: it acts after a domain reload).
- Instead of: saving the level into the scene, which gives a level two sources and a scene diff per level edit; or a preview inside the editor window (`PreviewRenderUtility`), which does not show the game camera's framing.

## A block at its door leaves on a touch
- Why: in the reference a block can start touching its door and stays there until the player taps it, and a tap is enough to send out a block whose way to the door was blocked and is now free. The level attempt asks the board's one exit rule (`Board.TryLeave`) on every touch as well as after every step, so a designer may place a block at its door, and a freed block leaves without being dragged away and back. Replaces the earlier `BlockStartsAtItsDoor` error, which forbade blocks at their door because only a step checked for an exit.
- Instead of: checking every block for an exit after any move, which sends out blocks the player did not touch; or keeping the error, which forbids a layout the reference uses.

## Level editor undo records whole copies of the level
- Why: Unity's difference-based `Undo.RecordObject` restored the draft wrongly after a cell was removed from the middle of a block's shape list (a test with real undo brought a block back with one cell twice and one missing). A level is a few hundred bytes, so a whole copy per stroke costs nothing; a stroke is recorded once, on release and only if it changed the level, so empty clicks add no undo step.
- Instead of: `RecordObject` before every press and drag, which is Unity's usual way but gives the broken restores; or an own undo stack, which leaves Unity's Edit menu and shortcuts.

## The solver waits for the optional scope
- Why: the case lists "warn before saving an unsolvable level" as optional scope, to be built only after the required scope, and scope control is evaluated. The five levels are small enough to check by playing them from the editor. The solver comes first in phase 7, where the level generator needs its metrics anyway.
- Instead of: building it inside the level editor phase, before gameplay and the meta screens exist.

## A new level becomes an asset on its first save
- Why: New opens an empty board in the editor's working copy; Save, without errors, creates the next `Level_NN` asset and appends it to the level catalog. A level with errors never becomes an asset, the catalog never lists a level the game cannot play, and pressing New and changing one's mind leaves nothing behind.
- Instead of: creating the asset and the catalog entry on New, which leaves an empty, invalid level in the game until the designer finishes or deletes it by hand.

## Play from the editor starts from the first build scene and hands over the level asset
- Why: the game then starts the way a build does, through the same scenes and scopes; the Boot scene sees the request and opens the Game scene instead of Home. `EditorSceneManager.playModeStartScene`, kept on the Boot scene by Core.Scenes, starts play mode there without closing the designer's open scenes, which come back when play mode ends. The request is the level asset's GUID, not a copy of its data: the editor saves before playing, so the asset is what the window shows, and the game knows which catalog level it plays, which win and next level need.
- Instead of: opening `Game.unity` in place of the open scenes, which asks to save them and leaves the designer elsewhere after play; or a copy of the level data, which loses the level's place in the catalog.

## A designer mostly changes an existing level
- Why: a designer is fastest making small changes to a level that already works: a copy of a level, a few blocks moved or recolored, the time adjusted. Duplicate in the level editor starts a new level from the open one, saved as the catalog's next level, so the original is never at risk. The level generator in phase 7 extends the same idea: a generated, solvable level goes into the working copy for the designer to adjust. Painting from an empty board stays for what has no level to start from, such as the first levels of a new mechanic.
- Instead of: only an empty board for every new level, which repeats the same groundwork each time.

## The level editor manages the levels in a searchable list
- Why: the catalog's order is the play order, and a designer keeps opening, reordering, duplicating and deleting levels; doing all of it in one list in the editor saves going to the catalog's inspector and the Project window. A `ListView` draws only the rows on screen and the search finds a level by its play number or name, so the list stays usable with thousands of levels. A deleted level's file goes to the trash, not away for good.
- Instead of: a dropdown menu of every level, unusable past a few dozen; or Unity's `AdvancedDropdown`, which is searchable but IMGUI, against the window's UI Toolkit only rule.

## The held block's outline is a fork of Quick Outline
- Why: the outline has to follow the block's outer shape only, an L's concave corner included, with nothing on the seams between its quarter pieces or around the studs, at an even width on screen. Quick Outline (MIT, by Chris Nolet) does this with two small passes over the block's own pieces: a mask pass marks the stencil wherever any piece is drawn, then a fill pass draws the pieces again pushed out along smoothed normals only where nothing is marked, so only the rim around the whole silhouette shows. The push grows with depth, so the width stays the same on screen. Its passes have no light mode, which URP draws as unlit passes, so it works in URP 14 as published. It is vendored in `Assets/Plugins/QuickOutline/` with its own assembly: the original files in one commit, our changes in the next, so the history shows what was taken and what was changed.
- What we changed, for pooled pieces and allocation-free grabs: the block view gives the renderers once it is built, instead of a search in `Awake`; one shared mask and one shared fill material, made from `BlockOutlineSettings` (color, width) and owned by the Game scope, instead of material instances per component loaded from `Resources`; turning it on and off swaps each renderer's materials between two arrays made at build, instead of building lists with LINQ on every enable; the smoothed normals are written into UV3 once at import (`BlockPlatesImport`), instead of at run time with LINQ and a static mesh registry. The outline outlines a piece's whole mesh, so the blocks are built from flat plates (see the block plates decision): with the supplied pieces sunk into the ground, the hidden part spilled the outline below the block. The modes we do not use, the bake fields and `OnValidate` are gone. A deactivated outline forgets its renderers, since they return to their pools and may serve another block. Builds keep every mesh channel (Optimize Mesh Data is off), so UV3 survives though no material asset reads it.
- Instead of: an own inverted-hull shader, which draws lines on the seams between pieces and around the studs; a ribbon mesh built from the cells, which matched the block only through measured corner radii at one height and needed geometry code of its own; a screen-space outline as a renderer feature, a full-screen pass every frame on mobile for one held block; Quick Outline as published, which allocates on every enable and does not follow pooled pieces.

## The coin wallet saves itself through a port it is given
- Why: the case's coin criterion is that the count stays right after leaving a level, so every change must reach the disk. `CoinWallet` reads its balance from an `ICoinStore` once and writes every change to it; the interface is in `Game.Logic`, the Core.Save implementation in `Game`. The rest of the game knows only the wallet, the logic stays engine-free and its tests use an in-memory store.
- Instead of: a `Game` class wrapping the wallet that saves after each call, which mostly forwards and lets a caller that uses the wallet directly forget to save.

## A win moves the player's progress only in the player's own run
- Why: progress is Core's completed-level count. A run started from the level editor is a designer's test, so it never moves the player's progress, also when the tested level is the player's current one; after its win the game continues with the catalog's next level, so a designer can play on through the order, or with the player's level when the tested level is not in the catalog yet. `GameEntry` chooses the run once, when the Game scene starts: an `ILevelRun` strategy (`PlayerRun` or `TestRun`, made by `LevelRuns`) that gives the level, its number and what `Advance` does after a win, so no win, HUD or popup asks where the run came from.
- Instead of: completing whatever level was won, which lets a test of level 4 skip levels 1 to 3; completing a won level when it equals the player's current one, which counts a designer's test of that level; or the catalog's first level after an uncatalogued level, which loses the player's place.

## Content scenes switch through one SceneFlow in the Boot scope
- Why: Core's switch needs a token that outlives the scene being left; `SceneFlow` supplies it once, names the scenes in one place, and gives Home and Game a plain `ToHome()` and `ToGame()` with nothing to await.
- Instead of: every caller holding scene names and a token, where a token tied to the calling scene would cancel the switch halfway.

## A level ends in one place in the play session, which calls back won or failed
- Why: a win is set by the touch that takes the last block out, a fail by the tick that reaches zero; `LevelAttempt.Tick` returns the outcome as it stands, so the session's tick sees both the same way and ends the level the first frame it is no longer playing. The session is the play feature's entry, so it reports the end to its caller through the callbacks given to `Start` and does not know what comes next; `GameEntry`, the Game scene's flow, decides (next level, same level, later the win screen and the fail popup). Two callbacks, `onWon` and `onFailed`, give each reaction its own method in the flow instead of a branch on the outcome there, and the session forgets the attempt before calling back, so a callback can start the next level at once.
- Instead of: one `onEnded(outcome)` callback, which moves the branch on the outcome into every caller; `Tick` returning whether the outcome changed, which misses a win set by a touch; the interaction reacting to the win where the touch set it, which puts a second end path next to the timer's; an event bus, which the conventions keep out of flow.

## Folders: feature first, code and assets apart, fixed folder names
- Why: a feature's files stay under one folder in each assembly, so adding or removing a feature touches one place, at any number of features. Inside a Runtime or Editor feature, code sits in `Scripts/<Role>/` and assets in folders named by their Unity type (`Installers/`, `Settings/`, `Prefabs/`, ...); both name lists are fixed in the conventions and nothing sits loose at a feature root, so where a file goes is never a choice and a reviewer finds the same shape in every feature. Each scene's scope and entry flow live in `Bootstrap/`, so all three scenes are put together the same way.
- Instead of: an asset tree by type beside the code (`Content/Settings/<Feature>/`, `Content/Installers/<Feature>/`), which repeats the feature tree under every asset type and scatters one feature over many folders as the game grows; or code and assets of a role in one folder, as before, which mixes scripts with content and left files loose at feature roots. Feature first above the assemblies too (`Coins/Data/`, `Coins/Logic/`, `Coins/Runtime/`, each joined to its assembly by an `.asmref`) was weighed and left: a folder's assembly would come from the nearest `.asmref` instead of its path, so a missing one silently moves logic into the runtime assembly, and the four-layer split, which the case weighs first, would no longer show at the top. The layers scale without it: when one layer grows too large to compile or own as a whole, its feature folders become assemblies of their own (`Logic/Coins/` as `Game.Logic.Coins`), each still under its layer, so the dependency direction stays visible in the folders.

## Board pieces come from a source the caller gives; the game's are pooled lazily in the Game scope
- Why: the case asks that nothing of an attempt stays after a restart, while the level editor's scene preview must build unsaved objects in edit mode, where pools would leave hidden objects in the open scene. `BoardBuilder.Build(level, layout, parent, pieces)` takes every piece from an `IBoardPieceSource` (`Take(prefab, parent)`) and keeps nothing, so the board belongs to its caller: the play session takes from `PooledBoardPieces` and gives the whole board back with `ReleaseAll` before the next level; the preview takes plain `DontSave` copies from `PreviewBoardPieces` and destroys its root. The source is a factory: the builder chooses which prefab, the source how an instance is made. It is named `Take`, not `Create`, because a pooled instance is reused and must be given back. `Take` is generic over the piece's component (`BoardPiece`, `BlockView`, `DoorView`), so a new kind of view changes neither the interface nor the sources; the pooled source gives instances back newest first, so a block's pieces leave it before the block goes back to its pool. The pools are made empty, one per prefab on first use: a level is about 150 small objects built once behind a transition, so prewarming only moves the first level's instantiation into the scene's load and needs sizes kept in step with the levels; lazily, the first level fills the pools and every restart and next level in the scene reuses them. They sit in the Game scope, not Boot, because their pieces are parented in the Game scene and die with it.
- Instead of: the builder destroying the board it built last (as before), which gave it the board's lifetime while the preview destroyed the same object; release on the interface, which the preview could not honor (it destroys, it does not give back); pools prewarmed from a settings asset sized for the largest level (built first, then dropped), which cost a settings asset, a sizing test and more objects than one level needs for no gain at this size; no pool, which is as much code (instances must still be tracked and destroyed) and instantiates on every restart; an abstract factory with a method per piece, which only pays off when whole families of pieces vary in code, while here a family is a `BoardArtSettings` asset, and which would repeat the piece-to-prefab mapping in both sources and change the interface for every new piece.

## The camera keeps its angle and moves to frame each level
- Why: the reference shows every level from the same tilt, with the board as large as the screen allows. `CameraFraming` keeps the scene camera's rotation and field of view and moves it until the board's box (frame included, ground to block tops) fills the free window of the screen on one axis and sits in its middle. It measures and corrects: it projects the box's corners with `WorldToViewportPoint`, steps the camera back or forward by the ratio of the board's size to the window's (a size on screen goes with 1 / depth) and slides it across by the gap between the board's center on screen and the window's. Perspective shows the near half of the board larger than the far half, so one correction overshoots a little; each round cuts the error about tenfold, and five rounds leave it within a pixel (measured at 1080 x 1920 and 20:9 for the smallest, largest and a deep board). It starts from where the board's bounding sphere fits the screen, so every corner is in front of the camera wherever the camera was, and a level is always framed the same way. Layout, build and framing are one `LevelStage.Show`, called by the session and the scene preview, so the preview frames like the game; `LevelStage` makes its builder and framing from the settings and the block features it is given, so the editor knows only `LevelStage` and a change to the builder's or the framing's needs touches one constructor instead of the installer and the editor.
- Instead of: an exact one-pass solution worked before the perspective division (each corner bounds the camera's shift by a straight line in its distance), which gives the same result but is hard to follow from the code; a direct formula from the board's size as the tilted camera sees it, which has to assume the whole board at its nearest depth to never spill out and so shows tall boards about 7 % smaller at 1080 x 1920; a fixed camera for the largest board, which shows small boards small; changing the field of view per level, which changes the perspective between levels; or an orthographic camera, which loses the reference's depth on the blocks.

## The board input sizes the board's collider, not the stage or the builder
- Why: the collider is input, and input belongs to the play session's touch path. `BoardInput.Begin` already gets each level's layout, so it fits `BoardInputView`'s collider to `BoardLayout.Box` (the board with its frame, ground to block tops), the one property `CameraFraming` frames too, so the two cannot drift apart. The view sits on the board root's object, so the collider is in the board's own space and cannot be nudged away from the drawn board. `LevelStage` and `BoardBuilder` stay about what is seen, and the level editor's scene preview, which has no input, is untouched.
- Instead of: sizing it in `LevelStage.Show`, which would take an input view the preview has to pass as null; or building it as a piece in `BoardBuilder` (as the plan first said), which would put unused colliders into the preview and a collider into the piece pools.

## The Game scene's flow is one method per step, with the screens' buttons as its transitions
- Why: the flow has five steps (play, pause, won, continue, failed) and each screen's `Show` takes the callbacks of its buttons, so `GameEntry` reads top to bottom as the flow chart: `Won` opens the complete popup with `Continue`, `Failed` the failed popup with `Play` and `ToHome`. No step needs state of its own beyond the run and the HUD, and a new screen is one more method.
- Instead of: a state machine with a class per state, which spreads five short methods over five files and a transition table for no behavior the methods lack; screens awaited with UniTask, which the conventions keep for work that is really awaited and which leaves a pending task when the scene is left from a popup.

## The run that is played is a strategy chosen once
- Why: the player's run and a designer's test run differ in three things: which level, which number, what a win does to progress. `ILevelRun` holds all three, so the flow calls `_run.Level`, `_run.Number` and `_run.Advance()` with no branch on where the run came from; a third kind (a daily level) is one more class.
- Instead of: methods for each kind on the former `LevelSequence` (`CompleteCurrent`, `After`) with the flow keeping the level and choosing between them, which put the run's kind into the flow and needed the number worked out a second time for the HUD.

## A level ends on a settled board and never while paused
- Why: the reference opens the win screen after the last block has left through its door. The attempt knows the win the moment the last block is out, but its slide out is still playing; the session ends the level the first frame the outcome is not playing, the attempt is not paused and no block view is leaving (`BoardPresenter.IsSettled`, with no booster shot playing, `BoardInput.IsBusy`). The same check keeps the win popup from opening over a pause. Between the decision and the end the HUD's restart and pause do nothing (`PlaySession.IsDecided`), so a tap during the last slide cannot restart or leave a level the player has just won (found in review).
- Instead of: a delay as long as the slide, which drifts from the motion settings; a callback from the last block's tween into the session, which gives the view a say in when a level ends; ending at once and letting the popup wait, which needs every popup to know about the board.

## The HUD pulls the time and writes it only when the shown second changes
- Why: the time changes every frame but the display once a second. The HUD reads `PlaySession.RemainingSeconds`, rounds up to whole seconds and, only on a new second, writes mm:ss into a fixed char buffer given to TMP's `SetCharArray`, so the countdown allocates nothing and the session does not know the HUD exists.
- Instead of: the session pushing the time to the HUD every frame, which couples the play feature to its view; `string.Format` or `ToString("mm\\:ss")` per frame, which allocates.

## The settings popup is a feature of its own
- Why: the Game scene opens it as pause and the Home screen opens it in phase 5, so it belongs to neither Play nor Home. `Preferences` has its own catalog and installer; a scene that shows the popup lists the installer. The switches write straight to Core's audio and haptics services, which already save the choices, so the game keeps no copy.
- Instead of: the popup in Play, which Home would then reach into; a game-side settings model mirroring Core's saved state.

## The win reward is paid in a test run too
- Why: a designer testing a level from the editor sees the same complete popup and HUD as the player; paying the reward keeps that flow identical and costs nothing a designer cares about. Only the level progression is the player's own.
- Instead of: a run-dependent reward, which adds a branch for no benefit to the test.

## One press feedback component with its tuning in a settings asset
- Why: every button should answer a touch the same way. `PressFeedback` sits next to a button and shrinks it while pressed with a DOTween scale in unscaled time; its scale, durations and eases are one `ButtonFeedbackSettings` asset in the Boot scope, injected because screens are created through the container. The `Button`'s own transition is off, so the scale is the only press visual.
- Instead of: the `Button`'s Animation transition, which needs an Animator Controller per button style and runs an Animator on every button; a scale per button in each prefab, which spreads one tuning over many prefabs.

## Screen catalogs are registered through a scoped factory in Core
- Why: each feature registers its own catalog, and VContainer's collection rejects two singleton registrations of the same implementation type, which `RegisterInstance` always is; the Game scene's second catalog (Preferences next to Play) threw `Conflict implementation type: UIScreenCatalog`. `Register(_ => catalog, Lifetime.Scoped)` gives the same asset in each scope and lets any number of catalogs reach the service. The fix is in Core, with a test, and goes back to the template.
- Instead of: one catalog per scene listing every screen, which makes a scene's asset change with every feature; a wrapper type per feature's catalog, which only works around the container.

## The board is framed between the HUD and the booster slots
- Why: the HUD takes the top of the screen and the booster slots its bottom, so the board fits the window between them, as in the reference. The top and bottom margins are in the UI's reference units and the side margin a fraction of the screen's width, all measured from the safe area's edges: the UI is scaled by the screen's height (see the UI scaling decision), so the HUD and the slots take the same share of the height on every screen, and a notch moves the window down with the HUD. The framing turns the units into that share by the reference resolution's height, from `CameraFramingSettings`' copy of it, which a test keeps equal to `GameUIRoot`'s. The HUD and the slots follow the reference's measured sizes and places, so the margins (360 above, 340 below, 6 % of the width at the sides) leave the reference's window: the board is centered on the screen and on tablets fills about as much of the width as the reference's. A tall board at 1080 x 1920 is bound by the window's height and comes out a little smaller; on 20:9 phones every authored board is bound by the width and sits in the middle of the free band. On a tablet the board grows with the wider screen until the window's height binds it, as in the reference.
- Instead of: one margin above and below as a fraction of the height (as before), which left the board centered on the screen and under the HUD's coin counter once the camera came closer; the HUD reporting its rectangle to the framing, which ties the camera to UI objects for a number the layout already fixes; a lens shift on a physical camera, which changes how the field of view is set in the scene.

## Each piece prefab settles its model's orientation; the builder places every piece the same way
- Why: the supplied model files face different ways (the block pieces lie on their side, the frame pieces stand upside down, the wall's rounded cap runs across its X). A piece prefab is a neutral root with a `BoardPiece` at the piece's pivot and the model under it, turned and offset once inside the prefab, so every piece follows one convention (top up, the pivot on the ground, a frame piece long along X and facing out along +Z). Model facts stay in the prefab: the block lift and frame lift in the settings only raise or sink a piece from the ground, and the one model measure left in code is the block's height, for the touch plane. The builder only sets a position, a turn about the vertical and a scale, and someone fixing or swapping a model works in the prefab, where the result is seen, instead of in rotation constants in code. The root's `BoardPiece` holds the model's renderer, so a pooled piece is recolored without a lookup.
- Instead of: one mesh per prefab with the builder turning each file (`LayFlat`, `CapUp` and offsets in code), as before, which hid art facts in code and had to be changed in code for every new piece; baking the turn into the model files, which changes the supplied art.

## The board's proportions are tuning values; the prefabs keep the model as supplied
- Why: how tall and thick walls and doors are is a look decision, so it lives in `BoardArtSettings` as a block lift, a frame lift and a wall and door thickness. The frame pieces are deep extrusions (a bar 3.6 units), so their visible height is how far they stand out of the ground; a lift changes it without distorting caps. The blocks are flat plates of their own (see the block plates decision), so their lift is 0 and stays a tuning value. The thickness scales a frame piece along its outward axis only, and the layout uses the same values to keep the inner faces on the board's edge and the board's box around the doors.
- Instead of: scaling the models inside the prefabs, which would make the layout read measures out of prefabs or repeat them in the settings.

## A door or a wall run is one stretched piece
- Why: the supplied wall and door meshes are straight extrusions along their length, with vertices only on their two ends, so scaling one along its length keeps its cross-section and caps exactly; one piece per door and per run of wall between doors and corners gives the reference's smooth bars with no seams, and fewer pieces to build and pool.
- Instead of: half-cell pieces side by side or overlapped, which keep a seam wherever two pieces meet (the frame read as a row of posts); a mesh built at run time for each run, which the supplied pieces make unnecessary.

## A leaving block is cut by drawing blocks only over the board
- Why: the block must vanish exactly at the door line while the rest stays whole, from any camera angle and for any shape. A block is outside the board only while it leaves, so the blocks' shader (`Game/Block`) simply draws nothing outside the rectangle of the board with its frame, given once per level as shader globals (`BoardClip`); a leaving block passes over its dropped door and is cut at the frame's outer face, as in the reference. No block holds any cutting state, two blocks can leave at once, and the renderers stay on the SRP Batcher. Back faces, seen only through the cut, are drawn in a flat shade of the color, so the cut reads as a solid face. Doors and their cubes stand outside the board, so they have their own materials (URP Lit), set so the light brings them to their blocks' colors.
- Instead of: a cut plane per leaving block through a `MaterialPropertyBlock` on each of its pieces, which needs the view to collect its pieces and takes them off the SRP Batcher; a depth-only mask outside the door, which with a steep, wide camera also hides parts of the board and the cubes; cutting the mesh on the CPU every frame. A shaped board (phase 7) with a door on a notch's inner edge would need more than a rectangle.

## Color per material, not per renderer
- Why: under the SRP Batcher a material per color costs nothing (it batches by shader, not by material), and a designer sets each color on its material. A color per renderer (`MaterialPropertyBlock`, instancing properties or `renderer.material`) takes the pieces off the SRP Batcher or copies materials at run time. A door always matches its block, so a color is changed on both: the block material holds the color as seen and its shadow color, the door material a darker value that the door's lit shading brightens back to the block's color.
- Instead of: one material colored per renderer; door materials made at run time from palette colors, which needs lifetime handling in the game and the level editor's preview.

## A leaving block speeds up from rest and slides by its own length past the frame
- Why: in the reference the block starts at once, slowly, reaches the cut a moment later and a longer block takes longer to pass; a constant acceleration from rest over the block's length plus the frame's width matches all three, with one tuning value. The duration follows from the length (`SlideDuration`), and the ease is a quadratic ease in, which is exactly that motion. The door's drop and rise and the cube burst are timed from the same slide.
- Instead of: a fixed distance and duration for every block, as before, where a long block was cut off early and a short one hung on.

## A leaving block snaps onto its door cell before it slides
- Why: after a fast drag the drawn block trails the cell where the rule let it leave by up to a cell or more. Sliding from there, from rest, made it drift toward the door from afar. Jumping onto the door cell looked abrupt. The reference snaps the block in front of its door and then sends it out, and the stop at the door reads strongly, so the block snaps there with the release's snap (time and ease) and slides from rest after it. The door drops as the snap starts, and the burst is timed from the snap's start.
- Instead of: a jump onto the door cell, as at first; a slide from where the block is drawn, as after the tenth review; a slide that carries the drag's speed, which removes the stop the reference shows.

## The cube burst lives on the door prefab
- Why: the cubes come from the door line, in the door's color (a door always matches its block), so a particle system under the door's root, scaled with the door's length (shape scaling) and given the door's material, needs no pool, color lookup or placement of its own. The cube is Unity's built-in cube mesh: the supplied assets have no cube, and a primitive is not art.
- Instead of: a pooled burst prefab placed and colored per exit.
- Its emitter is a rectangle across the door with no random or spherical direction: a shape's scale, and the door root's length scale under shape scaling, scale the emission direction too, so a thin box sent the cubes along the door, and any random direction would be stretched along it by the door's length. The spread comes from noise instead.

## Blocks are flat plates made from the supplied pieces in Blender
- Why: the supplied block pieces are deep extrusions (1.6 units for a plate the reference shows at about a quarter of a cell), so the blocks were sunk into the ground. The held block's outline outlines a piece's whole mesh, and the hidden part spilled it below the block. `Tools/Blender/make_block_plates.py` cuts each piece where the ground met it, closes its bottom and moves it down to the piece's pivot, into `Assets/Art/Models/BlockPlates.fbx`; the supplied `BlockParts.fbx` stays as it was. The fix lives in the art, where an artist would make it, so no code works around the pieces' depth, and the script makes it again from the supplied file. The problem is reported in [Supplied assets](SuppliedAssets.md).
- Instead of: flattening the meshes in an import postprocessor, which hides an art fix in code; scaling the pieces down, which flattens the studs into engraved rings; outlining only what is visible (ZTest LessEqual), which leaves the outline open along the blocks' near bottom edges; clipping below the ground in the outline's shaders, which changes the vendored shaders and ties them to the board's globals.

## A reward and the level progress are saved on the press; animations only show them
- Why: on a phone the game can be killed or sent to the background at any moment, and losing coins or a finished level is the worst thing a player can see. Continue adds the coins (`CoinWallet.Add`) and advances the run (`ILevelRun.Advance`) before the coins fly, and Core Save writes each at once; the coin counter shows a number of its own that catches up as coins land, and every `Show` gives it the real balance again. The same holds for any later flow that pays, advances or spends.
- Instead of: paying in the coin flight's completion, which loses the reward when the flight never ends.

## Coins fly to the counter with DOTween; confetti is a ParticleSystem drawn by UI Particle
- Why: the coin flight is choreography the flow depends on: the counter counts each landing and the next level starts after the last one. A DOTween sequence knows its length and calls back on every landing, and the reference's scatter and arc are two eases. Confetti is fire and forget, so nothing depends on it and a ParticleSystem tuned in the inspector does it best. Core's `UIRoot` is a Screen Space Overlay canvas, which draws over every camera, so a plain ParticleSystem would be hidden under the popup's dimmer; [UI Particle](https://github.com/mob-sakai/ParticleEffectForUGUI) (MIT, OpenUPM, pinned at 4.14.2) bakes particles into a mesh drawn by the canvas, so any later UI effect can be a ParticleSystem too.
- Instead of: coins as particles pulled by `UIParticleAttractor`, whose per-particle callback can be missed when particles arrive together (issue #274) and whose end is not known, so the flow would wait a guessed time; confetti as a hand-written `Graphic` simulating its own particles, which is code to keep for one effect; switching the UI canvas to Screen Space Camera, which changes Core and every screen. The package writes its own settings to `Assets/ProjectSettings/UIParticleProjectSettings.asset` and the preloaded assets; that is its design and is left as it is. Its auto scaling is `UIParticle`, not the default `Transform`, which drives the object's scale even in the editor (a batch run saved it as zero), and its position mode is `Absolute`, since the default scales an emitter's distance from the center with the particles and threw the corner emitters off screen.

## A level change is covered by Core's scene fade, and the popup that led to it closes under the black
- Why: retry, restart and continue rebuild the board at once, which reads as a jump; the reference fades to black and back. Core's `ScreenFadeTransition` in the Boot scene already covers scene switches, sits above every scene's UI and blocks touches while covering, so the Boot scope registers it as `ISceneTransition` too and `GameEntry` covers, plays, uncovers. The level popups do not close themselves on these choices, so their dimmer stays until the screen is black. The fade is the flow's only awaited work, which the flow decision keeps for work that is really awaited; screens are still not awaited.
- Instead of: a second fade in the Game scene, which is the same component twice with two durations to keep in step; closing the popup first, which shows the bright board for the length of the fade.

## The time box flashes with an overlay, not a tint
- Why: the time box sprite is blue, so tinting it red multiplies into a dark purple; a red overlay made of the supplied rounded square fades in and out over it once a second in the last 30 seconds, on the countdown's own second changes, so a paused countdown does not flash.
- Instead of: a tint of the box, or a flash driven by its own clock, which would keep flashing while paused.

## The UI is scaled by the screen's height, through a game variant of Core's UI root
- Why: the game is portrait only and its screens are laid out at 1080 x 1920 reference units. The reference, measured at 9:16, 20:9, 10:16 and 3:4, keeps its UI the same against the screen's height on every screen: its HUD is about 9 % of the height each time, so on a 20:9 phone the UI is about a quarter larger against the width than at 9:16, and on a tablet the extra width stays empty. Matching the height does exactly that: the canvas is always 1920 units tall, about 864 units wide on a 20:9 phone and wider on a tablet. The layout is built for the narrow canvas as the reference's is: the HUD's bar with restart and pause is one group anchored to the center, the coin count to the right edge, the booster slots and the popups fit within about 820 units (21:9). Home was sized after the reference on a tall phone and is in the same height-scaled units. `GameUIRoot` is a prefab variant of Core's `UIRoot` with only the scaler's match changed, used by the Home and Game scenes and the prefab editing environment, so the setting lives in one place and Core stays game-agnostic; the coin flight's canvas scales the same way, kept equal by a test.
- Instead of: Core's half and half match, which is neither; matching the width, which oversizes the UI on tablets; fitting the reference into the screen (Expand), which is the height on tablets but the width on phones taller than 9:16, so on most phones the HUD, the slots and the popups came out about a fifth smaller than the reference's (this was the setting until the first device build showed it); changing Core's `UIRoot`, which every game shares, including landscape ones; an override on each scene's instance, which spreads one setting over three scenes.

## UI prefabs are edited in an environment that holds the game's UI root
- Why: a UI prefab opened in prefab mode is put under a default canvas without the game's scaler, so its sizes and anchors looked different from the game. `Editor/PrefabMode/Scenes/UIPrefabEnvironment.unity` holds only `GameUIRoot` and is the project's UI Environment (Project Settings, Editor), so a screen opened in prefab mode is drawn at the game's scale.
- Instead of: checking layouts only in play mode, or a second canvas made for editing, which would drift from the game's.

## The screens follow the reference's arrangement with the supplied art
- Why: the reviewers know the reference, and the supplied UI art is made for its screens (the level and time bar, the square restart and pause, the coin pill, the booster slots, the home button's icon), so each piece has one obvious place. The HUD, the level complete and failed popups and the settings popup are laid out as the reference's ([Reference](Reference.md), UI layout). Where the reference shows what the case leaves out, the screen keeps the place without the function: the booster slots stay locked pictures and the coins' "+" a button with press feedback only. The title and texts are the game's own, in the project's font.
- Instead of: a layout of the project's own, which reads as a different game next to the reference; the reference's extra parts (the out-of-time offer, lives on the failed popup, the settings popup's legal, purchase, support and language buttons), which the case does not ask for and which would be buttons with nothing behind them. The coin pile with light rays has no supplied art, so the complete popup shows one large coin.

## The screens use Lilita One, the one asset not supplied by the case
- Why: the supplied assets have no font, and TextMesh Pro's default (Liberation Sans) and its extras (Anton, Bangers, Oswald, Roboto) are narrow or angular next to the reference's heavy, rounded lettering. Lilita One is close to it and free under the SIL Open Font License; it sits with its license in `Assets/Plugins/LilitaOne/` and is listed in the README. Its TextMesh Pro asset is static with the printable ASCII characters, all the game's texts use, so its atlas and the presets made from it never change at run time; it is sampled at 72 points with 18 pixels of padding on a 2048 atlas (one channel, about 4 MB), because the presets' edges and shadows can reach only as far as the padding.
- Instead of: Liberation Sans in bold, which read as a placeholder; a dynamic font asset, which would add glyphs at run time to an atlas the title's material shares.

## Showing coins is a service of the Coins feature, not part of the win popup
- Why: coins can be earned on any screen (the win today, a reward or the shop on Home later), and the flight into a counter is how coins are shown, not how a level ends. `CoinFlight` lives in the Boot scope on an overlay canvas of its own, between every scene's UI and the scene fade, so it is drawn over any popup in any scene; the caller adds the coins to the wallet first and passes where they start and which `CoinCounter` they reach. The win popup only exposes its continue button's position as `RewardOrigin` and knows nothing about coins or the HUD. Every call is a flight of its own that adds its amount to the counter landing by landing, so rewards that overlap fly side by side and the counter ends on their sum; a flight is linked to its counter and dies with it, so a scene that unloads mid-flight is never called back.
- Instead of: a coin flow inside the win popup, which every other screen would copy and which drew above the HUD only because popups happen to be above it; a flight that finds the visible counter on its own, which needs a registry of counters, a service locator in effect; one flight at a time, where a second reward would cut the first and drop its callback.

## Screens are built from shared prefabs
- Why: a button style, the popup body or the coin counter changes in one place and reaches every screen, and a new screen starts from parts that already take touches, give press feedback and use the right lettering. `Buttons/Prefabs/` (`TextButton`, `TextButtonGreen`, `IconButton`, `CloseButton`), `Popups/Prefabs/` (`PopupPanel`, `PopupDimmer`), `Coins/Prefabs/CoinCounter`, `Preferences/Prefabs/SwitchRow`, `Play/Prefabs/UI/` (`BoosterSlot`, `Confetti`); screens hold instances with overrides (text, sprite, size). TextMesh Pro's default font is Lilita One, so a new text starts in the game's lettering.
- Instead of: one flat hierarchy per screen, which a person editing the project later must change screen by screen; a shared popup base prefab, which Unity cannot put under the existing popups without rebuilding them (it does not change an existing variant's base), so the popups share the dimmer and the panel as nested prefabs instead.

## The switch's off knob is Unity's built-in UI circle
- Why: the reference's off knob is a pale circle; the supplied circles are all colored (blue, green, red), and a tint can only darken a sprite, not lighten it. Unity's built-in `UI/Skin/Knob` is a plain white circle that a tint turns pale blue, an engine primitive like the cube of the door burst, not art. The on knob is the supplied `btn_toggle_on`.
- Instead of: the supplied blue circle, which stays saturated blue; the supplied rounded square, whose corners stay square at the knob's size.

## One quality level and no shadows
- Why: the game is one small, brightly lit board, and the reference shows it with hardly any shadow. One quality level (`Mobile`) with one URP asset (`URP-Mobile`) means the editor draws what the phone draws; before, the editor used the template's High Fidelity level (HDR, four shadow cascades, SSAO) and Android its Balanced level, so the look was tuned against a picture no player saw. The pipeline has no shadows, no extra lights, no HDR and no depth or opaque texture, and the renderer has no features; the light casts no shadow, so the dark strip a dropped door threw on the ground is gone. The blocks' shader keeps only its lit and depth passes. MSAA is off, as in the reference: 4x smoothed the frame's slanted edges under the tilted camera, but on a low-end phone the GPU could not hold 60 fps with it at the native resolution, and at phone pixel densities the edges alias only a little.
- Instead of: the template's three levels, which only matter when a game must scale across weak and strong devices and which split what the editor shows from what the phone shows; soft shadows tuned down, which costs a shadow pass every frame for a look the reference does not have.

## The blocks are toon shaded, the board lit from its far side
- Why: the reference's blocks read by their shading: tops in the block's own color, a shadow color of each block's own on the sides turned from the light (deeper and shifted in hue: yellow goes amber, red goes maroon), the block a little darker toward its bottom, and the faces leaning toward the light (the far half of each stud, the far edges) brighter than the top, which is what makes the studs read round. The blocks' shader (`Game/Block`) does exactly these four things with a few material values: a ramp on how much a face turns to the light between the color and the shadow color, a gradient on the height above the board's ground (taken in the board's space, as the supplied pieces are turned inside their prefabs), and a rim on how much a face leans toward the light's side. A block's top shows the material's color whatever the light's strength, so the colors and shadow colors are set as measured in the reference. One white directional light shines down from the board's far side, which puts the shadow on the near sides and the rim on the far ones. Ground, base, frame and doors stay on URP Lit, matte (smoothness 0), with environment reflections off, under a strong trilight ambient (brighter from above, a little darker from the sides, dark from below); their materials hold darker values that this light brightens to the reference's tones and, for the doors, to their blocks' colors. The scene's environment lighting is generated with lightmaps off (`GameLighting` lighting settings and a small `LightingData` asset next to the scene; the environment reflection is a custom one left empty, as nothing on the board reflects, so no reflection cubemap is baked), so the ambient is saved with the scene and is the same in the editor, in batch runs and in a build; without it, changes to the ambient colors did not reach the ambient probe in batch runs, and in the editor they show only after Generate Lighting.
- Instead of: physically based shading on the blocks, which darkens every face by how far it turns from the light and gives no rim, so the studs read flat; a ramp alone, or a ramp with a darker copy of the color as its shadow, which matched the tops but looked flat and dull; a white highlight, which drew thin white lines instead of a lit edge; light from the camera's side, which flattens the studs and lights the frame's faces toward the player; glossy materials, which turn the tops white under a light from behind.

## A base under the ground tiles
- Why: the reference shows the cells on a lighter base that reads as thin lines between them. The supplied ground tile is a flat rounded square a little smaller than a cell, so one more tile, stretched under the whole board and half the wall around it and set a little lower, gives the lines without new art, and the board's edges show no background. It is one pooled piece per level (`GroundBasePiece`, a variant of the ground piece with its model lowered and its own material).
- Instead of: line pieces between the cells, which need art and a piece per line; the background showing in the gaps, which draws light lines on a dark board.


## Home's tab bar is a toggle group, its panels swap through the toggles' events
- Why: as in the reference, the tab bar is navigation, not a row of buttons: one tab is always selected, and the selected one widens, shows its card, raises its icon and grows its label. uGUI's `Toggle` and `ToggleGroup` (switching off not allowed) give "exactly one on" without code; a `TabView` per tab changes the tab's width and card at once and tweens only its icon and label, as the reference does (tweening the width rebuilt the bar's layout every frame and, with an overshoot, read as jerky). Each switching tab's `onValueChanged` is wired in the screen prefab to its panel's `GameObject.SetActive`, so the panels swap at once, as in the reference, and the tab prefab holds no reference it cannot fill (a tab only knows its panel once placed in the screen). Locked tabs are not toggles: they shake on a press and are never selected.
- Instead of: buttons with press feedback and a script keeping one selected, which writes what `ToggleGroup` already does; a panel field on `TabView`, which stays empty in the tab prefab and fails the prefab reference test; a sliding highlight shared by the tabs, which the reference does not have.

## Home's level nodes are plain rounded squares
- Why: the reference's nodes are rounded squares with four studs; the supplied art has the green square (`Btn_level_green`) without studs and no slate one. The current node is `Btn_level_green`, the locked ones the supplied rounded square tinted slate blue on a gold one, with the supplied lock; the path's rails and band are plain tinted rectangles.
- Instead of: studs faked from the small round buttons, which adds four images per node for a detail the case does not ask for; tinting `Btn_level_green`, which a tint can only darken, so it cannot turn slate.

## Game code carries no comments; reasons and contracts live in the notes
- Why: comments drift from the code and were written where the code already spoke; the system notes and Decisions are read anyway and are where a reader looks for why. An interface's contract, an engine quirk a reader could "fix" wrongly or a measured value goes into the note under the name of the type it is about. A hook refuses any edit that adds a comment to `Assets/Game`. Core is a package copied into other projects and read without this project's notes, so it keeps short comments on its public contracts.
- Instead of: comments only for reasons, which kept growing into comments on what the code does; XML doc comments, which repeat the signatures.

## Mechanics plug into the board through role interfaces; the board knows none of them
- Why: the board keeps what every mechanic shares (its blocks, occupancy, moves, exits, the count of blocks gone, breaking a block or a cell) and asks two role interfaces at fixed points: `IMoveRule` whether a block may step or leave in a direction, `IBlockGuard` whether a block is guarded and, if so, to take a hit for it. `IceMechanic` plays both roles and owns the ice's state; the arrow block will be a second `IMoveRule`. A new mechanic is a class, a field in the level format, one line in `LevelMechanics.For(level)` and, when it has a level rule, one check in `LevelValidator`'s list; `LevelMechanics.For` is the one place that builds the mechanics for the game, the level editor and the solver alike; the board, the drag, the attempt and the booster effects stay as they are. Two interfaces, not one, so a mechanic that only limits moves has no empty guard methods.
- Instead of: the ice's hits and a chip method on the board, which makes every mechanic an edit to the board; one mechanic interface with every hook, which leaves most of them empty; mechanics built from a data-driven list in the level, which needs a type registry in logic, view and editor for two mechanics and turns a missing registration from a compile error into a silent one at run time.

## Ice counts the blocks gone and is read when asked; views catch up by pulling
- Why: every way a block leaves the board entirely ends in the board's one private removal, which counts it, so a block's ice left is its starting ice minus that count minus the hits it took: nothing has to be told that a block went, a restart starts from a new board, and the solver gets ice right through the same board. Views work the same way: whoever changes the board (the drag, a booster) calls one `CatchUp`, and each block view compares the logic's state with what it shows (ice left, shape, gone) and plays the difference; new blocks from a split get views from the pools.
- Instead of: a counter on each iced block lowered by a "block left" event, an event bus carrying flow inside one feature, with state to reset; the logic returning what happened for the views to switch on, which needs a switch over kinds and lets the views drift from the logic.

## A booster is a logic effect and a presentation
- Why: what a booster does to the board is a rule, so it lives in `Game.Logic` as a command (`IBoardEffect`: `CanTarget`, `Apply`) and is tested without Unity; `LevelAttempt.TryApply(effect, x, y)` is its one way in, checking the state, applying, checking for a win and ending the aim. In `Game`, `TargetedBooster` fixes the order every aimed booster follows (aim, apply, stock down and saved, its own animation, catch up), so the stock goes down only when a shot lands and before anything is shown; each booster adds only its effect and its animation. Freeze has no target, so it calls `LevelTimer.Freeze` and implements `IBooster` directly.
- Instead of: a method per booster on `LevelAttempt`, which changes the attempt for every booster; one effect interface for Freeze too, with a target cell it ignores.

## Board touches go through modes: drag, pick, busy
- Why: aiming turns a tap into a choice of cell instead of a grab, and while a shot plays the logic is already ahead of the views, so a touch could hold a block that is gone or a split part whose view is not built yet. Each mode is a small state `BoardInput` switches between (`Drag`, `Pick`, `Busy`), and the board input stays one collider and one set of handlers.
- Instead of: flags checked in the grab, drag and release handlers, which every new mode would add to.

## A rocket hit splits a block into separate blocks
- Why: the reference does it: what remains of a block after a cell is blown away becomes one block per group of cells touching side to side. The hit block keeps its index for its first group and the others are appended, so the drag, the views and the leave animation keep their indices; the board's block list has room for one block per cell of the level, so a split never allocates.
- Instead of: a block that moves as one with a gap in it, which the reference does not have and which the exit rule (every cell inside the door's run) would treat oddly.

## Aiming does not stop the time; the buy popup does
- Why: the reference keeps the timer running while a booster aims and while it fires (it stops for under a second during the rocket's flight, too short to matter), so aiming costs time and the boosters stay a choice under pressure. The buy popup covers the board and asks for a decision about coins, so it pauses the level like the pause popup.
- Instead of: stopping the time while aiming, which makes aiming free; a running clock under the buy popup, which punishes reading it.

## A purchase saves the booster stock before spending the coins
- Why: Core Save writes each key on its own, and the stock and the coins are two keys, so the two writes are not one step. Saving the stock first means a crash between them leaves the player with boosters they did not pay for rather than coins gone for nothing.
- Instead of: one save document for coins and boosters, which ties two features' data together for a window of milliseconds.

## Mechanics and views read the board through a read-only view
- Why: the ice's state is a count read when asked, so whoever shows it needs the board's `GoneCount`, and the views catch up by reading the board's blocks (the phase 7 design gave the views the ice but not the board it is read from); but the level attempt keeps its board so that no caller steps or hits a block past its rules. The board is shown by permission, never as the concrete `Board` outside its owner: `IReadOnlyBoard` (`GoneCount`, `Blocks`, `BlockAt`, `IsGuarded`) to whoever only reads (the roles, the views through `LevelAttempt.Board`, an effect's `CanTarget`), `IBoard` (read plus `Hit` and the rocket's `HitCell`) to an effect's `Apply`, so its signature says an effect only hits; `Board`, which also steps blocks and lets them leave, stays inside the attempt.
- Instead of: the attempt passing on each fact a view needs (`IceLeft`, a block's cells), which adds a method to the attempt for every mechanic; a provider that joins the ice and the board for the views, which only forwards and still needs a read-only board for the blocks; the whole `Board` on the attempt, which lets a view move blocks; one interface per reader cut by the fields it reads (an `IBoardFacts` with only `GoneCount` for the roles, tried and dropped), which multiplies types without a difference in what a reader may do.

## The board gives a booster's hit to the guards first
- Why: "a guarded block takes the hit instead of breaking" is one rule for every booster that hits (the hammer on a block, the rocket on a cell). With a public `Break` and the order written in each effect, an effect that forgot to ask the guards would break an iced block. `Board.Hit(block)` (and the rocket's `HitCell`) asks the guards and removes the block only when none takes the hit; the removal itself is private. So every public way to change the board keeps the rules, and an effect handed the board can do nothing a rule forbids. The vacuum leaves guarded blocks out by `IsGuarded` before hitting the rest, so it never takes their ice.
- Instead of: `Break` and `TryAbsorbHit` public with each effect calling them in order, which leaves the rule to every effect; the concrete `Board` in an effect's `Apply`, which would be safe once its methods keep the rules but would let an effect step blocks and say nothing about what an effect does; an effect base class only `Game.Logic` can extend, which keeps fake effects out of the tests.

## Booster code sits in the play feature's role folders
- Why: the boosters are part of the play feature (they aim through its board interaction and start through its session), and the folder rule fixes where each kind of file goes, so the boosters spread over the roles the rule already has: the boosters and their store in `Play/Scripts/Services/`, the slots, overlay and shots in `Play/Scripts/Presentation/Boosters/`, the definitions and motion in `Play/Scripts/Settings/`, the buy popup in `Play/Scripts/UI/`; stock and purchase in `Logic/Boosters/` like the coins, the effects in `Logic/Play/Effects/`.
- Instead of: one `Play/Scripts/Boosters/` folder (the plan's working name), a role the folder rule does not have; a feature of its own, whose code would name the play feature's insides.

## One targeted booster class; a booster is a definition, an effect and a shot
- Why: every aimed booster follows the same order, so `TargetedBooster` owns it and takes what differs as parts: the logic effect (`IBoardEffect`) and the shot prefab its `TargetedDefinition` names (`BoosterShot`, a view with its own animation). A new aimed booster is an effect, a shot prefab, a definition asset and a line in `PlayInstaller`, with no new booster class and no change to the flow. A definition is a settings asset per booster; what only one kind has (the freeze's seconds, an aimed booster's shot) is on a subclass, so no asset carries a field it leaves empty.
- Instead of: a `TargetedBooster` subclass per booster overriding its animation, which repeats a class per booster for one method; one definition class with the shot and the seconds on all, which leaves fields empty that the installer test reports.

## The aiming overlay dims around the board and copies the active slot
- Why: the board is drawn by the camera under every UI canvas, so the overlay cannot dim the screen and leave a hole with one image; four dim rectangles around the board's rectangle on screen (its box with the frame, projected) leave the board bright and let a tap there reach the board's collider, while a tap on any dim cancels. The active slot shows above the dim as a copy of its art placed over the real one, which keeps the slot row and its layout as they are; a press on the copy cancels, as the button again does in the reference.
- Instead of: a stencil cut-out shader, new art and material for one effect; raising the real slot above the overlay with a nested canvas and its own raycaster, a sorting override on every slot for a state that lasts seconds.

## Booster economy: 2 free, 3 for 100 coins
- Why: the reference sells 3 for 1400 coins, but our levels pay 40 coins each, so its price could not be paid within the case's five levels. Each booster starts with 2 free and a pack of 3 costs 100, so a player can buy a pack every few levels; all values are in each booster's definition. When the coins are short the popup's buy button is off and half transparent; a purchase closes the popup and resumes the level, and the player presses the booster again to use it.
- Instead of: the reference's 1400, a price no player of this build can reach; a button that opens a shop, which the case leaves out; aiming at once after a purchase, which turns the popup's close into an action.

## Hammer and vacuum shots: blocks shrink away, the vacuum pulls them in
- Why: agreed with the developer: the vacuum grows a little above the tapped block and the taken blocks float up to it one after the other, shrinking until they vanish; the hammer flies from its slot, strikes, and the block shrinks into the struck cell. The shot moves the views of the blocks the effect took through `BoardPresenter.VanishGone` before the flow's catch up, and a vanishing view counts as leaving, so the catch up leaves it alone and a level won by the shot ends after it.
- Instead of: hiding the taken blocks at once, which reads as a glitch; a break burst, which needs particles per block for the same message.

## The booster slots are placed in the HUD and bound in order
- Why: the slots' buttons get their press feedback injected when Core makes the screen, so slots made at run time would need the container in a view. The HUD holds one slot per booster the game will have, the bar binds them left to right to the registered boosters and hides the rest; a booster added later (the Rocket, second in the slot order) takes its slot without a prefab change. This replaces the locked pictures of "The screens follow the reference's arrangement with the supplied art".
- Instead of: instantiating slots through the container from the HUD, which hands a view the resolver.

## A booster pressed with stock goes from the HUD to the session; only the buy popup is a flow step
- Why: using a booster needs the HUD's own overlay and shots, and the session is made before the screen, so a press routed through `GameEntry` had the flow take the HUD's insides out and hand them back to the session (found in review). The HUD knows its slots, its overlay, its shots and the stock its badges show, so it calls `PlaySession.Use` itself with stock and calls `GameEntry`'s out-of-stock callback at zero; the buy popup, which pauses the level and opens a screen, stays a step of the flow. This narrows "The Game scene's flow is one method per step": a press that changes no screen is not a step.
- Instead of: `GameEntry` routing both presses with `hud.Aiming` and `hud.Shots` passed through, which keeps every button in the flow but exposes the HUD's parts to it; the overlay and shots registered in the container, which needs them in the scene rather than in the HUD prefab Core opens.

## A shot's tween is killed through its target before it plays again
- Why: a shot's sequence ends by hiding the shot, and a second hammer or vacuum can start while the first one's shot is still fading out, so the old sequence's end hid the new shot and killed its tween before it landed: the board stayed busy and the level could not end (found in review). `BoosterShot.Play` kills the shot's own tween (`DOTween.Kill(this)`, the sequence targeted at the shot), shows it and finishes the sequence its subclass builds; a subclass only builds the sequence, so the Rocket's shot gets the same guard.
- Instead of: each shot keeping and killing its own sequence, which every new shot has to remember; waiting for the earlier shot to finish before a booster can be used, which holds the player for a fade.

## A split's parts keep the hit block's position
- Why: a block's cells are its shape relative to its `Position`, and its view's pieces are laid out the same way from the view's origin. When the parts keep the hit block's `Position` and only their own cells, the hit block's view stays where it is and is rebuilt from its cells left, and each new part's view is built at that same position, so every part appears exactly where its cells were, with no move and no recomputed anchor. Nothing reads `Position` as one of the block's cells: the drag keeps the grab relative to it, the exit and the leave read the cells and the extent.
- Instead of: moving each part's `Position` onto its own first cell, which needs a new origin for every part, a shifted shape and a view moved by the same offset in the same frame as its rebuild.

## The rocket's burst is the rocket's animation, not the catch-up's
- Why: the rocket knows the cell it hits, so the burst plays where it lands for every hit alike: a cell taken, a block's last cell (the block is gone, its shape never changes) and an iced block (one ice taken). `CatchUp` sees only what changed in a block's shape, so it would miss the last cell (the block is gone) and the ice (nothing changes shape). So `CatchUp` only rebuilds a view whose block lost cells and builds views for appended blocks; the burst comes with the rocket's shot, `RocketShot` (decided with the developer).
- Instead of: a burst on the lost cell in `CatchUp`, as first planned.

## The rocket's burst takes its color from the views, before they catch up
- Why: when the shot lands, the logic has already taken the cell, so the board can no longer say whose cell it was (the cell is empty, or the block is gone). The views still show the board as it was until `CatchUp`, and each view keeps the cells it was built from, relative to its block's `Position`, which a split leaves unchanged; so `BoardPresenter.TryFindColorShownAt` finds the view still showing the cell and gives its color. It works the same for a cell taken, a block's last cell and an iced block, and nothing in the booster flow changes.
- Instead of: reading the block's color before the hit in `TargetedBooster` and passing it to every shot, which changes the contract of every shot for one booster's burst; a burst in one fixed color, which reads as an explosion beside the block rather than the block's own pieces flying off.

## Crosshairs are marked by the effect's own target check
- Why: the reference marks every cell of every block, iced ones too, which is exactly where the rocket can aim, so the overlay marks every cell of a block for which the effect's `CanTarget` is true; a definition's `MarksTargets` turns it on (only the Rocket's). The marks can never disagree with what a tap does, and a booster that wanted marks later only sets the flag.
- Instead of: a crosshair on every cell of every block drawn without the effect, which repeats the rocket's rule in the overlay; crosshairs as a separate view per booster, which adds a type for one image per cell.

## A locked block is not held; it shakes
- Why: an iced block cannot move, so holding it lit an outline that promised a drag that never came. The developer chose a refusal: the attempt holds no block that no move rule lets go any way (`Board.IsLocked`, so a later mechanic that stops a block entirely gets the same answer), still starts the timer, and names the block in a locked pose; the view gives it a short sideways punch.
- Instead of: holding it with the outline, as before; no answer at all, which reads as a missed touch.

## Ice is drawn on the block's own pieces through a property block
- Why: in the reference an iced block is the block's own shape, studs and all, in light blue ice with a number on it. One `MaterialPropertyBlock` on the block's piece renderers swaps its colors for the ice's and lays the supplied `icetexture.jpg` over it in the board's space (the `Game/Block` shader's surface map, white on every other material); the melt fades the colors back and takes the block off. The renderers' materials stay as they are, so the outline's material arrays, read once per level, stay right.
- Instead of: a separate translucent cover, which needs its own geometry per shape and shows the block's color under it, unlike the reference; swapping in an ice material, which the outline would restore over on a later hold and which cannot fade.

## The validator reports every block that too much ice locks
- Why: ice melts only by blocks gone, so once the sorted ice outruns the blocks that can leave, every block from there on is locked for good, not only the first. Marking each of them on the board shows the designer the whole problem in one look.
- Instead of: one error on the level, which leaves the designer to find the blocks; one error on the first block, which hides the others until it is fixed.

## A selection lists its number fields
- Why: a block has a number of its own (its ice), a door none. Each selection gives `Fields` (`SelectionField`: label, tooltip, getter and setter on the level, and the number input it builds), and `SelectionFieldsView` adds whatever the selection lists, so the window never asks what kind of selection it holds and a new block number is one entry in `BlockSelection`. Every change runs through the window's `Edit`, one undo step.
- Instead of: an ice field the window shows when the selection is a block, a type check that grows with every property; ice on `ILevelSelection`, which a door would have to ignore; field kinds for choices, built for an arrow dropdown that the Arrow tool replaced.

## Ice is given with a tool: a click adds one, a right click takes one off
- Why: the arrow is drawn on the board with a tool, so ice is given there too: the designer works on the board with tools for every mechanic, and the side panel's Ice field stays for typing a number. A right click is a second action every tool can answer (`ILevelEditorTool.RightClick`); the tools without one return that nothing changed. The developer chose it over Shift and a click.
- Instead of: the Ice field alone, the one mechanic set from the side panel; Shift with a click, which needs a key held while the hand is on the mouse.

## The designer draws the arrow; the level stores where it lies
- Why: the supplied `Arrows.fbx` holds three double-headed arrows, one, two and three cells long, so an arrow names an axis, not a direction. Where the arrow lies on the block and how long it is are the designer's choice (a short arrow on a long block, the lower row of an S), so the Arrow tool draws it with a drag along a row or column and the level stores it as `ArrowData` (axis, first cell relative to the block, length), as a door stores its side, start and length. The move rule reads only the axis, so where the arrow lies never changes how the block moves; the view and the editor read the rest. Where it lies decides only which part of a rocket's split keeps it ("A rocket's split gives the arrow to the part holding its cells"). The validator checks only that the arrow lies on the block's own cells (`ArrowOffBlock`); the three-cell limit belongs to the art, so the tool keeps to it and a test holds it to the prefab's models. Erasing or cutting off a cell under the arrow removes the arrow. `ArrowMechanic` is a second move rule: the board asks it for steps and, with the direction out through the door, for exits, so the arrow limits leaving too without a line in the board; the leave warning counts only doors on the block's axis. Chosen with the developer.
- Instead of: placing the arrow by a rule on the shape (the longest run of cells along the axis), which needed geometry code for every shape, chose arbitrarily between equal rows and could not give a short arrow; a dropdown for the axis, which a drag gives in the same gesture as the place; one-way arrows, which the supplied models do not draw.

## A rocket's split gives the arrow to the part holding its cells
- Why: the arrow is drawn on cells of the block, so after a split it belongs where those cells went; chosen by the developer. `ArrowMechanic.ArrowOf` reads it when asked from the level block a part came from (`BoardBlock.LevelBlock`, a board fact any mechanic with per-block level data can use) and the part's shape, so nothing is copied at the split and the board still knows no mechanic. A hit on one of the arrow's cells leaves no part holding it, and the arrow is gone.
- Instead of: the hit block's index keeping the axis whatever cells it kept, which showed the arrow over cells the block no longer had and gave the axis to a part without the arrow; telling the mechanics about a split, which makes the board call its roles for an event only one of them needs.

## The level attempt makes the board and its mechanics together
- Why: a board and its mechanics share state (the ice's hits count against the board's blocks gone), so they must be made in pairs; when the session built the mechanics and handed them in, nothing stopped a second board from sharing them, and handing the views the whole set gave them its guard list and so `TakeHit`. Now `LevelAttempt(level)` builds both, `Board`'s constructor and `LevelMechanics`' rule and guard lists are internal to `Game.Logic`, and the views read the mechanics they show by name through `LevelAttempt.Mechanics`. The compiler holds the boundary, and a new mechanic with a view adds a property, never a parameter to `BoardPresenter.Show`.
- Instead of: passing each shown mechanic to `Begin` (`Ice`, `Arrow`, one more parameter per mechanic); a note saying not to build a second board; `LevelMechanics` building the board, which makes a second call build a second board on the same mechanics.

## Frame rate and screen-on in a Core module without a service
- Why: phones run at 30 fps unless `Application.targetFrameRate` is set, and a timed puzzle must not let the screen dim; every mobile game made from Core needs both, so they belong to Core, not to this game. `Core.Display` applies a `DisplaySettings` asset (60 fps, screen on) once when the root container is built. Nothing calls it afterwards, so it has no service or interface.
- Instead of: a startup class in the game's bootstrap, which leaves the next game without it; a service and interface with no caller, which would only forward two assignments.

## Mobile build settings after the reference, Vulkan with GLES3 fallback, managed stripping Minimal
- Why: the reference game is built with the same Unity version, IL2CPP and ARM64, minimum API 24, target API 36, with both Vulkan and GLES3, so these choices are known to ship. Vulkan stays first (Unity's default, with its own deny list for bad drivers) and GLES3 catches devices without it; both are checked on a device. Stripping stays at Minimal because VContainer creates services through reflection, and the size saved by higher levels is small next to the risk of a constructor stripped in the player only.
- Instead of: GLES3 only, which gives up Vulkan's lower CPU cost for compatibility already covered by the fallback; Low or Medium stripping with a `link.xml` to maintain.

## The board's touch path is three classes: input, presenter, projection
- Why: one class had grown six jobs (touch modes, drag, the leave animation, screen geometry, keeping the block views in step with the attempt, and lookups for the boosters' shots), so every booster and every mechanic with a view changed it, and the boosters held the whole of it to read one screen point. `BoardInput` turns touches into calls on the attempt by mode; `BoardPresenter` owns the block and door views indexed like the attempt's blocks and shows what the attempt returns (hold, refuse, follow, snap back, leave through a door, vanish, catch up); `BoardProjection` converts between screen points and board cells. The overlay and the shots get the projection and the presenter injected, so a booster's context carries only the attempt, the input and the presenter it calls. The door view keeps its side, so leaving needs no level data.
- Instead of: one board interaction for all of it; a facade over the three, which would only forward; the projection folded into `TouchPlane`, which stays the plane math the projection uses.

## A mechanic shows itself on a block through a block feature
- Why: the block view, its builder and the board presenter each named the ice and the arrow (serialized fields, `Ice` and `Arrow` properties, an ice and an arrow parameter on every build, an ice call in the catch up), so every new mechanic would have changed all of them. Now each mechanic with a view has one plain class, `IceFeature` or `ArrowFeature` (`IBlockFeature`: `Show` and `CatchUp`), registered by the installer with its own settings; `BlockViewBuilder` shows every feature on each view it builds or rebuilds and catches them up when the presenter asks, and the view parts (`IceCover`, `BlockArrow`) only present what the feature gives them. A new mechanic adds its feature, its part on the block prefab and its line in `BlockFeatures.Of`, the one list the installer registers, the scene preview hands to `LevelStage` and the tests use; a feature with settings adds a parameter there, which the compiler carries to those three callers, so none can fall behind. A view part is found with `BlockView.Part<T>()`, a lookup by type unlike the one rejected for mechanics below: a part is a child component of the prefab, finding it by type is the engine's own way, the alternative is a serialized field per mechanic on the view (the coupling this removes), and the builder tests fail when the prefab lacks a part.
- Instead of: a field and a call per mechanic in the view, builder and presenter; settings serialized on the view parts, which skips the installer that owns tuning; `[Inject]` on the view parts, which pooled instances never get (the pool instantiates without the container) and the scene preview has no container for.

## Block features reach their mechanic through a typed property of `LevelMechanics`
- Why: `LevelMechanics.For` already lists every mechanic of a level, so a typed property costs one line in the same file and the compiler checks it. A lookup by type (`Find<T>()`) was considered: it is not a service locator (it is local to one attempt and passed in), but it needs a type check on the list, which the code style rules out, and a missing mechanic fails only at run time, while it saves nothing over the property.
- Instead of: `Find<T>()` over the mechanics; each feature making its own mechanic and handing it to the attempt, which makes a scoped feature hold per-attempt state that every restart must replace, and moves the list of mechanics out of `Game.Logic`, where the validator needs it.

## Block views are built from the attempt, also in the scene preview
- Why: a staged block read its ice and arrow from the level data and a block built at run time from the mechanics, two paths for one picture (the split part's "no ice" was a literal 0). Building every view from the attempt (`BlockOnBoard`: read-only board, mechanics, index) gives one path. The scene preview makes a `LevelAttempt` for the level it shows; a board cannot be made from a level with a block cell off the board or two blocks on one cell, so the preview shows nothing then (`Board.CanHold`), which the editor's own edits never produce and its problem list reports.
- Instead of: two build paths, one from `BlockData` and one from `BoardBlock`, each feature reading both.

## The editor's board draws mechanics through one drawing each, in two layers
- Why: the board view held an if per mechanic and the drawing code of both. `IBlockMechanicDrawing` (`DrawOnBlock`, `DrawOnTop`) moves each mechanic's drawing into its own class over a small `IBoardCanvas` (one implementation, the board view, kept as an interface on purpose: a drawing gets cell geometry and a way to add a box, not the view's input events and `Show`), and the two layers keep the order that left all three marks readable (tints and arrows under badges). Arrow and ice edits moved out of `LevelEdits` into `ArrowEdits` and `IceEdits`; `LevelEdits` still drops an arrow left off its block after cells are removed, the one edit of a mechanic that follows a shape change.
- Instead of: an if per mechanic in `LevelBoardView` and in `LevelEdits`; one editor interface per mechanic for drawing, selection fields and descriptions together, which no second mechanic needs yet.

## The leave check reads the arrow's axis, not the move rules
- Why: `BlocksCanLeaveCheck` asks whether a block could ever leave through a door of its color. The move rules answer whether it can move now: ice forbids every direction until it melts, so checking them would report every iced block as stuck. Only a lasting restriction belongs in the check, and the arrow is the only one.
- Instead of: running the move rules on a board made for the check.
