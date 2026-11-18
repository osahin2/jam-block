using Core.Levels;
using Core.UI;
using Game.Home;
using Game.Logic.Coins;
using VContainer.Unity;

namespace Game.Bootstrap
{
    // Entry of the Home scene: shows the home screen; its level button opens the game at the player's level.
    public sealed class HomeEntry : IStartable
    {
        private readonly IUIService _ui;
        private readonly ILevelProgression _progression;
        private readonly CoinWallet _wallet;
        private readonly SceneFlow _flow;

        public HomeEntry(IUIService ui, ILevelProgression progression, CoinWallet wallet, SceneFlow flow)
        {
            _ui = ui;
            _progression = progression;
            _wallet = wallet;
            _flow = flow;
        }

        public void Start() => _ui.Open<HomeScreen>().Show(_progression.CurrentLevelNumber, _wallet.Balance, _flow.ToGame);
    }
}
