using System;
using Game.Levels;
using Game.Play.Flow;
using VContainer.Unity;

namespace Game.Bootstrap
{
    // The Game scene's flow: starts the first level and decides what follows when a level ends. A win continues with
    // the next level and a fail plays the same level again, both in this scene; the win screen and the fail popup
    // come between them later. What the next level is depends on the run, chosen once at the start: the player's
    // run completes their level, a run started from the level editor is a designer's test that never moves progress.
    public sealed class GameEntry : IStartable
    {
        private readonly PlaySession _session;
        private readonly LevelSequence _sequence;
        private readonly EditorPlayRequest _request;
        private Func<LevelAsset> _nextAfterWin;
        private LevelAsset _playing;

        public GameEntry(PlaySession session, LevelSequence sequence, EditorPlayRequest request)
        {
            _session = session;
            _sequence = sequence;
            _request = request;
        }

        public void Start()
        {
            if (_request.TryTake(out var tested))
            {
                _nextAfterWin = NextInCatalog;
                Play(tested);
                return;
            }
            _nextAfterWin = _sequence.CompleteCurrent;
            Play(_sequence.Current);
        }

        private void Play(LevelAsset level)
        {
            _playing = level;
            _session.Start(level.Level, Won, Failed);
        }

        private void Won() => Play(_nextAfterWin());

        private void Failed() => Play(_playing);

        private LevelAsset NextInCatalog() => _sequence.After(_playing);
    }
}
