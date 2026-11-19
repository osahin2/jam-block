using System;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    // One try at a level: the board, the drag and the timer, and whether the level is won or failed. The first touch
    // on the board starts the timer; the board takes touches only while the level is playing and not paused.
    // Positions are in cells from the board's top-left corner. A restart is a new attempt.
    public sealed class LevelAttempt
    {
        public const int NoBlock = Board.NoBlock;

        private readonly Board _board;
        private readonly BlockDrag _drag;
        private readonly LevelTimer _timer;

        public LevelOutcome Outcome { get; private set; }
        public float RemainingSeconds => _timer.RemainingSeconds;
        public bool IsPaused => _timer.IsPaused;
        private bool TakesTouches => Outcome == LevelOutcome.Playing && !IsPaused;

        public LevelAttempt(LevelData level)
        {
            _board = new Board(level);
            _drag = new BlockDrag(_board);
            _timer = new LevelTimer(level.TimeLimitSeconds);
        }

        // Returns true when a block is under the finger. A block that touches its door with its way out free leaves at
        // once, as a tap does in the reference, and the pose says so; any other block is now held and the pose is
        // DragPose.None.
        public bool Grab(float x, float y, out DragPose pose)
        {
            pose = DragPose.None;
            var cell = new Cell((int)Math.Floor(x), (int)Math.Floor(y));
            if (!TakesTouches || !_board.Contains(cell))
                return false;
            _timer.Start();
            var block = _board.BlockAt(cell);
            if (block == NoBlock)
                return false;
            if (!_board.TryLeave(block, out var exitDoor))
            {
                _drag.Begin(block, x, y);
                return true;
            }
            pose = DragPose.Left(block, _board.Blocks[block].Position, exitDoor);
            UpdateWon();
            return true;
        }

        // Returns false when no block is held or the board takes no touches; the held block then stays where it is.
        public bool MoveTo(float x, float y, out DragPose pose)
        {
            pose = DragPose.None;
            if (!_drag.IsActive || !TakesTouches)
                return false;
            pose = _drag.MoveTo(x, y);
            UpdateWon();
            return true;
        }

        // Returns the block that was held, or NoBlock. Always allowed, so a held block can be put back after the
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

        private void UpdateWon()
        {
            if (_board.IsCleared)
                Outcome = LevelOutcome.Won;
        }
    }
}
