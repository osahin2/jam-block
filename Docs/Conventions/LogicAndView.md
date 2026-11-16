# Logic and view

> Summary: Rules in `Game.Logic` are plain C# that return what happened; views in `Game` only show it and forward input. One presentation class per outcome type turns a rule's result into view calls.

## The flow of one player action
```
input (view)  ->  flow  ->  rules.Resolve(action)  ->  result: ordered list of outcomes
                    |
                    v
          presentation: one player per outcome type  ->  view calls (one animation)
```

- **Rules** (`Game.Logic`) have no engine reference, so they run in EditMode tests without a scene. An operation returns its outcome; rules never raise events. When one action causes a chain (a block slides, leaves through a door, the level is won), the result is the ordered list of outcomes, because that order is what the player watches.
- **The flow** (`Game`) is one orchestrator that owns the phases (build, accept input, resolve, play the result, show the result screen, go next). It reacts to input, the end of an animation, the timer and screen buttons.
- **Presentation** has one player class per outcome type, looked up by type from a registry built once (Strategy, no switch over types). A player turns its outcome into view calls and returns its piece of the animation; the flow plays the whole result. Adding an outcome type is one new player and one registration line; a test fails if an outcome type has no player.
- **Views** only show state and forward input. They hold no rules and decide nothing.

There are no presenter classes: the players are the bridge from rules to views.

## Example
```csharp
// Game.Logic: plain C#, returns what happened.
public sealed class Board
{
    public MoveResult Move(BlockId block, Direction direction)   // Rejected, or ordered outcomes
}

public abstract class Outcome { }
public sealed class BlockSlid : Outcome { public BlockId Block; public Cell From, To; }
public sealed class BlockExited : Outcome { public BlockId Block; public DoorId Door; }
public sealed class LevelWon : Outcome { }

// Game: one player per outcome type, found by type in a dictionary built once.
public sealed class BlockSlidPlayer : OutcomePlayer<BlockSlid>
{
    private readonly BoardView _board;

    public BlockSlidPlayer(BoardView board) => _board = board;

    protected override Animation Build(BlockSlid slid) => _board.BlockOf(slid.Block).SlideTo(slid.To);
}
```
`Animation` stands for whatever the motion layer returns; it is chosen with the tweening decision (`Docs/Game/Decisions.md`).

## Kinds of presentation classes
A `*View` is always a MonoBehaviour. Everything else in presentation is plain C#.

| Kind | What it is | Gets its dependencies |
|---|---|---|
| Prefab view | MonoBehaviour on a spawned prefab: its own look and motion | `Init(..., settings)` from its spawner; not created through the container |
| Composite view | MonoBehaviour in the scene that builds, keeps and clears its child views | `[Inject] Construct(...)`, registered by the scene scope with `RegisterComponent` |
| Layout | Plain C#: cell to position, board fit to screen | Constructor (settings) |
| Spawner | Plain C#: instantiates (or pools) one prefab and calls its `Init` | Constructor (settings) |
| Input | Plain C#: reads the pointer, maps a drag to a block and a direction | Constructor |
| Outcome players | Plain C#: outcome to view calls | Constructor (views, settings) |

UI screens follow Core.UI ([UI](../Base/Systems/UI.md)): the flow opens a screen and passes one named callback per button the flow reacts to.

## Rules of thumb
- If a class needs `UnityEngine`, it is not a rule; the compiler enforces this for `Game.Logic`.
- If a view starts deciding something ("can this block move?"), the decision belongs in the rules and the view gets the answer.
- A move goes through a view method, never by setting a transform from outside, so the view keeps its own state right.
- Sound and haptics are called from the players, through the Core services, at the point in the animation they belong to.
- Restart builds a new rules object from the level data; nothing from the previous attempt is reused.
