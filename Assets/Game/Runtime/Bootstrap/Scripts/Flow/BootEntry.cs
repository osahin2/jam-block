using Game.Levels;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class BootEntry : IStartable
    {
        private readonly SceneFlow _flow;
        private readonly EditorPlayRequest _request;

        public BootEntry(SceneFlow flow, EditorPlayRequest request)
        {
            _flow = flow;
            _request = request;
        }

        public void Start()
        {
            if (_request.IsPending)
                _flow.ToGame();
            else
                _flow.ToHome();
        }
    }
}
