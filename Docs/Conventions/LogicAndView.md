# Logic and view

> Summary: `Game.Logic` decides and returns what happened; views in `Game` show state and forward input; one flow class per feature connects them and owns the order of steps. How a result reaches the views is kept as simple as the feature allows.

## Rules
- **Logic decides.** Whether a move is allowed, what it causes, when the level is won or failed, the coin balance: plain C# in `Game.Logic`, tested in EditMode without a scene.
- **Logic reports through return values and queryable state.** The caller that caused a change gets what it needs back from that call. A notification is for state that is read by others who did not cause the change ([Communication](Communication.md)).
- **Logic gets time and input as parameters** (`Tick(deltaSeconds)`, a move request). It never reads a clock, a transform or the screen.
- **Views show and forward.** A view holds no rule; when it needs an answer ("can this block go there?") it asks the logic. A view changes its objects through its own methods; nothing sets its transforms from outside.
- **One flow per feature** in `Game` connects input, logic and views and owns the order of steps (build, play, result, next). It reads top to bottom.
- **Sound and haptics** are triggered from presentation, through the Core services, at the moment the player sees the event.
- **Restart** builds new logic from the level data; nothing from the previous attempt is reused.
- **Data before presentation.** What the player earns, spends or completes (coins, level progress) is written and saved on the action itself, before any animation starts; the animation only shows it, and a view never holds the value the game keeps. Never write data in a tween's completion: a game killed mid-animation must keep it.
- **Behavior that differs by kind** (a block type, a result type) lives with the kind: polymorphism or a strategy looked up once, never a switch over types.
- **No machinery ahead of need.** A type hierarchy for results, a registry or an animation pipeline comes when the second case exists, not before.

The shape of a move result and the motion layer follow the movement and motion decisions ([Decisions](../Game/Decisions.md)).

## Where presentation classes get their dependencies
A `*View` is a MonoBehaviour; everything else in presentation is plain C#.

| Kind | Gets its dependencies |
|---|---|
| View on a spawned prefab | `Init(...)` from whoever spawns it; not created through the container |
| View placed in a scene | `[Inject] Construct(...)`, registered by the scene scope with `RegisterComponent` |
| Plain C# (layout, spawner, input, flow) | Constructor |

UI screens follow Core.UI ([UI](../Base/Systems/UI.md)): the flow opens a screen and passes one named callback per button it reacts to.
