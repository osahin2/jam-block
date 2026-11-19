using Core.Installers;
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
        [SerializeField] private CameraFramingSettings _framing;

        // Needs the scene's BoardRoot and Camera, registered by the scene's scope.
        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_art);
            builder.RegisterInstance(_palette);
            builder.RegisterInstance(_motion);
            builder.RegisterInstance(_framing);
            builder.Register<PooledBoardPieces>(Lifetime.Scoped);
            builder.Register<LevelStage>(Lifetime.Scoped);
            builder.RegisterEntryPoint<BoardPointer>(Lifetime.Scoped).AsSelf();
            builder.Register<BoardInteraction>(Lifetime.Scoped);
            builder.RegisterEntryPoint<PlaySession>(Lifetime.Scoped).AsSelf();
        }
    }
}
