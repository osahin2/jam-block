using Game.Levels;
using Game.Levels.Settings;
using Game.Play.Flow;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class GameEntry : IStartable
    {
        private readonly PlaySession _session;
        private readonly LevelCatalog _catalog;
        private readonly EditorPlayRequest _request;

        public GameEntry(PlaySession session, LevelCatalog catalog, EditorPlayRequest request)
        {
            _session = session;
            _catalog = catalog;
            _request = request;
        }

        public void Start() => _session.Start(StartingLevel().Level);

        // The level the level editor asked to play, else the first level of the catalog.
        private LevelAsset StartingLevel() => _request.TryTake(out var level) ? level : _catalog.Levels[0];
    }
}
