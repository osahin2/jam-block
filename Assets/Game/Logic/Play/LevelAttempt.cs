using System;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    // One try at a level: the board, the drag and the timer, and whether the level is won or failed. The first touch
    // on the board starts the timer; the board takes touches only while the level is playing and not paused.
    // Positions are in cells from the board's top-left corner. A restart is a new attempt.
    public sealed class LevelAttempt
    {
        private readonly Board _board;
        private readonly BlockDrag _drag;
        private readonly LevelTimer _timer;

        public LevelOutcome Outcome { get; private set; }
        public float RemainingSeconds => _timer.RemainingSeconds;
        public int HeldBlock => _drag.Block;
        private bool TakesTouches => Outcome == LevelOutcome.Playing && !_timer.IsPaused;

        public LevelAttempt(LevelData level)
        {
            _board = new Board(level);
            _drag = new BlockDrag(_board);
            _timer = new LevelTimer(level.TimeLimitSeconds);
        }

        // Returns true when a block is under the finger and is now held.
        public bool Grab(float x, float y)
        {
            if (!TakesTouches || x < 0f || y < 0f || x >= _board.Width || y >= _board.Height)
                return false;
            _timer.Start();
            var block = _board.BlockAt(new Cell((int)Math.Floor(x), (int)Math.Floor(y)));
            if (block == Board.NoBlock)
                return false;
            _drag.Begin(block, x, y);
            return true;
        }

        // Returns false when no block is held or the board takes no touches; the held block then stays where it is.
        public bool MoveTo(float x, float y, out DragPose pose)
        {
            pose = default;
            if (!_drag.IsActive || !TakesTouches)
                return false;
            pose = _drag.MoveTo(x, y);
            if (_board.IsCleared)
                Outcome = LevelOutcome.Won;
            return true;
        }

        // Returns the block that was held, or Board.NoBlock. Always allowed, so a held block can be put back after the
        // level fails or pauses.
        public int Release()
        {
            var block = _drag.Block;
            _drag.End();
            return block;
        }

        public Cell PositionOf(int block) => _board.Blocks[block].Position;

        public LevelOutcome Tick(float deltaSeconds)
        {
            if (Outcome == LevelOutcome.Playing && _timer.Tick(deltaSeconds))
                Outcome = LevelOutcome.Failed;
            return Outcome;
        }

        public void Pause() => _timer.Pause();

        public void Resume() => _timer.Resume();
    }
}
