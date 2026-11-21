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

        public float RemainingSeconds => _attempt == null ? 0f : _attempt.RemainingSeconds;

        // Also true while the last block slides out before the callback; the level may then only end.
        public bool IsDecided => _attempt != null && _attempt.Outcome != LevelOutcome.Playing;

        public PlaySession(LevelStage stage, PooledBoardPieces pieces, BoardRoot board, Camera camera, BoardInteraction interaction)
        {
            _stage = stage;
            _pieces = pieces;
            _board = board;
            _camera = camera;
            _interaction = interaction;
        }

        // The callback may start the next level; a start while a level plays abandons it without a callback.
        public void Start(LevelData level, Action onWon, Action onFailed)
        {
            _pieces.ReleaseAll();
            _attempt = new LevelAttempt(level);
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

        // Stops running before calling back, so this runs once per attempt and the callback can start another.
        private void End(LevelOutcome outcome)
        {
            _running = false;
            _interaction.Release();
            var ended = outcome == LevelOutcome.Won ? _onWon : _onFailed;
            _onWon = null;
            _onFailed = null;
            ended();
        }
    }
}
