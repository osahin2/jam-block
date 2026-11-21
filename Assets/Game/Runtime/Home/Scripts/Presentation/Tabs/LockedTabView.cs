using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Game.Home
{
    public sealed class LockedTabView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform _icon;

        private TabBarSettings _settings;
        private Tween _shake;

        [Inject]
        public void Construct(TabBarSettings settings) => _settings = settings;

        public void OnPointerClick(PointerEventData eventData)
        {
            _shake?.Kill(true);
            _shake = _icon.DOShakeAnchorPos(_settings.ShakeDuration, Vector2.right * _settings.ShakeStrength,
                    _settings.ShakeVibrato, 0f)
                .SetUpdate(true)
                .SetLink(gameObject);
        }
    }
}
