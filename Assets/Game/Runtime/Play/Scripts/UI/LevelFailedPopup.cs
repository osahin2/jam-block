using System;
using Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Play
{
    public sealed class LevelFailedPopup : UIScreen
    {
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _homeButton;

        private Action _onRetry;
        private Action _onHome;

        public override UILayer Layer => UILayer.Popup;

        private void Awake()
        {
            _retryButton.onClick.AddListener(ChooseRetry);
            _homeButton.onClick.AddListener(ChooseHome);
        }

        public void Show(Action onRetry, Action onHome)
        {
            _onRetry = onRetry;
            _onHome = onHome;
        }

        private void ChooseRetry() => End(_onRetry);

        private void ChooseHome() => End(_onHome);

        // Clears both callbacks before calling the chosen one, so a second click does nothing. Stays open: both choices
        // cover the screen before it goes, so its dimmer never drops over the board.
        private void End(Action chosen)
        {
            _onRetry = null;
            _onHome = null;
            chosen?.Invoke();
        }
    }
}
