using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Scenes
{
    // The scene that holds the root scope stays loaded; content scenes are switched additively on top of it.
    public interface ISceneService
    {
        // The content scene loaded through this service, or null before the first switch.
        string CurrentScene { get; }

        // Covers the screen, unloads CurrentScene, loads sceneName additively and makes it active, then uncovers.
        // The token must outlive the scene being left; cancelling stops between steps and leaves the screen covered.
        UniTask SwitchToAsync(string sceneName, CancellationToken cancellationToken);
    }
}
