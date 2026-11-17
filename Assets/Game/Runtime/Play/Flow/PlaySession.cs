using Game.Data.Levels;
using Game.Logic.Play;
using Game.Play.Presentation;
using Game.Play.Settings;
using UnityEngine;
using VContainer.Unity;

namespace Game.Play.Flow
{
    // Entry of the play feature: builds a level, hands the board to the interaction and runs the level's time. When
    // the time runs out the board stops and a held block goes back onto its cell.
    public sealed class PlaySession : ITickable
    {
        private readonly BoardBuilder _builder;
        private readonly BoardInteraction _interaction;
        private readonly BoardArtSettings _art;
        private LevelAttempt _attempt;

        public PlaySession(BoardBuilder builder, BoardInteraction interaction, BoardArtSettings art)
        {
            _builder = builder;
            _interaction = interaction;
            _art = art;
        }

        public void Start(LevelData level)
        {
            _attempt = new LevelAttempt(level);
            var layout = new BoardLayout(level.Width, level.Height, _art.CellSize);
            _interaction.Begin(level, _attempt, layout, _builder.Build(level, layout));
        }

        public void Tick()
        {
            if (_attempt == null || _attempt.Outcome != LevelOutcome.Playing)
                return;
            if (_attempt.Tick(Time.deltaTime) == LevelOutcome.Failed)
                _interaction.Release();
        }
    }
}
