using Game.Levels.Settings;
using Game.Play.Flow;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class GameEntry : IStartable
    {
        private readonly PlaySession _session;
        private readonly LevelCatalog _catalog;

        public GameEntry(PlaySession session, LevelCatalog catalog)
        {
            _session = session;
            _catalog = catalog;
        }

        public void Start() => _session.Start(_catalog.Levels[0].Level);
    }
}
