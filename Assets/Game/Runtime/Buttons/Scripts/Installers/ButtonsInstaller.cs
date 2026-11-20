using Core.Installers;
using UnityEngine;
using VContainer;

namespace Game.Buttons
{
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
