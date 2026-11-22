using System;
using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;
using VContainer.Unity;

namespace Game.Play
{
    public sealed class PlaySession : ITickable
    {
        private readonly LevelStage _stage;
        private readonly PooledBoardPieces _pieces;
        private readonly BoardRoot _boardRoot;
        private readonly Camera _camera;
        private readonly BoardProjection _projection;
        private readonly BoardPresenter _board;
        private readonly BoardInput _input;
        private LevelAttempt _attempt;
        private bool _running;
        private Action _onWon;
        private Action _onFailed;
        private IBooster _booster;

        public float RemainingSeconds => _attempt == null ? 0f : _attempt.RemainingSeconds;

        public float FrozenSeconds => _attempt == null ? 0f : _attempt.FrozenSeconds;

        public bool IsDecided => _attempt != null && _attempt.Outcome != LevelOutcome.Playing;

        private bool IsBoardSettled => !_input.IsBusy && _board.IsSettled;

        public PlaySession(LevelStage stage, PooledBoardPieces pieces, BoardRoot boardRoot, Camera camera,
            BoardProjection projection, BoardPresenter board, BoardInput input)
        {
            _stage = stage;
            _pieces = pieces;
            _boardRoot = boardRoot;
            _camera = camera;
            _projection = projection;
            _board = board;
            _input = input;
        }

        public void Start(LevelData level, Action onWon, Action onFailed)
        {
            _booster?.Cancel();
            _attempt = new LevelAttempt(level);
            _running = true;
            _onWon = onWon;
            _onFailed = onFailed;
            ShowBoard(level);
        }

        public void Tick()
        {
            if (!_running)
                return;
            var outcome = _attempt.Tick(Time.deltaTime);
            if (outcome != LevelOutcome.Playing && !_attempt.IsPaused && IsBoardSettled)
                End(outcome);
        }

        public void Pause()
        {
            _attempt.Pause();
            _input.Release();
        }

        public void Resume() => _attempt.Resume();

        public bool IsAvailable(IBooster booster) => _running && booster.IsAvailable(_attempt);

        public void Use(IBooster booster, AimingOverlay overlay, BoosterShots shots)
        {
            if (_booster != null || !IsAvailable(booster))
                return;
            _booster = booster;
            booster.Use(new BoosterContext(_attempt, _input, _board, overlay, shots), BoosterDone);
        }

        private void ShowBoard(LevelData level)
        {
            _pieces.ReleaseAll();
            var staged = _stage.Show(level, _attempt, _boardRoot.transform, _pieces, _camera);
            _projection.Use(staged.Layout);
            _board.Show(_attempt, staged);
            _input.Begin(_attempt, staged.Layout);
        }

        private void BoosterDone() => _booster = null;

        private void End(LevelOutcome outcome)
        {
            _booster?.Cancel();
            _running = false;
            _input.Release();
            var ended = outcome == LevelOutcome.Won ? _onWon : _onFailed;
            _onWon = null;
            _onFailed = null;
            ended();
        }
    }
}
