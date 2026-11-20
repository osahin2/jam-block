using Core.Installers;
using UnityEngine;
using VContainer;

namespace Game.Popups
{
    [CreateAssetMenu(menuName = "Game/Popups/Popups Installer")]
    public sealed class PopupsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private PopupMotionSettings _motion;

        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_motion);
        }
    }
}
