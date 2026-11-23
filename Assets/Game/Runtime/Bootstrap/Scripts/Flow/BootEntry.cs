using Game.Levels;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class BootEntry : IStartable
    {
        private readonly SceneFlow _flow;
        private readonly LevelRuns _runs;

        public BootEntry(SceneFlow flow, LevelRuns runs)
        {
            _flow = flow;
            _runs = runs;
        }

        public void Start()
        {
            if (_runs.HasTestRun)
                _flow.ToGame();
            else
                _flow.ToHome();
        }
    }
}
