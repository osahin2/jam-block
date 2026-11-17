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
- Why: one asset per level gives undo, the inspector and asset references in the editor for free; the level types in `Game.Data` are plain C# with an own `Cell` and a color id, so logic and tests never touch the engine, and the view maps color ids to the supplied art.
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
