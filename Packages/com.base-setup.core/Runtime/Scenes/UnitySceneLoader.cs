using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Scenes
{
    internal sealed class UnitySceneLoader : ISceneLoader
    {
        public async UniTask LoadAsync(string sceneName, IProgress<float> progress, CancellationToken cancellationToken)
        {
            AsyncOperation load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (load == null)
                throw new ArgumentException($"[Core.Scenes] Scene '{sceneName}' is not in the build scene list.", nameof(sceneName));

            await load.ToUniTask(progress, cancellationToken: cancellationToken);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            progress.Report(1f);
        }

        public async UniTask UnloadAsync(string sceneName, CancellationToken cancellationToken)
        {
            await SceneManager.UnloadSceneAsync(sceneName).ToUniTask(cancellationToken: cancellationToken);
            await Resources.UnloadUnusedAssets().ToUniTask(cancellationToken: cancellationToken);
        }
    }
}
