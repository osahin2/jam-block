using System;
using System.Threading;
using Core.Scenes;
using Core.UI;
using Cysharp.Threading.Tasks;
using Game.Coins;
using Game.Levels;
using Game.Logic.Boosters;
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
        private readonly IUIService _ui;
        private readonly CoinWallet _wallet;
        private readonly WinRewardSettings _reward;
        private readonly SceneFlow _flow;
        private readonly ISceneTransition _transition;
        private readonly CoinFlight _coinFlight;
        private readonly BoosterPurchase _purchase;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();
        private ILevelRun _run;
        private LevelHud _hud;
        private LevelCompletePopup _complete;
        private LevelFailedPopup _failed;
        private BoosterPurchasePopup _offer;
        private BoosterDefinition _offered;

        public GameEntry(PlaySession session, LevelRuns runs, IUIService ui, CoinWallet wallet, WinRewardSettings reward,
            SceneFlow flow, ISceneTransition transition, CoinFlight coinFlight, BoosterPurchase purchase)
        {
            _session = session;
            _runs = runs;
            _ui = ui;
            _wallet = wallet;
            _reward = reward;
            _flow = flow;
            _transition = transition;
            _coinFlight = coinFlight;
            _purchase = purchase;
        }

        public void Start()
        {
            _run = _runs.TakeRun();
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
            _hud.Show(_run.Number, _wallet.Balance, Restart, Pause, OfferBooster);
        }

        private void OfferBooster(BoosterDefinition booster)
        {
            _session.Pause();
            _offered = booster;
            _offer = _ui.Open<BoosterPurchasePopup>();
            _offer.Show(_offered, _purchase.CanAfford(_offered.Price), Buy, CloseOffer);
        }

        private void Buy()
        {
            _purchase.TryBuy(_offered.Id, _offered.PackSize, _offered.Price);
            _hud.Coins.Show(_wallet.Balance);
            CloseOffer();
        }

        private void CloseOffer()
        {
            _ui.Close(_offer);
            _session.Resume();
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
            _ui.Open<SettingsPopup>().ShowWithHome(_session.Resume, _flow.ToHome);
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
