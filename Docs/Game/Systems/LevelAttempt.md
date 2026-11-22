# Level attempt

> Summary: One try at a level in `Game.Logic`: the board, the drag and the timer behind one class that the play feature talks to, and the level's outcome (playing, won, failed). A restart is a new attempt.

## Code
- Folder: `Assets/Game/Logic/Play/Attempt/`
- Assembly: `Game.Logic`
- Entry points: `LevelAttempt` (`Logic/Play/Attempt/LevelAttempt.cs`); `LevelTimer` (`Logic/Play/Attempt/LevelTimer.cs`) and `LevelOutcome` are used through it

## How it works
- `new LevelAttempt(level)` builds the level's mechanics (`LevelMechanics.For(level)`, [Board](Board.md)) and a `Board` with their rules and guards, one pair per attempt, so no set of mechanics is ever shared by two boards; `Mechanics` gives the views each mechanic by name (`Ice`, `Arrow`). The board's constructor and the mechanics' rule and guard lists are internal to `Game.Logic` (the tests reach them through `InternalsVisibleTo`), so nothing outside the logic makes a board or reaches a guard's `TakeHit` ([Decisions](../Decisions.md)). It also builds a `BlockDrag` over the board and a `LevelTimer` with the level's time limit. Positions are in cells from the board's top-left corner, as in [Board](Board.md). The board stays inside the attempt, so no caller can step or break a block past its rules; `Board` gives it read-only (`IReadOnlyBoard`) for the views to catch up from, a block's cell among them.
- `Grab(x, y, out pose)`: a touch inside the board while not aiming starts the timer (the first one counts, later ones change nothing); when a block is under the finger the call returns true. A block that touches its door with its way out free leaves at once (`Board.TryLeave`), so a tap is enough, and the pose reports the door; when that was the last block, `Outcome` becomes `Won` before the call returns. A locked block (`Board.IsLocked`, an iced one) is not held: the call returns true with `DragPose.Locked`, which names the block and its cell, so the view can show the refusal ([Ice](Ice.md)). Any other block is held and the pose is `DragPose.None`. A touch outside the board does nothing.
- `MoveTo(x, y, out pose)`: moves the held block through the drag rule and returns its `DragPose`. When the move takes the last block out, `Outcome` becomes `Won` before the call returns. `Grab` and `MoveTo` set it through one private `UpdateWon`. Returns false, with `DragPose.None`, when nothing is held or the board takes no touches.
- `Release()`: ends the drag and returns the block that was held, or `LevelAttempt.NoBlock`, so the view can snap it back onto its cell without knowing the board.
- `Tick(deltaSeconds)`: counts the timer down while the level is playing and returns the outcome as it stands, not whether it changed; the tick that reaches zero sets `Failed`. The play session reads this value every frame, so a win set by a touch and a fail set by the timer reach it the same way ([Play](Play.md)).
- `Pause()` / `Resume()`: stop and restart the timer and the board together, for the pause and settings popups; `IsPaused` tells which, so the play session does not end a level while it is paused.
- `Freeze(seconds)`: the Freeze booster's stop of the time. Only when `CanFreeze` (boosters can be used and the time is not frozen already, `IsFrozen`), returning true; the board keeps taking touches.
- `CellAt(x, y)` (static): the cell under a board position in cells, the rounding every tap goes through; the targeted booster uses it to aim its shot at the cell the effect hit.

## Effects
A booster's effect on the board is an `IBoardEffect` (`Logic/Play/Effects/`): `CanTarget(board, cell)` says from the read-only board whether a cell is a target, `Apply(board, cell)` gets the board as `IBoard` and changes it only through `Hit` (and the rocket's `HitCell`), which give a guarded block's hit to its guard ([Board](Board.md)). Contract: `CanTarget` changes nothing; `Apply` is called only on a cell `CanTarget` accepted, inside the board.
- `CanUseBoosters`: the level is playing, not paused, and the first touch has started the timer.
- `Aim()`: when boosters can be used, ends the drag and turns taps into picks (`IsAiming`), returning true; the timer keeps running. A held block's view must be put back first (`BoardInput.Release`), since the attempt no longer returns that block once the drag has ended. `Grab` takes nothing while aiming.
- `CancelAim()`: back to the drag.
- `TryApply(effect, x, y)`: the one way an effect reaches the board. Only while aiming and taking touches, on a cell inside the board that the effect can target: it applies the effect, sets `Won` when that took the last block (before the call returns, as a touch does) and ends the aim, returning true. Otherwise it changes nothing and the aim goes on; cancelling is the caller's choice.

## Timer
`LevelTimer` counts down from the time limit. It runs once started, unless paused or expired; `Tick` returns true only on the tick that reaches zero, and the remaining time stops at 0. Pausing before the first touch keeps it stopped after resume. `Freeze(seconds)` holds the count for that long: each tick first uses up the frozen seconds and counts only the rest; a paused or not yet started timer uses up nothing, so a freeze lasts its seconds of play. The HUD rounds the remaining seconds up, so 00:00 shows only when the level fails.

## Rules & invariants
- The board takes touches only while the outcome is `Playing` and the attempt is not paused. Release is always allowed, so a block held when the time runs out or a popup opens can be put back.
- The outcome is set once: a won level cannot fail on a later tick, and a failed one takes no more moves.
- Nothing is reused between attempts: restart and retry build a new `LevelAttempt` from the level data.
- `Grab`, `MoveTo`, `Release` and `Tick` do not allocate.

## Tests
- `Tests/Play/Attempt/LevelTimerTests`: no count before start or while paused, resume, start while paused, stop at zero, expiry reported once; no count while frozen, only the time after the freeze counted, a freeze not used up while paused or before the start.
- `Tests/Play/Attempt/LevelAttemptTests`: first touch starts the timer (on a block, on an empty cell, not outside the board), held block and its pose, no pose without a held block, a block out with others left, win on the last block with the timer stopped, the win known before the next tick, a tap that sends out a block at its door (and wins on the last one) or holds one whose way out is blocked, a grab on a locked block that holds nothing and names it in a locked pose, fail at zero, every tick after a fail returning it again, a held block after fail, no touches after the level is over, pause and resume, `IsPaused` following them.
- `Tests/Play/Attempt/LevelAttemptEffectTests` (with the test effect `BreakTargetEffect`): boosters off before the first touch and after the level is decided, no effect without aiming, an applied effect ends the aim, a cell that is no target or outside the board keeps the aim, an effect that takes the last block wins, aiming releases the held block and stops grabs, cancelling turns the drag back on, the time runs while aiming, nothing applied after a fail or while paused; a freeze stops the time for its seconds and is refused before the first touch, while frozen and after a fail.

## Related
- [Board](Board.md), [Play](Play.md), [Reference](../Reference.md), [Decisions](../Decisions.md)
