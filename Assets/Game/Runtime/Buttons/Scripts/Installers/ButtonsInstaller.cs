using Core.Installers;
using UnityEngine;
using VContainer;

namespace Game.Buttons
{
    // In the Boot scope, so the buttons of every content scene's screens get the same feedback settings.
    [CreateAssetMenu(menuName = "Game/Buttons/Buttons Installer")]
    public sealed class ButtonsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private ButtonFeedbackSettings _feedback;

        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_feedback);
        }
    }
}
