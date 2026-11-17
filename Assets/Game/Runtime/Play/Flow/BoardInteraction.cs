using Game.Data.Levels;
using Game.Logic.Play;
using Game.Play.Presentation;
using UnityEngine;

namespace Game.Play.Flow
{
    // Connects the pointer, the level attempt and the block views: a touch goes to the attempt in board cells, the
    // pose that comes back goes to the held block's view.
    public sealed class BoardInteraction
    {
        private readonly BoardPointer _pointer;
        private LevelData _level;
        private LevelAttempt _attempt;
        private BoardLayout _layout;
        private BlockView[] _blocks;

        public BoardInteraction(BoardPointer pointer)
        {
            _pointer = pointer;
        }

        public void Begin(LevelData level, LevelAttempt attempt, BoardLayout layout, BlockView[] blocks)
        {
            _level = level;
            _attempt = attempt;
            _layout = layout;
            _blocks = blocks;
            _pointer.Listen(Grab, Drag, Release);
        }

        // Puts the held block, if any, back onto its cell.
        public void Release()
        {
            var block = _attempt.Release();
            if (block != Board.NoBlock)
                _blocks[block].SnapTo(_layout.CellCenter(_attempt.PositionOf(block)));
        }

        private void Grab(Vector3 point)
        {
            var onBoard = _layout.WorldToBoard(point);
            _attempt.Grab(onBoard.x, onBoard.y);
        }

        private void Drag(Vector3 point)
        {
            var block = _attempt.HeldBlock;
            var onBoard = _layout.WorldToBoard(point);
            if (!_attempt.MoveTo(onBoard.x, onBoard.y, out var pose))
                return;
            if (pose.HasLeft)
                Leave(block, pose);
            else
                _blocks[block].Follow(_layout.PoseToWorld(pose));
        }

        private void Leave(int block, DragPose pose)
        {
            var outward = Direction.OutOf(_level.Doors[pose.ExitDoor].Side);
            _blocks[block].Leave(_layout.CellCenter(pose.Position), _layout.ToWorld(outward) * _layout.CellSize);
        }
    }
}
