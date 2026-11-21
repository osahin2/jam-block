using System;
using System.Threading;
using Core.Scenes;
using Core.UI;
using Cysharp.Threading.Tasks;
using Game.Coins;
using Game.Levels;
using Game.Logic.Coins;
using Game.Play;
using Game.Preferences;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class GameEntry : IStartable, IDisposable
    {
        private readonly PlaySession _session;
        private readonly LevelRuns _runs;
        private readonly EditorPlayRequest _request;
        private readonly IUIService _ui;
        private readonly CoinWallet _wallet;
        private readonly WinRewardSettings _reward;
        private readonly SceneFlow _flow;
        private readonly ISceneTransition _transition;
        private readonly CoinFlight _coinFlight;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();
        private ILevelRun _run;
        private LevelHud _hud;
        private LevelCompletePopup _complete;
        private LevelFailedPopup _failed;

        public GameEntry(PlaySession session, LevelRuns runs, EditorPlayRequest request, IUIService ui,
            CoinWallet wallet, WinRewardSettings reward, SceneFlow flow, ISceneTransition transition, CoinFlight coinFlight)
        {
            _session = session;
            _runs = runs;
            _request = request;
            _ui = ui;
            _wallet = wallet;
            _reward = reward;
            _flow = flow;
            _transition = transition;
            _coinFlight = coinFlight;
        }

        public void Start()
        {
            _run = _request.TryTake(out var tested) ? _runs.RunForTest(tested) : _runs.RunForPlayer();
            _hud = _ui.Open<LevelHud>();
            Play();
        }

        public void Dispose()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        private void Play()
        {
            _session.Start(_run.Level.Level, Won, Failed);
            _hud.Show(_run.Number, _wallet.Balance, Restart, Pause);
        }

        private void Restart()
        {
            if (_session.IsDecided)
                return;
            _session.Pause();
            PlayCoveredAsync(null, _lifetime.Token).Forget();
        }

        private void Pause()
        {
            if (_session.IsDecided)
                return;
            _session.Pause();
            _ui.Open<SettingsPopup>().Show(_session.Resume, _flow.ToHome);
        }

        private void Won()
        {
            _complete = _ui.Open<LevelCompletePopup>();
            _complete.Show(_reward.Coins, Continue);
        }

        private void Continue()
        {
            _wallet.Add(_reward.Coins);
            _run.Advance();
            _coinFlight.Play(_complete.RewardOrigin, _hud.Coins, _reward.Coins, PlayNext);
        }

        private void PlayNext() => PlayCoveredAsync(_complete, _lifetime.Token).Forget();

        private void Failed()
        {
            _failed = _ui.Open<LevelFailedPopup>();
            _failed.Show(Retry, _flow.ToHome);
        }

        private void Retry() => PlayCoveredAsync(_failed, _lifetime.Token).Forget();

        private async UniTaskVoid PlayCoveredAsync(UIScreen leaving, CancellationToken token)
        {
            await _transition.CoverAsync(token);
            if (leaving != null)
                _ui.Close(leaving);
            Play();
            await _transition.UncoverAsync(token);
        }
    }
}
