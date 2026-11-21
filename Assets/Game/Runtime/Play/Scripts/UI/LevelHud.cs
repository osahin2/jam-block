using System;
using Core.UI;
using Game.Coins;
using Game.Logic.Boosters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Play
{
    public sealed class LevelHud : UIScreen
    {
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private TMP_Text _timeLabel;
        [SerializeField] private CoinCounter _coins;
        [SerializeField] private TimeWarning _timeWarning;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private BoosterBar _boosters;
        [SerializeField] private AimingOverlay _aiming;
        [SerializeField] private BoosterShots _shots;

        private readonly char[] _time = new char[CountdownText.Length];
        private PlaySession _session;
        private BoosterInventory _inventory;
        private Action _onRestart;
        private Action _onPause;
        private Action<BoosterDefinition> _onOutOfStock;
        private int _shownSeconds = -1;

        public CoinCounter Coins => _coins;

        [Inject]
        public void Construct(PlaySession session, BoosterInventory inventory)
        {
            _session = session;
            _inventory = inventory;
        }

        private void Awake()
        {
            _restartButton.onClick.AddListener(Restart);
            _pauseButton.onClick.AddListener(Pause);
            _boosters.Show(PressBooster);
        }

        private void Update()
        {
            var seconds = CountdownText.WholeSeconds(_session.RemainingSeconds);
            if (seconds == _shownSeconds)
                return;
            _shownSeconds = seconds;
            CountdownText.Write(seconds, _time);
            _timeLabel.SetCharArray(_time);
            _timeWarning.Show(seconds);
        }

        public void Show(int levelNumber, int coins, Action onRestart, Action onPause,
            Action<BoosterDefinition> onOutOfStock)
        {
            _levelLabel.SetText("{0}", levelNumber);
            _coins.Show(coins);
            _onRestart = onRestart;
            _onPause = onPause;
            _onOutOfStock = onOutOfStock;
        }

        private void Restart() => _onRestart?.Invoke();

        private void Pause() => _onPause?.Invoke();

        private void PressBooster(IBooster booster)
        {
            if (_inventory.Count(booster.Definition.Id) > 0)
                _session.Use(booster, _aiming, _shots);
            else
                _onOutOfStock?.Invoke(booster.Definition);
        }
    }
}
