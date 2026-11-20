using Core.Installers;
using Game.Logic.Coins;
using UnityEngine;
using VContainer;

namespace Game.Coins
{
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
