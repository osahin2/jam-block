using System;
using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;
using VContainer.Unity;

namespace Game.Play
{
    // Entry of the play feature: builds a level, hands the board to the interaction and runs the level's time. On the
    // first frame the attempt is no longer playing, won or failed, the session ends it in one place: the board stops,
    // a held block goes back onto its cell and the caller hears which way the level ended. Every start builds the
    // level again from its data, so a restart or the next level keeps nothing from the attempt before.
    public sealed class PlaySession : ITickable
    {
        private readonly BoardBuilder _builder;
        private readonly BoardInteraction _interaction;
        private readonly BoardArtSettings _art;
        private LevelAttempt _attempt;
        private Action _onWon;
        private Action _onFailed;

        public PlaySession(BoardBuilder builder, BoardInteraction interaction, BoardArtSettings art)
        {
            _builder = builder;
            _interaction = interaction;
            _art = art;
        }

        // Plays the level until it is won or failed and then calls back once. The callback may start the next level.
        public void Start(LevelData level, Action onWon, Action onFailed)
        {
            _attempt = new LevelAttempt(level);
            _onWon = onWon;
            _onFailed = onFailed;
            var layout = new BoardLayout(level.Width, level.Height, _art.CellSize);
            _interaction.Begin(level, _attempt, layout, _builder.Build(level, layout));
        }

        public void Tick()
        {
            if (_attempt == null)
                return;
            var outcome = _attempt.Tick(Time.deltaTime);
            if (outcome != LevelOutcome.Playing)
                End(outcome);
        }

        // Forgets the attempt before calling back, so this runs once per attempt and the callback can start another.
        private void End(LevelOutcome outcome)
        {
            _attempt = null;
            _interaction.Release();
            var ended = outcome == LevelOutcome.Won ? _onWon : _onFailed;
            _onWon = null;
            _onFailed = null;
            ended();
        }
    }
}
