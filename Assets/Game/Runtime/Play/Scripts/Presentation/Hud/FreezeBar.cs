using DG.Tweening;
using UnityEngine;
using VContainer;

namespace Game.Play
{
    public sealed class FreezeBar : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _panel;
        [SerializeField] private RectTransform _fill;

        private FreezeBarSettings _settings;
        private float _fullSeconds;

        private bool IsOpen => _fullSeconds > 0f;

        [Inject]
        public void Construct(FreezeBarSettings settings) => _settings = settings;

        public void Show(float frozenSeconds)
        {
            if (frozenSeconds > 0f && !IsOpen)
                Open(frozenSeconds);
            else if (frozenSeconds <= 0f && IsOpen)
                Close();
            if (IsOpen)
                ShowLeft(frozenSeconds / _fullSeconds);
        }

        private void Open(float frozenSeconds)
        {
            _fullSeconds = frozenSeconds;
            DOTween.Kill(this);
            _panel.gameObject.SetActive(true);
            _panel.alpha = 0f;
            var panel = _panel.transform;
            panel.localScale = Vector3.one * _settings.OpenScale;
            DOTween.Sequence()
                .Join(_panel.DOFade(1f, _settings.OpenDuration))
                .Join(panel.DOScale(1f, _settings.OpenDuration).SetEase(_settings.OpenEase))
                .SetTarget(this)
                .SetLink(gameObject);
        }

        private void Close()
        {
            _fullSeconds = 0f;
            DOTween.Kill(this);
            _panel.DOFade(0f, _settings.CloseDuration)
                .SetEase(_settings.CloseEase)
                .OnComplete(Hide)
                .SetTarget(this)
                .SetLink(gameObject);
        }

        private void ShowLeft(float share)
        {
            var max = _fill.anchorMax;
            if (Mathf.Approximately(max.x, share))
                return;
            max.x = share;
            _fill.anchorMax = max;
        }

        private void Hide() => _panel.gameObject.SetActive(false);
    }
}
