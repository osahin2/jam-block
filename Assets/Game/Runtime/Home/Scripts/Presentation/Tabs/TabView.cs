using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Home
{
    [RequireComponent(typeof(Toggle), typeof(LayoutElement))]
    public sealed class TabView : MonoBehaviour
    {
        [SerializeField] private GameObject _card;
        [SerializeField] private RectTransform _icon;
        [SerializeField] private RectTransform _label;

        private TabBarSettings _settings;
        private Toggle _toggle;
        private LayoutElement _layout;
        private float _iconRestY;
        private Vector3 _iconRestScale;
        private Vector3 _labelRestScale;
        private bool _selected;

        [Inject]
        public void Construct(TabBarSettings settings) => _settings = settings;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _layout = GetComponent<LayoutElement>();
            _iconRestY = _icon.anchoredPosition.y;
            _iconRestScale = _icon.localScale;
            _labelRestScale = _label.localScale;
            _toggle.onValueChanged.AddListener(OnToggled);
        }

        private void Start()
        {
            Select(_toggle.isOn);
            _icon.DOComplete();
            _label.DOComplete();
        }

        private void OnToggled(bool on)
        {
            if (on != _selected)
                Select(on);
        }

        private void Select(bool on)
        {
            _selected = on;
            _layout.flexibleWidth = on ? _settings.SelectedWidth : 1f;
            _card.SetActive(on);
            _label.gameObject.SetActive(on);
            _icon.DOKill();
            _label.DOKill();

            var duration = _settings.SelectDuration;
            Styled(_icon.DOAnchorPosY(on ? _iconRestY + _settings.IconRise : _iconRestY, duration));
            Styled(_icon.DOScale(on ? _iconRestScale * _settings.IconScale : _iconRestScale, duration));
            if (on)
                Styled(_label.DOScale(_labelRestScale, duration).From(Vector3.zero));
        }

        private void Styled(Tween tween) => tween.SetEase(_settings.SelectEase).SetUpdate(true).SetLink(gameObject);
    }
}
