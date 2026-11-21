using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Popups
{
    public sealed class PopupEntrance : MonoBehaviour
    {
        [SerializeField] private Graphic _dimmer;
        [SerializeField] private RectTransform[] _parts;

        private PopupMotionSettings _settings;
        private float _dimAlpha;
        private Vector3[] _restScales;
        private Sequence _entrance;

        [Inject]
        public void Construct(PopupMotionSettings settings) => _settings = settings;

        private void Awake()
        {
            _dimAlpha = _dimmer.color.a;
            _restScales = new Vector3[_parts.Length];
            for (var i = 0; i < _parts.Length; i++)
                _restScales[i] = _parts[i].localScale;
        }

        private void OnEnable()
        {
            _entrance?.Kill();
            _entrance = Build();
        }

        private Sequence Build()
        {
            var sequence = DOTween.Sequence()
                .SetUpdate(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);

            var hidden = _dimmer.color;
            hidden.a = 0f;
            _dimmer.color = hidden;
            sequence.Insert(0f, _dimmer.DOFade(_dimAlpha, _settings.DimDuration));

            for (var i = 0; i < _parts.Length; i++)
            {
                var part = _parts[i];
                part.localScale = _restScales[i] * _settings.StartScale;
                sequence.Insert(i * _settings.Stagger, part.DOScale(_restScales[i], _settings.PopDuration).SetEase(_settings.PopEase));
            }

            return sequence;
        }
    }
}
