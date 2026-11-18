using Game.Levels;
using VContainer.Unity;

namespace Game.Bootstrap
{
    // Opens the first content scene: the game straight away for a level the level editor asked to play, else home.
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
