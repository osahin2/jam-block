using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Scenes
{
    internal sealed class SceneService : ISceneService
    {
        private readonly ISceneLoader _loader;
        private readonly ISceneTransition _transition;
        private readonly IProgress<float> _loadProgress;
        private bool _switching;

        public string CurrentScene { get; private set; }

        public SceneService(ISceneLoader loader, ISceneTransition transition)
        {
            _loader = loader;
            _transition = transition;
            _loadProgress = new Progress(transition);
        }

        public async UniTask SwitchToAsync(string sceneName, CancellationToken cancellationToken)
        {
            // Two overlapping switches would unload each other's scenes; the caller must wait for the first one.
            if (_switching)
                throw new InvalidOperationException($"[Core.Scenes] Cannot switch to '{sceneName}' while another switch is running.");

            _switching = true;
            try
            {
                await _transition.CoverAsync(cancellationToken);
                await UnloadCurrentAsync(cancellationToken);
                _transition.ReportProgress(0f);
                await _loader.LoadAsync(sceneName, _loadProgress, cancellationToken);
                CurrentScene = sceneName;
                await _transition.UncoverAsync(cancellationToken);
            }
            finally
            {
                _switching = false;
            }
        }

        private async UniTask UnloadCurrentAsync(CancellationToken cancellationToken)
        {
            if (CurrentScene == null) return;

            await _loader.UnloadAsync(CurrentScene, cancellationToken);
            CurrentScene = null;
        }

        private sealed class Progress : IProgress<float>
        {
            private readonly ISceneTransition _transition;

            public Progress(ISceneTransition transition)
            {
                _transition = transition;
            }

            public void Report(float value) => _transition.ReportProgress(value);
        }
    }
}
