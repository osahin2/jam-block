using Core.UI;
using Game.Levels;
using Game.Logic.Coins;
using Game.Play;
using Game.Preferences;
using VContainer.Unity;

namespace Game.Bootstrap
{
    // The Game scene's flow: one method per step, each screen's buttons lead to the next one. The HUD stays open over
    // every level; restart plays the level again, pause stops it under the settings popup, whose close resumes it and
    // whose home leaves the scene. A win opens the level complete popup, whose continue pays the reward and plays the
    // next level; a fail opens the level failed popup, with retry and home. Which level is next is the run's, chosen
    // once at the start: the player's run completes their level, a run started from the level editor is a designer's
    // test that never moves progress.
    public sealed class GameEntry : IStartable
    {
        private readonly PlaySession _session;
        private readonly LevelRuns _runs;
        private readonly EditorPlayRequest _request;
        private readonly IUIService _ui;
        private readonly CoinWallet _wallet;
        private readonly WinRewardSettings _reward;
        private readonly SceneFlow _flow;
        private ILevelRun _run;
        private LevelHud _hud;

        public GameEntry(PlaySession session, LevelRuns runs, EditorPlayRequest request, IUIService ui,
            CoinWallet wallet, WinRewardSettings reward, SceneFlow flow)
        {
            _session = session;
            _runs = runs;
            _request = request;
            _ui = ui;
            _wallet = wallet;
            _reward = reward;
            _flow = flow;
        }

        public void Start()
        {
            _run = _request.TryTake(out var tested) ? _runs.RunForTest(tested) : _runs.RunForPlayer();
            _hud = _ui.Open<LevelHud>();
            Play();
        }

        private void Play()
        {
            _session.Start(_run.Level.Level, Won, Failed);
            _hud.Show(_run.Number, _wallet.Balance, Restart, Pause);
        }

        // Restart and pause do nothing once the level is decided: its end is on the way, and a restart or a home in
        // between would throw away a win the player has seen leave the board.
        private void Restart()
        {
            if (!_session.IsDecided)
                Play();
        }

        private void Pause()
        {
            if (_session.IsDecided)
                return;
            _session.Pause();
            _ui.Open<SettingsPopup>().Show(_session.Resume, _flow.ToHome);
        }

        private void Won() => _ui.Open<LevelCompletePopup>().Show(_reward.Coins, Continue);

        private void Continue()
        {
            _wallet.Add(_reward.Coins);
            _run.Advance();
            Play();
        }

        private void Failed() => _ui.Open<LevelFailedPopup>().Show(Play, _flow.ToHome);
    }
}
