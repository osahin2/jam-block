using Core.Installers;
using Core.UI;
using UnityEngine;
using VContainer;

namespace Game.Home
{
    [CreateAssetMenu(menuName = "Game/Home/Home Installer")]
    public sealed class HomeInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private UIScreenCatalog _screens;
        [SerializeField] private TabBarSettings _tabBar;

        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterUIScreens(_screens);
            builder.RegisterInstance(_tabBar);
        }
    }
}
