using System;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    public sealed class BoardInput
    {
        private readonly BoardInputView _input;
        private readonly BoardProjection _projection;
        private readonly BoardPresenter _board;
        private LevelAttempt _attempt;
        private Action<Vector2> _onPicked;

        public bool IsBusy { get; private set; }

        public BoardInput(BoardInputView input, BoardProjection projection, BoardPresenter board)
        {
            _input = input;
            _projection = projection;
            _board = board;
        }

        public void Begin(LevelAttempt attempt, BoardLayout layout)
        {
            _attempt = attempt;
            _input.Cover(layout.Box);
            EnterDrag();
        }

        public void EnterDrag()
        {
            IsBusy = false;
            _onPicked = null;
            _input.Listen(Grab, Drag, Release);
        }

        public void EnterPick(Action<Vector2> onPicked)
        {
            IsBusy = false;
            _onPicked = onPicked;
            _input.Listen(Pick, Ignore, Ignore);
        }

        public void EnterBusy()
        {
            IsBusy = true;
            _onPicked = null;
            _input.Listen(Ignore, Ignore, Ignore);
        }

        public void Release()
        {
            var block = _attempt.Release();
            if (block != LevelAttempt.NoBlock)
                _board.SnapBack(block);
        }

        private void Grab(Vector2 screen)
        {
            if (_projection.TryFindOnBoard(screen, out var onBoard) && _attempt.Grab(onBoard.x, onBoard.y, out var pose))
                ShowGrabbed(pose);
        }

        private void ShowGrabbed(DragPose pose)
        {
            if (pose.HasLeft)
                _board.ShowLeaving(pose);
            else if (pose.IsLocked)
                _board.Refuse(pose.Block);
            else
                _board.Hold(_attempt.HeldBlock);
        }

        private void Drag(Vector2 screen)
        {
            if (!_projection.TryFindOnBoard(screen, out var onBoard) || !_attempt.MoveTo(onBoard.x, onBoard.y, out var pose))
                return;
            if (pose.HasLeft)
                _board.ShowLeaving(pose);
            else
                _board.Follow(pose);
        }

        private void Pick(Vector2 screen)
        {
            if (_projection.TryFindOnBoard(screen, out var onBoard))
                _onPicked(onBoard);
        }

        private static void Ignore(Vector2 screen)
        {
        }

        private static void Ignore()
        {
        }
    }
}
