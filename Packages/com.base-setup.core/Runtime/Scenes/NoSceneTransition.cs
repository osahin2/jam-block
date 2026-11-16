using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Scenes
{
    internal sealed class NoSceneTransition : ISceneTransition
    {
        public UniTask CoverAsync(CancellationToken cancellationToken) => UniTask.CompletedTask;

        public void ReportProgress(float progress) { }

        public UniTask UncoverAsync(CancellationToken cancellationToken) => UniTask.CompletedTask;
    }
}
