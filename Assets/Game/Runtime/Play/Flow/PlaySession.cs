using Game.Data.Levels;
using Game.Logic.Play;
using Game.Play.Presentation;
using Game.Play.Settings;
using UnityEngine;

namespace Game.Play.Flow
{
    // Entry of the play feature: builds a level and connects the pointer, the drag rule and the block views.
    public sealed class PlaySession
    {
        private readonly BoardBuilder _builder;
        private readonly BoardPointer _pointer;
        private readonly BoardArtSettings _art;
        private readonly BlockMotionSettings _motion;
        private LevelData _level;
        private Board _board;
        private BlockDrag _drag;
        private BoardLayout _layout;
        private BlockView[] _blocks;

        public PlaySession(BoardBuilder builder, BoardPointer pointer, BoardArtSettings art, BlockMotionSettings motion)
        {
            _builder = builder;
            _pointer = pointer;
            _art = art;
            _motion = motion;
        }

        public void Start(LevelData level)
        {
            _level = level;
            _board = new Board(level);
            _drag = new BlockDrag(_board);
            _layout = new BoardLayout(level.Width, level.Height, _art.CellSize);
            _blocks = _builder.Build(level, _layout);
            _pointer.Listen(Grab, Drag, Release);
        }

        private void Grab(Vector3 point)
        {
            var onBoard = _layout.WorldToBoard(point);
            var block = _board.BlockAt(new Cell(Mathf.FloorToInt(onBoard.x), Mathf.FloorToInt(onBoard.y)));
            if (block != Board.NoBlock)
                _drag.Begin(block, onBoard.x, onBoard.y);
        }

        private void Drag(Vector3 point)
        {
            if (!_drag.IsActive)
                return;
            var block = _drag.Block;
            var onBoard = _layout.WorldToBoard(point);
            var pose = _drag.MoveTo(onBoard.x, onBoard.y);
            if (pose.HasLeft)
                Leave(block, pose);
            else
                _blocks[block].Follow(_layout.PoseToWorld(pose));
        }

        private void Release()
        {
            if (!_drag.IsActive)
                return;
            var block = _drag.Block;
            _drag.End();
            _blocks[block].SnapTo(_layout.CellCenter(_board.Blocks[block].Position));
        }

        // The block snaps onto the cell in front of its door, holds for a moment, then slides out through it.
        private void Leave(int block, DragPose pose)
        {
            var doorCell = _layout.CellCenter(pose.Position);
            var outward = _layout.ToWorld(Direction.OutOf(_level.Doors[pose.ExitDoor].Side));
            _blocks[block].Leave(doorCell, doorCell + outward * (_motion.LeaveDistance * _layout.CellSize));
        }
    }
}
