using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    public sealed class BoardInteraction
    {
        private readonly BoardInputView _input;
        private readonly TouchPlane _touchPlane;
        private readonly BlockMotionSettings _motion;
        private readonly BlockOutlineMaterials _outline;
        private LevelData _level;
        private LevelAttempt _attempt;
        private BoardLayout _layout;
        private BlockView[] _blocks;
        private DoorView[] _doors;

        public bool IsSettled
        {
            get
            {
                foreach (var block in _blocks)
                {
                    if (block.IsLeaving)
                        return false;
                }
                return true;
            }
        }

        public BoardInteraction(BoardInputView input, TouchPlane touchPlane, BlockMotionSettings motion,
            BlockOutlineMaterials outline)
        {
            _input = input;
            _touchPlane = touchPlane;
            _motion = motion;
            _outline = outline;
        }

        public void Begin(LevelData level, LevelAttempt attempt, StagedLevel staged)
        {
            _level = level;
            _attempt = attempt;
            _layout = staged.Layout;
            _blocks = staged.Blocks;
            _doors = staged.Doors;
            foreach (var block in _blocks)
                block.UseOutline(_outline);
            _input.Cover(_layout.Box);
            _input.Listen(Grab, Drag, Release);
        }

        public void Release()
        {
            var block = _attempt.Release();
            if (block != LevelAttempt.NoBlock)
                _blocks[block].SnapTo(_layout.CellCenter(_attempt.Board.Blocks[block].Position));
        }

        public void CatchUp()
        {
            var blocks = _attempt.Board.Blocks;
            for (var i = 0; i < blocks.Count; i++)
                _blocks[i].CatchUp(blocks[i]);
        }

        private void Grab(Vector2 screen)
        {
            if (!TryFindOnBoard(screen, out var onBoard))
                return;
            if (!_attempt.Grab(onBoard.x, onBoard.y, out var pose))
                return;
            if (pose.HasLeft)
                Leave(pose);
            else
                _blocks[_attempt.HeldBlock].Hold();
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
            var side = _level.Doors[pose.ExitDoor].Side;
            var frame = _layout.FrameWidth / _layout.CellSize;
            var length = _attempt.Board.Blocks[pose.Block].ExtentAcross(side) + frame;
            var slide = _motion.SlideDuration(length);
            var doorCell = _layout.CellCenter(pose.Position);
            var outside = doorCell + _layout.ToLocal(Direction.OutOf(side)) * (length * _layout.CellSize);
            _blocks[pose.Block].Leave(doorCell, outside, slide);
            var snap = _motion.SnapDuration;
            _doors[pose.ExitDoor].LetThrough(snap + _motion.SlideDuration(frame), snap + slide);
            CatchUp();
        }
    }
}
