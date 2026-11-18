using Game.Levels;
using Game.Play.Flow;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class GameEntry : IStartable
    {
        private readonly PlaySession _session;
        private readonly LevelSequence _sequence;
        private readonly EditorPlayRequest _request;

        public GameEntry(PlaySession session, LevelSequence sequence, EditorPlayRequest request)
        {
            _session = session;
            _sequence = sequence;
            _request = request;
        }

        public void Start() => _session.Start(StartingLevel().Level);

        // The level the level editor asked to play, else the player's current level.
        private LevelAsset StartingLevel() => _request.TryTake(out var level) ? level : _sequence.Current;
    }
}
