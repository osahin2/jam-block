using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Play
{
    public sealed class TimeWarning : MonoBehaviour
    {
        [SerializeField] private Graphic _overlay;

        private TimeWarningSettings _settings;
        private Sequence _flash;

        [Inject]
        public void Construct(TimeWarningSettings settings) => _settings = settings;

        public static bool Flashes(int wholeSeconds, int lastSeconds) => wholeSeconds > 0 && wholeSeconds <= lastSeconds;

        public void Show(int wholeSeconds)
        {
            if (!Flashes(wholeSeconds, _settings.LastSeconds))
                return;
            _flash?.Kill();
            _flash = Build();
        }

        private Sequence Build()
        {
            var color = _settings.FlashColor;
            var peak = color.a;
            color.a = 0f;
            _overlay.color = color;
            var half = _settings.FlashDuration * 0.5f;
            return DOTween.Sequence()
                .Append(_overlay.DOFade(peak, half))
                .Append(_overlay.DOFade(0f, half))
                .SetUpdate(true)
                .SetLink(gameObject);
        }
    }
}
