using Core.Installers;
using Game.Logic.Coins;
using UnityEngine;
using VContainer;

namespace Game.Coins
{
    // In the Boot scope: one wallet for the whole app, so the balance is the same in every scene.
    [CreateAssetMenu(menuName = "Game/Coins/Coins Installer")]
    public sealed class CoinsInstaller : ScriptableObjectInstaller
    {
        public override void Install(IContainerBuilder builder)
        {
            builder.Register<ICoinStore, SavedCoinStore>(Lifetime.Singleton);
            builder.Register<CoinWallet>(Lifetime.Singleton);
        }
    }
}
