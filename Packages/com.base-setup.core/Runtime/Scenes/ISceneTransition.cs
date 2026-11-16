using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Scenes
{
    // What the player sees while scenes change: a fade, a loading screen. Covering an already covered screen completes at once.
    public interface ISceneTransition
    {
        UniTask CoverAsync(CancellationToken cancellationToken);

        // Load progress of the incoming scene, 0 to 1, reported while covered.
        void ReportProgress(float progress);

        UniTask UncoverAsync(CancellationToken cancellationToken);
    }
}
