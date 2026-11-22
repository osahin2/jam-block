# Level attempt

> One try at a level: board, drag and timer behind one class the play feature talks to, and its outcome (playing, won, failed). A restart is a new attempt.

**Code:** `Logic/Play/Attempt/`, `Logic/Play/Effects/`. **Entry:** `LevelAttempt` (`Grab`, `MoveTo`, `Release`, `Tick`, `Pause`/`Resume`, `Freeze`, `Aim`/`CancelAim`, `TryApply`).

## Rules
- The attempt builds its board and mechanics together, one pair per attempt; the board's constructor and the guard lists are internal to `Game.Logic`, so nothing outside reaches a guard. Views read the board as `IReadOnlyBoard`.
- The first touch inside the board starts the timer. A won outcome is set before `Grab`, `MoveTo` or `TryApply` returns; `Tick` returns the outcome as it stands, so the session sees a win and a fail the same way. The outcome is set once.
- The board takes touches only while playing and not paused; release is always allowed.
- `IBoardEffect`: `CanTarget` changes nothing; `Apply` is called only on a cell `CanTarget` accepted and changes the board only through `Hit`/`HitCell`. `TryApply` is the one way an effect reaches the board.
- Boosters work only after the first touch, while playing and not paused. Aiming ends the drag, so the view must put a held block back first; the time keeps running.
- `LevelTimer.Freeze` holds the count for that many seconds of play; a paused or unstarted timer uses none of it.
- `Grab`, `MoveTo`, `Release` and `Tick` do not allocate.

Related: [Board](Board.md), [Play](Play.md), [Boosters](Boosters.md)
