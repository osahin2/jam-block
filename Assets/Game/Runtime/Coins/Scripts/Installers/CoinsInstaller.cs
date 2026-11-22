using Core.Installers;
using Game.Logic.Coins;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Coins
{
    [CreateAssetMenu(menuName = "Game/Coins/Coins Installer")]
    public sealed class CoinsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private CoinFlightSettings _flightSettings;
        [SerializeField] private CoinFlight _flight;

        public override void Install(IContainerBuilder builder)
        {
            builder.Register<ICoinStore, SavedCoinStore>(Lifetime.Singleton);
            builder.Register<CoinWallet>(Lifetime.Singleton);
            builder.RegisterInstance(_flightSettings);
            builder.RegisterComponentInNewPrefab(_flight, Lifetime.Singleton).DontDestroyOnLoad();
        }
    }
}
