using Core.Installers;
using Core.UI;
using UnityEngine;
using VContainer;

namespace Game.Preferences
{
    // In every content scope that opens the settings popup; the audio and haptics services it changes are the Boot
    // scope's.
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
