# Level attempt

> Summary: One try at a level in `Game.Logic`: the board, the drag and the timer behind one class that the play feature talks to, and the level's outcome (playing, won, failed). A restart is a new attempt.

## Code
- Folder: `Assets/Game/Logic/Play/Attempt/`
- Assembly: `Game.Logic`
- Entry points: `LevelAttempt` (`Logic/Play/Attempt/LevelAttempt.cs`); `LevelTimer` (`Logic/Play/Attempt/LevelTimer.cs`) and `LevelOutcome` are used through it

## How it works
- `new LevelAttempt(level)` builds a `Board`, a `BlockDrag` over it and a `LevelTimer` with the level's time limit. Positions are in cells from the board's top-left corner, as in [Board](Board.md). The board stays inside the attempt, so no caller can step a block past its rules; `PositionOf(block)` gives a block's cell.
- `Grab(x, y, out pose)`: a touch inside the board starts the timer (the first one counts, later ones change nothing); when a block is under the finger the call returns true. A block that touches its door with its way out free leaves at once (`Board.TryLeave`), so a tap is enough, and the pose reports the door; when that was the last block, `Outcome` becomes `Won` before the call returns. Any other block is held and the pose is `DragPose.None`. A touch outside the board does nothing.
- `MoveTo(x, y, out pose)`: moves the held block through the drag rule and returns its `DragPose`. When the move takes the last block out, `Outcome` becomes `Won` before the call returns. `Grab` and `MoveTo` set it through one private `UpdateWon`. Returns false, with `DragPose.None`, when nothing is held or the board takes no touches.
- `Release()`: ends the drag and returns the block that was held, or `LevelAttempt.NoBlock`, so the view can snap it back onto its cell without knowing the board.
- `Tick(deltaSeconds)`: counts the timer down while the level is playing and returns the outcome as it stands, not whether it changed; the tick that reaches zero sets `Failed`. The play session reads this value every frame, so a win set by a touch and a fail set by the timer reach it the same way ([Play](Play.md)).
- `Pause()` / `Resume()`: stop and restart the timer and the board together, for the pause and settings popups; `IsPaused` tells which, so the play session does not end a level while it is paused.

## Timer
`LevelTimer` counts down from the time limit. It runs once started, unless paused or expired; `Tick` returns true only on the tick that reaches zero, and the remaining time stops at 0. Pausing before the first touch keeps it stopped after resume. The HUD rounds the remaining seconds up, so 00:00 shows only when the level fails.

## Rules & invariants
- The board takes touches only while the outcome is `Playing` and the attempt is not paused. Release is always allowed, so a block held when the time runs out or a popup opens can be put back.
- The outcome is set once: a won level cannot fail on a later tick, and a failed one takes no more moves.
- Nothing is reused between attempts: restart and retry build a new `LevelAttempt` from the level data.
- `Grab`, `MoveTo`, `Release` and `Tick` do not allocate.

## Tests
- `Tests/Play/Attempt/LevelTimerTests`: no count before start or while paused, resume, start while paused, stop at zero, expiry reported once.
- `Tests/Play/Attempt/LevelAttemptTests`: first touch starts the timer (on a block, on an empty cell, not outside the board), held block and its pose, no pose without a held block, a block out with others left, win on the last block with the timer stopped, the win known before the next tick, a tap that sends out a block at its door (and wins on the last one) or holds one whose way out is blocked, fail at zero, every tick after a fail returning it again, a held block after fail, no touches after the level is over, pause and resume, `IsPaused` following them.

## Related
- [Board](Board.md), [Play](Play.md), [Reference](../Reference.md), [Decisions](../Decisions.md)
