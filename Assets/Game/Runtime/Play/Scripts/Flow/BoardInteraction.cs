using System;
using System.Collections.Generic;
using DG.Tweening;
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
        private readonly BlockViewBuilder _blockViews;
        private readonly PooledBoardPieces _pieces;
        private readonly BoardRoot _board;
        private readonly List<BlockView> _blocks = new List<BlockView>();
        private LevelData _level;
        private LevelAttempt _attempt;
        private BoardLayout _layout;
        private DoorView[] _doors;
        private Action<Vector2> _onPicked;
        private bool _busy;

        public bool IsSettled
        {
            get
            {
                if (_busy)
                    return false;
                foreach (var block in _blocks)
                {
                    if (block.IsLeaving)
                        return false;
                }
                return true;
            }
        }

        public BoardInteraction(BoardInputView input, TouchPlane touchPlane, BlockMotionSettings motion,
            BlockOutlineMaterials outline, BlockViewBuilder blockViews, PooledBoardPieces pieces, BoardRoot board)
        {
            _input = input;
            _touchPlane = touchPlane;
            _motion = motion;
            _outline = outline;
            _blockViews = blockViews;
            _pieces = pieces;
            _board = board;
        }

        public void Begin(LevelData level, LevelAttempt attempt, StagedLevel staged)
        {
            _level = level;
            _attempt = attempt;
            _layout = staged.Layout;
            _blocks.Clear();
            _blocks.AddRange(staged.Blocks);
            _doors = staged.Doors;
            foreach (var block in _blocks)
                block.UseOutline(_outline);
            _input.Cover(_layout.Box);
            EnterDrag();
        }

        public void EnterDrag()
        {
            _busy = false;
            _onPicked = null;
            _input.Listen(Grab, Drag, Release);
        }

        public void EnterPick(Action<Vector2> onPicked)
        {
            _busy = false;
            _onPicked = onPicked;
            _input.Listen(Pick, Ignore, Ignore);
        }

        public void EnterBusy()
        {
            _busy = true;
            _onPicked = null;
            _input.Listen(Ignore, Ignore, Ignore);
        }

        public Rect ScreenRect()
        {
            var box = _layout.Box;
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            for (var corner = 0; corner < 8; corner++)
            {
                var local = new Vector3(
                    (corner & 1) == 0 ? box.min.x : box.max.x,
                    (corner & 2) == 0 ? box.min.y : box.max.y,
                    (corner & 4) == 0 ? box.min.z : box.max.z);
                var screen = _touchPlane.ToScreen(local);
                min = Vector2.Min(min, screen);
                max = Vector2.Max(max, screen);
            }
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        public Vector2 ScreenPointOf(Cell cell, float lift) => _touchPlane.ToScreen(Lifted(cell, lift));

        public void VanishGone(Cell toward, float lift, float duration, float stagger, Ease ease)
        {
            var target = Lifted(toward, lift);
            var blocks = _attempt.Board.Blocks;
            var delay = 0f;
            for (var i = 0; i < _blocks.Count; i++)
            {
                if (blocks[i].IsGone && _blocks[i].TryVanish(target, duration, delay, ease))
                    delay += stagger;
            }
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
            {
                CatchUpShape(i, blocks[i]);
                _blocks[i].CatchUp(blocks[i]);
            }
        }

        private void CatchUpShape(int index, BoardBlock block)
        {
            if (index < _blocks.Count && _blocks[index].CellCount == block.Shape.Count)
                return;
            if (index == _blocks.Count)
                _blocks.Add(_blockViews.Build(block.ColorId, block.Shape, _layout.CellCenter(block.Position), _board.transform, _pieces));
            else
                _blockViews.Rebuild(_blocks[index], block, _pieces);
            _blocks[index].UseOutline(_outline);
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

        private void Pick(Vector2 screen)
        {
            if (TryFindOnBoard(screen, out var onBoard))
                _onPicked(onBoard);
        }

        private static void Ignore(Vector2 screen)
        {
        }

        private static void Ignore()
        {
        }

        private Vector3 Lifted(Cell cell, float lift) => _layout.CellCenter(cell) + Vector3.up * (lift * _layout.CellSize);

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
