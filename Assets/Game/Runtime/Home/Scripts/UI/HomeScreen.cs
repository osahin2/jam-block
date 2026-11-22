using System;
using Core.UI;
using Game.Coins;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Home
{
    public sealed class HomeScreen : UIScreen
    {
        [SerializeField] private Button _levelButton;
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private LevelPathView _path;
        [SerializeField] private CoinCounter _coins;
        [SerializeField] private Button _settingsButton;

        private Action _onLevel;
        private Action _onSettings;

        private void Awake()
        {
            _levelButton.onClick.AddListener(StartLevel);
            _settingsButton.onClick.AddListener(OpenSettings);
        }

        public void Show(int levelNumber, int coins, Action onLevel, Action onSettings)
        {
            _levelLabel.SetText("Level {0}", levelNumber);
            _path.Show(levelNumber);
            _coins.Show(coins);
            _onLevel = onLevel;
            _onSettings = onSettings;
        }

        private void StartLevel()
        {
            var onLevel = _onLevel;
            _onLevel = null;
            _onSettings = null;
            Close();
            onLevel?.Invoke();
        }

        private void OpenSettings() => _onSettings?.Invoke();
    }
}
