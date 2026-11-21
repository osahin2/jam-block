using System;
using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Play
{
    public sealed class BoosterPurchasePopup : UIScreen
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Button _buyButton;
        [SerializeField] private CanvasGroup _buyGroup;
        [SerializeField] private TMP_Text _offer;
        [SerializeField] private Button _closeButton;
        [SerializeField, Range(0f, 1f)] private float _offAlpha = 0.5f;

        private Action _onBuy;
        private Action _onClose;

        public override UILayer Layer => UILayer.Popup;

        private void Awake()
        {
            _buyButton.onClick.AddListener(ChooseBuy);
            _closeButton.onClick.AddListener(ChooseClose);
        }

        public void Show(BoosterDefinition booster, bool canAfford, Action onBuy, Action onClose)
        {
            _title.SetText(booster.Title);
            _icon.sprite = booster.Icon;
            _description.SetText(booster.Description);
            _offer.SetText("{0} for {1}", booster.PackSize, booster.Price);
            _buyButton.interactable = canAfford;
            _buyGroup.alpha = canAfford ? 1f : _offAlpha;
            _onBuy = onBuy;
            _onClose = onClose;
        }

        private void ChooseBuy() => End(_onBuy);

        private void ChooseClose() => End(_onClose);

        private void End(Action chosen)
        {
            _onBuy = null;
            _onClose = null;
            chosen?.Invoke();
        }
    }
}
