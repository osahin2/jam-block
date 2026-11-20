using Core.Installers;
using Core.UI;
using UnityEngine;
using VContainer;

namespace Game.Preferences
{
    [CreateAssetMenu(menuName = "Game/Preferences/Preferences Installer")]
    public sealed class PreferencesInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private UIScreenCatalog _screens;

        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterUIScreens(_screens);
        }
    }
}
