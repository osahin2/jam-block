using System.Collections.Generic;
using DG.Tweening;
using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    public sealed class BoardPresenter
    {
        private readonly BlockMotionSettings _motion;
        private readonly BlockOutlineMaterials _outline;
        private readonly BlockViewBuilder _blockViews;
        private readonly PooledBoardPieces _pieces;
        private readonly BoardRoot _board;
        private readonly List<BlockView> _blocks = new List<BlockView>();
        private LevelAttempt _attempt;
        private BoardLayout _layout;
        private DoorView[] _doors;

        public bool IsSettled => !AnyBlockLeaving();
        private float FrameInCells => _layout.FrameWidth / _layout.CellSize;

        public BoardPresenter(BlockMotionSettings motion, BlockOutlineMaterials outline, BlockViewBuilder blockViews,
            PooledBoardPieces pieces, BoardRoot board)
        {
            _motion = motion;
            _outline = outline;
            _blockViews = blockViews;
            _pieces = pieces;
            _board = board;
        }

        public void Show(LevelAttempt attempt, StagedLevel staged)
        {
            _attempt = attempt;
            _layout = staged.Layout;
            _doors = staged.Doors;
            _blocks.Clear();
            _blocks.AddRange(staged.Blocks);
            foreach (var block in _blocks)
                block.UseOutline(_outline);
        }

        public void Hold(int block) => _blocks[block].Hold();

        public void Refuse(int block) => _blocks[block].Refuse();

        public void Follow(DragPose pose) => _blocks[pose.Block].Follow(_layout.PoseToLocal(pose));

        public void SnapBack(int block) =>
            _blocks[block].SnapTo(_layout.CellCenter(_attempt.Board.Blocks[block].Position));

        public void ShowLeaving(DragPose pose)
        {
            var door = _doors[pose.ExitDoor];
            var cellsToClear = CellsToClearDoor(pose.Block, door.Side);
            SlideOut(pose, door.Side, cellsToClear);
            OpenDoor(door, cellsToClear);
            CatchUp();
        }

        public void VanishGone(Cell toward, float lift, float duration, float stagger, Ease ease)
        {
            var target = _layout.AboveCell(toward, lift);
            var blocks = _attempt.Board.Blocks;
            var delay = 0f;
            for (var i = 0; i < _blocks.Count; i++)
            {
                if (blocks[i].IsGone && _blocks[i].TryVanish(target, duration, delay, ease))
                    delay += stagger;
            }
        }

        public bool TryFindColorShownAt(Cell cell, out Color color)
        {
            var blocks = _attempt.Board.Blocks;
            for (var i = 0; i < _blocks.Count; i++)
            {
                if (!_blocks[i].Shows(cell - blocks[i].Position))
                    continue;
                color = _blocks[i].Color;
                return true;
            }
            color = default;
            return false;
        }

        public void CatchUp()
        {
            for (var i = 0; i < _attempt.Board.Blocks.Count; i++)
                CatchUpBlock(BlockOnBoard.Of(_attempt, i));
        }

        private bool AnyBlockLeaving()
        {
            foreach (var block in _blocks)
            {
                if (block.IsLeaving)
                    return true;
            }
            return false;
        }

        private float CellsToClearDoor(int block, Side side) =>
            _attempt.Board.Blocks[block].ExtentAcross(side) + FrameInCells;

        private void SlideOut(DragPose pose, Side side, float cells)
        {
            var doorCell = _layout.CellCenter(pose.Position);
            var outside = doorCell + _layout.ToLocal(Direction.OutOf(side)) * (cells * _layout.CellSize);
            _blocks[pose.Block].Leave(doorCell, outside, _motion.SlideDuration(cells));
        }

        private void OpenDoor(DoorView door, float cells)
        {
            var snap = _motion.SnapDuration;
            door.LetThrough(snap + _motion.SlideDuration(FrameInCells), snap + _motion.SlideDuration(cells));
        }

        private void CatchUpBlock(BlockOnBoard block)
        {
            CatchUpShape(block);
            var view = _blocks[block.Index];
            view.CatchUp(block.Block);
            _blockViews.CatchUpFeatures(view, block);
        }

        private void CatchUpShape(BlockOnBoard block)
        {
            if (block.Index == _blocks.Count)
                AddView(block);
            else if (_blocks[block.Index].CellCount != block.Block.Shape.Count)
                RebuildView(block);
        }

        private void AddView(BlockOnBoard block)
        {
            var pose = _layout.CellCenter(block.Block.Position);
            var view = _blockViews.Build(block, pose, _board.transform, _pieces);
            view.UseOutline(_outline);
            _blocks.Add(view);
        }

        private void RebuildView(BlockOnBoard block)
        {
            var view = _blocks[block.Index];
            _blockViews.Rebuild(view, block, _pieces);
            view.UseOutline(_outline);
        }
    }
}
