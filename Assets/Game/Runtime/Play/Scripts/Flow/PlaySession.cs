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
        private readonly BoardRoot _board;
        private readonly Camera _camera;
        private readonly BoardInteraction _interaction;
        private LevelAttempt _attempt;
        private bool _running;
        private Action _onWon;
        private Action _onFailed;
        private IBooster _booster;

        public float RemainingSeconds => _attempt == null ? 0f : _attempt.RemainingSeconds;

        public bool IsDecided => _attempt != null && _attempt.Outcome != LevelOutcome.Playing;

        public PlaySession(LevelStage stage, PooledBoardPieces pieces, BoardRoot board, Camera camera, BoardInteraction interaction)
        {
            _stage = stage;
            _pieces = pieces;
            _board = board;
            _camera = camera;
            _interaction = interaction;
        }

        public void Start(LevelData level, Action onWon, Action onFailed)
        {
            _booster?.Cancel();
            _pieces.ReleaseAll();
            _attempt = new LevelAttempt(level, LevelMechanics.For(level));
            _running = true;
            _onWon = onWon;
            _onFailed = onFailed;
            var staged = _stage.Show(level, _board.transform, _pieces, _camera);
            _interaction.Begin(level, _attempt, staged);
        }

        public void Tick()
        {
            if (!_running)
                return;
            var outcome = _attempt.Tick(Time.deltaTime);
            if (outcome != LevelOutcome.Playing && !_attempt.IsPaused && _interaction.IsSettled)
                End(outcome);
        }

        public void Pause()
        {
            _attempt.Pause();
            _interaction.Release();
        }

        public void Resume() => _attempt.Resume();

        public bool IsAvailable(IBooster booster) => _running && booster.IsAvailable(_attempt);

        public void Use(IBooster booster, AimingOverlay overlay, BoosterShots shots)
        {
            if (_booster != null || !IsAvailable(booster))
                return;
            _booster = booster;
            booster.Use(new BoosterContext(_attempt, _interaction, overlay, shots), BoosterDone);
        }

        private void BoosterDone() => _booster = null;

        private void End(LevelOutcome outcome)
        {
            _booster?.Cancel();
            _running = false;
            _interaction.Release();
            var ended = outcome == LevelOutcome.Won ? _onWon : _onFailed;
            _onWon = null;
            _onFailed = null;
            ended();
        }
    }
}
