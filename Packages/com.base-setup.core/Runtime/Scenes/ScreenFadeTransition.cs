using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scenes
{
    public sealed class ScreenFadeTransition : MonoBehaviour, ISceneTransition
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private float _fadeDuration = 0.25f;

        public UniTask CoverAsync(CancellationToken cancellationToken)
        {
            _group.blocksRaycasts = true;
            return FadeAsync(1f, cancellationToken);
        }

        public void ReportProgress(float progress) { }

        public async UniTask UncoverAsync(CancellationToken cancellationToken)
        {
            await FadeAsync(0f, cancellationToken);
            _group.blocksRaycasts = false;
        }

        // Unscaled time, so a paused game (timeScale 0) still fades.
        private async UniTask FadeAsync(float target, CancellationToken cancellationToken)
        {
            float speed = 1f / Mathf.Max(_fadeDuration, 0.0001f);
            while (!Mathf.Approximately(_group.alpha, target))
            {
                _group.alpha = Mathf.MoveTowards(_group.alpha, target, speed * Time.unscaledDeltaTime);
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            _group.alpha = target;
        }
    }
}
