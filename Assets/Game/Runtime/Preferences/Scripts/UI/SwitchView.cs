using System;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Preferences
{
    public sealed class SwitchView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Graphic _track;
        [SerializeField] private RectTransform _knob;
        [SerializeField] private Graphic _onFace;
        [SerializeField] private Graphic _offFace;

        private SwitchSettings _settings;
        private DOGetter<float> _getProgress;
        private DOSetter<float> _setProgress;
        private Tween _motion;
        private float _progress;
        private bool _isOn;
        private Action<bool> _onChanged;

        [Inject]
        public void Construct(SwitchSettings settings) => _settings = settings;

        private void Awake()
        {
            _button.onClick.AddListener(Flip);
            _getProgress = () => _progress;
            _setProgress = Apply;
        }

        private void OnDisable() => Show(_isOn);

        public void Listen(Action<bool> onChanged) => _onChanged = onChanged;

        public void Show(bool isOn)
        {
            _isOn = isOn;
            _motion?.Kill();
            _motion = null;
            Apply(isOn ? 1f : 0f);
        }

        private void Flip()
        {
            _isOn = !_isOn;
            _motion?.Kill();
            _motion = DOTween.To(_getProgress, _setProgress, _isOn ? 1f : 0f, _settings.Duration)
                .SetEase(_settings.Ease)
                .SetUpdate(true)
                .SetLink(gameObject);
            _onChanged?.Invoke(_isOn);
        }

        private void Apply(float progress)
        {
            _progress = progress;
            var track = _track.rectTransform.rect;
            var travel = (track.width - track.height) * 0.5f;
            _knob.anchoredPosition = new Vector2(Mathf.Lerp(-travel, travel, progress), 0f);
            SetAlpha(_onFace, progress);
            SetAlpha(_offFace, 1f - progress);
            _track.color = Color.Lerp(_settings.OffTrack, _settings.OnTrack, progress);
        }

        private static void SetAlpha(Graphic graphic, float alpha)
        {
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }
}
