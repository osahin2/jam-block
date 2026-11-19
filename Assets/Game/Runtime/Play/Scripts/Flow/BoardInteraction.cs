using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    // Connects the board's input, the level attempt and the block views: a touch, projected onto the plane of the
    // blocks' tops, goes to the attempt in board cells; the pose that comes back goes to its block's view.
    public sealed class BoardInteraction
    {
        private readonly BoardInputView _input;
        private readonly TouchPlane _touchPlane;
        private LevelData _level;
        private LevelAttempt _attempt;
        private BoardLayout _layout;
        private BlockView[] _blocks;

        public BoardInteraction(BoardInputView input, TouchPlane touchPlane)
        {
            _input = input;
            _touchPlane = touchPlane;
        }

        // Fits the input's collider to the board with its frame, up to the blocks' tops, and listens to it.
        public void Begin(LevelData level, LevelAttempt attempt, BoardLayout layout, BlockView[] blocks)
        {
            _level = level;
            _attempt = attempt;
            _layout = layout;
            _blocks = blocks;
            _input.Cover(layout.Box);
            _input.Listen(Grab, Drag, Release);
        }

        // Puts the held block, if any, back onto its cell.
        public void Release()
        {
            var block = _attempt.Release();
            if (block != LevelAttempt.NoBlock)
                _blocks[block].SnapTo(_layout.CellCenter(_attempt.PositionOf(block)));
        }

        private void Grab(Vector2 screen)
        {
            if (!TryFindOnBoard(screen, out var onBoard))
                return;
            if (_attempt.Grab(onBoard.x, onBoard.y, out var pose) && pose.HasLeft)
                Leave(pose);
        }

        private void Drag(Vector2 screen)
        {
            if (!TryFindOnBoard(screen, out var onBoard) || !_attempt.MoveTo(onBoard.x, onBoard.y, out var pose))
                return;
            if (pose.HasLeft)
                Leave(pose);
            else
                _blocks[pose.Block].Follow(_layout.PoseToLocal(pose));
        }

        // In cells, as the attempt expects.
        private bool TryFindOnBoard(Vector2 screen, out Vector2 onBoard)
        {
            if (_touchPlane.TryProject(screen, out var point))
            {
                onBoard = _layout.LocalToBoard(point);
                return true;
            }
            onBoard = default;
            return false;
        }

        private void Leave(DragPose pose)
        {
            var outward = Direction.OutOf(_level.Doors[pose.ExitDoor].Side);
            _blocks[pose.Block].Leave(_layout.CellCenter(pose.Position), _layout.ToLocal(outward) * _layout.CellSize);
        }
    }
}
