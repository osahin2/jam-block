using Core.Levels;

namespace Game.Levels
{
    public sealed class LevelRuns
    {
        private readonly LevelCatalog _catalog;
        private readonly ILevelProgression _progression;
        private readonly EditorPlayRequest _request;

        public bool HasTestRun => _request.IsPending;

        public LevelRuns(LevelCatalog catalog, ILevelProgression progression, EditorPlayRequest request)
        {
            _catalog = catalog;
            _progression = progression;
            _request = request;
        }

        public ILevelRun TakeRun() => _request.TryTake(out var tested)
            ? new TestRun(_catalog, _progression, tested)
            : new PlayerRun(_catalog, _progression);
    }
}
