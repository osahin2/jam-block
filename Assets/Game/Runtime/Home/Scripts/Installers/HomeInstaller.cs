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

        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterUIScreens(_screens);
        }
    }
}
