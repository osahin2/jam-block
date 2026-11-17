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
- Why: observed rule. A block leaves as soon as it touches a side of the board where a door of its own color covers its whole extent along that side, from any drag direction; a door of another color is wall. Checked in one place in the logic after each step.
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
- Why: the case asks for a coin inventory whose count stays correct, and nothing in its required scope spends coins: the reference spends them on boosters, the shop, lives and the out-of-time offer, all left out above. `CoinWallet` keeps the balance non-negative and rejects negative amounts as programmer errors; saving is the `Game` side's job. The first spend planned is the out-of-time offer, optional after the required scope (phase 7 in the plan).
- Instead of: a spend rule nothing calls, which is dead code; or building the out-of-time offer before the editor, which the evaluation weighs more.

## Board input through the EventSystem on one board-sized collider
- Why: the EventSystem sends a press to the UI first, so HUD buttons and popups block the board without extra checks, and it is the input path the UI uses anyway. One collider covers the board, so touches outside it never arrive, and which block is under the finger stays a logic question (`Board.BlockAt`), not a physics one. Drag positions are projected onto the touch plane, which also works when the finger leaves the collider. The visual spike polled `Pointer.current` to move fast; it is replaced with the HUD in phase 4.
- Instead of: polling the pointer each frame, which ignores UI; or a collider and handler per block, which moves the block lookup into physics and needs colliders rebuilt as blocks move.

## The play session runs the level; the board interaction runs the touches
- Why: they change for different reasons. `PlaySession` owns the level's lifetime and time (build, tick, and in phase 4 the HUD, win, fail, restart and next level); `BoardInteraction` owns the touch path (pointer to attempt to block views), which changes with input and motion. Splitting before phase 4 keeps the flow from growing inside the touch code.
- Instead of: one session class that both connects the views and runs the flow.

## The level editor window is UI Toolkit only
- Why: the editor is evaluated for usability, and UI Toolkit gives it for little code: fields bound to the working copy follow undo by themselves, a problem list, toolbar toggles, hover and selected states in USS, layout in UXML apart from the code. Everything the window needs exists in 2022.3; the Unity 6 APIs that do not are listed with their replacements in the `add-game-feature` skill. No IMGUI inside it: if UI Toolkit blocks something, the whole window moves to IMGUI.
- Instead of: IMGUI, stable and quick for painting a grid, but hover, selection states and the sync between fields and undo are hand-written; or a mix, a board in an `IMGUIContainer` inside a UI Toolkit window, which leaves two UI models in one window.

## Levels are previewed in the Game scene, built by the game and never saved
- Why: the designer sees a level in the Scene and Game views as the game will build it, framing included, without entering play mode. The preview uses the game's own `BoardBuilder`, so there is no second way of drawing a level, and its objects are `DontSave` and not editable, so the level stays in its asset only and the scene file never changes. It shows the level open in the level editor, live, or else the level asset last selected in the Project window; following the selection while no window is open needs an `[InitializeOnLoad]` editor hook, the one static entry point, called only by the window.
- Instead of: saving the level into the scene, which gives a level two sources and a scene diff per level edit; or a preview inside the editor window (`PreviewRenderUtility`), which does not show the game camera's framing.

## A block may not start at its own door
- Why: a block leaves as soon as it touches a covering door of its color, but the board only checks after a step, so such a block would sit at an open door until moved away and back; and the solver assumes every block starts on the board. A designer never wants this, so the validator reports it as an error, decided by the board's own exit rule (`Board.ExitDoorOf`), not a second copy of it.
- Instead of: letting the game remove the block when the level starts, which plays a leave the player did not cause.
