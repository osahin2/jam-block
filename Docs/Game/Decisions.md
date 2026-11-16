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

## Hand-written motion, no tween library
- Why: the drag is a per-frame follow, not a tween; the remaining motions are few (settle on release, slide out through the door with a pooled particle burst, popup open) and are written in the views without per-frame allocation.
- Instead of: DOTween, which adds a large third-party import for three motions.

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

## Level validity is checked in one validator, not by exceptions in the board
- Why: the editor needs every problem of a level, with the block, door or cell it is about, and the difference between "cannot be played" and "probably a mistake"; exceptions give only the first problem as text. `LevelValidator` returns all of them. The editor shows them, a test checks every level asset in the project, and `Board` refuses a level with errors by throwing once with the whole report, so a broken asset cannot silently corrupt the occupancy grid (a cell past the right edge would land on the next row). Warnings do not stop play.
- Instead of: checks inside the `Board` constructor, which the editor could only reach through try/catch and which would have to be written a second time for the editor's warnings; or a `Board` that trusts its input, which leaves a broken asset to fail as a wrong collision far from its cause.

## A blocked step is a caller bug and throws
- Why: the view asks `CanStep` every frame for the sub-cell drag offset and commits only free steps, so a blocked `Step` happens only through a bug. Throwing stops it before two blocks share a cell.
- Instead of: `TryStep` or a "blocked" result, which turns a bug into a silent no-op and adds a branch every caller must handle.
