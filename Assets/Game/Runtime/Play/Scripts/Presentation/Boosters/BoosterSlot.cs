using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Play
{
    public sealed class BoosterSlot : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _stock;
        [SerializeField] private TMP_Text _count;
        [SerializeField] private GameObject _buy;
        [SerializeField, Range(0f, 1f)] private float _offAlpha = 0.5f;

        private IBooster _booster;
        private Action<IBooster> _onPressed;
        private int _shownCount = -1;

        public BoosterDefinition Definition => _booster.Definition;
        public RectTransform Rect => (RectTransform)transform;

        private void Awake() => _button.onClick.AddListener(Press);

        public void Bind(IBooster booster, Action<IBooster> onPressed)
        {
            _booster = booster;
            _onPressed = onPressed;
            _icon.sprite = booster.Definition.Icon;
            _shownCount = -1;
        }

        public void Refresh(bool available, int count)
        {
            if (available != _button.interactable)
            {
                _button.interactable = available;
                _group.alpha = available ? 1f : _offAlpha;
            }
            if (count == _shownCount)
                return;
            _shownCount = count;
            _stock.SetActive(count > 0);
            _buy.SetActive(count == 0);
            _count.SetText("{0}", count);
        }

        private void Press() => _onPressed(_booster);
    }
}
