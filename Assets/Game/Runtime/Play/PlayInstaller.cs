using Core.Installers;
using Game.Play.Flow;
using Game.Play.Presentation;
using Game.Play.Settings;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Play Installer")]
    public sealed class PlayInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private BoardArtSettings _art;
        [SerializeField] private ColorPalette _palette;
        [SerializeField] private BlockMotionSettings _motion;

        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_art);
            builder.RegisterInstance(_palette);
            builder.RegisterInstance(_motion);
            builder.Register<BlockPieceBuilder>(Lifetime.Scoped);
            builder.Register<BoardBuilder>(Lifetime.Scoped);
            builder.RegisterEntryPoint<BoardPointer>(Lifetime.Scoped).AsSelf();
            builder.Register<PlaySession>(Lifetime.Scoped);
        }
    }
}
