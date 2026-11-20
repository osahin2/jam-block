using System;
using Core.UI;
using Game.Coins;
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

        private readonly char[] _time = new char[CountdownText.Length];
        private PlaySession _session;
        private Action _onRestart;
        private Action _onPause;
        private int _shownSeconds = -1;

        public CoinCounter Coins => _coins;

        [Inject]
        public void Construct(PlaySession session) => _session = session;

        private void Awake()
        {
            _restartButton.onClick.AddListener(Restart);
            _pauseButton.onClick.AddListener(Pause);
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

        public void Show(int levelNumber, int coins, Action onRestart, Action onPause)
        {
            _levelLabel.SetText("{0}", levelNumber);
            _coins.Show(coins);
            _onRestart = onRestart;
            _onPause = onPause;
        }

        private void Restart() => _onRestart?.Invoke();

        private void Pause() => _onPause?.Invoke();
    }
}
