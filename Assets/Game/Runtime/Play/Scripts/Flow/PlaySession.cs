using System;
using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;
using VContainer.Unity;

namespace Game.Play
{
    // Entry of the play feature: shows a level, hands the board to the interaction and runs the level's time, which it
    // stops while paused. A level ends in one place: the first frame the attempt is won or failed, not paused, and no
    // block is still sliding out through its door, so the caller's next screen comes after the last block has gone.
    // Then the board stops, a held block goes back onto its cell and the caller hears which way the level ended. The
    // session owns the board: every start gives the pieces of the board before back to their pools and builds the
    // level again from its data, so a restart or the next level keeps nothing from the attempt before and
    // instantiates nothing.
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

        // The time left in the level being played, or as it ended.
        public float RemainingSeconds => _attempt == null ? 0f : _attempt.RemainingSeconds;

        // True from the moment the level is won or failed, also while the last block is still sliding out and the
        // callback has not come yet; the level may then only end, not be restarted or left.
        public bool IsDecided => _attempt != null && _attempt.Outcome != LevelOutcome.Playing;

        public PlaySession(LevelStage stage, PooledBoardPieces pieces, BoardRoot board, Camera camera, BoardInteraction interaction)
        {
            _stage = stage;
            _pieces = pieces;
            _board = board;
            _camera = camera;
            _interaction = interaction;
        }

        // Plays the level until it is won or failed and then calls back once. The callback may start the next level.
        // A start while a level is playing abandons it without a callback.
        public void Start(LevelData level, Action onWon, Action onFailed)
        {
            _pieces.ReleaseAll();
            _attempt = new LevelAttempt(level);
            _running = true;
            _onWon = onWon;
            _onFailed = onFailed;
            var staged = _stage.Show(level, _board.transform, _pieces, _camera);
            _interaction.Begin(level, _attempt, staged.Layout, staged.Blocks);
        }

        public void Tick()
        {
            if (!_running)
                return;
            var outcome = _attempt.Tick(Time.deltaTime);
            if (outcome != LevelOutcome.Playing && !_attempt.IsPaused && _interaction.IsSettled)
                End(outcome);
        }

        // Stops the time and the board, and puts a held block back onto its cell.
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
