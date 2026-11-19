using System;
using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Play
{
    // Opens once the last block has left: the coins the win pays and a continue button.
    public sealed class LevelCompletePopup : UIScreen
    {
        [SerializeField] private TMP_Text _rewardLabel;
        [SerializeField] private Button _continueButton;

        private Action _onContinue;

        public override UILayer Layer => UILayer.Popup;

        private void Awake() => _continueButton.onClick.AddListener(Continue);

        public void Show(int coins, Action onContinue)
        {
            _rewardLabel.SetText("+{0}", coins);
            _onContinue = onContinue;
        }

        private void Continue()
        {
            var onContinue = _onContinue;
            _onContinue = null;
            Close();
            onContinue?.Invoke();
        }
    }
}
