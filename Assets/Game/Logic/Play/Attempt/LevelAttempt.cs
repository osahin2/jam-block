using System;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class LevelAttempt
    {
        public const int NoBlock = Play.Board.NoBlock;

        private readonly Board _board;
        private readonly BlockDrag _drag;
        private readonly LevelTimer _timer;

        public LevelOutcome Outcome { get; private set; }
        public float RemainingSeconds => _timer.RemainingSeconds;
        public bool IsPaused => _timer.IsPaused;
        public bool IsAiming { get; private set; }
        public bool CanUseBoosters => TakesTouches && _timer.HasStarted;
        public int HeldBlock => _drag.Block;
        public IReadOnlyBoard Board => _board;
        private bool TakesTouches => Outcome == LevelOutcome.Playing && !IsPaused;

        public LevelAttempt(LevelData level, LevelMechanics mechanics)
        {
            _board = new Board(level, mechanics.Rules, mechanics.Guards);
            _drag = new BlockDrag(_board);
            _timer = new LevelTimer(level.TimeLimitSeconds);
        }

        public bool Grab(float x, float y, out DragPose pose)
        {
            pose = DragPose.None;
            var cell = CellAt(x, y);
            if (!TakesTouches || IsAiming || !_board.Contains(cell))
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

        public bool MoveTo(float x, float y, out DragPose pose)
        {
            pose = DragPose.None;
            if (!_drag.IsActive || !TakesTouches)
                return false;
            pose = _drag.MoveTo(x, y);
            UpdateWon();
            return true;
        }

        public int Release()
        {
            var block = _drag.Block;
            _drag.End();
            return block;
        }

        public bool Aim()
        {
            if (!CanUseBoosters)
                return false;
            _drag.End();
            IsAiming = true;
            return true;
        }

        public void CancelAim() => IsAiming = false;

        public bool TryApply(IBoardEffect effect, float x, float y)
        {
            var cell = CellAt(x, y);
            if (!IsAiming || !TakesTouches || !_board.Contains(cell) || !effect.CanTarget(_board, cell))
                return false;
            effect.Apply(_board, cell);
            UpdateWon();
            IsAiming = false;
            return true;
        }

        public LevelOutcome Tick(float deltaSeconds)
        {
            if (Outcome == LevelOutcome.Playing && _timer.Tick(deltaSeconds))
                Outcome = LevelOutcome.Failed;
            return Outcome;
        }

        public void Pause() => _timer.Pause();

        public void Resume() => _timer.Resume();

        private static Cell CellAt(float x, float y) => new Cell((int)Math.Floor(x), (int)Math.Floor(y));

        private void UpdateWon()
        {
            if (_board.IsCleared)
                Outcome = LevelOutcome.Won;
        }
    }
}
