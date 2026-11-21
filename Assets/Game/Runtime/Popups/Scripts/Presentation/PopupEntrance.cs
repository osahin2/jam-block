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
        private Sequence _entrance;

        [Inject]
        public void Construct(PopupMotionSettings settings) => _settings = settings;

        private void OnEnable()
        {
            _entrance ??= Build();
            _entrance.Restart();
        }

        private Sequence Build()
        {
            var sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .SetUpdate(true)
                .SetLink(gameObject, LinkBehaviour.PauseOnDisable);

            var dimAlpha = _dimmer.color.a;
            var hidden = _dimmer.color;
            hidden.a = 0f;
            _dimmer.color = hidden;
            sequence.Insert(0f, _dimmer.DOFade(dimAlpha, _settings.DimDuration));

            for (var i = 0; i < _parts.Length; i++)
            {
                var part = _parts[i];
                var restScale = part.localScale;
                part.localScale = restScale * _settings.StartScale;
                sequence.Insert(i * _settings.Stagger, part.DOScale(restScale, _settings.PopDuration).SetEase(_settings.PopEase));
            }

            return sequence;
        }
    }
}
