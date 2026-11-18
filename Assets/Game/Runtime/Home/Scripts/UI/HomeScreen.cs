using System;
using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Home.UI
{
    // The home screen: the player's level on the level button and the coin count.
    public sealed class HomeScreen : UIScreen
    {
        [SerializeField] private Button _levelButton;
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private TMP_Text _coinsLabel;

        private Action _onLevel;

        private void Awake() => _levelButton.onClick.AddListener(StartLevel);

        public void Show(int levelNumber, int coins, Action onLevel)
        {
            _levelLabel.text = $"Level {levelNumber}";
            _coinsLabel.text = coins.ToString();
            _onLevel = onLevel;
        }

        private void StartLevel()
        {
            var onLevel = _onLevel;
            _onLevel = null;
            Close();
            onLevel?.Invoke();
        }
    }
}
