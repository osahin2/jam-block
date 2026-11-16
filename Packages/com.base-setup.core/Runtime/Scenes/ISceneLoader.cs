using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Scenes
{
    // Seam between the switch sequence and the engine, so the sequence is tested without loading scenes.
    internal interface ISceneLoader
    {
        // Loads additively and makes the scene active; reports 1 when done.
        UniTask LoadAsync(string sceneName, IProgress<float> progress, CancellationToken cancellationToken);

        // Unloads the scene and then the assets nothing references any more.
        UniTask UnloadAsync(string sceneName, CancellationToken cancellationToken);
    }
}
