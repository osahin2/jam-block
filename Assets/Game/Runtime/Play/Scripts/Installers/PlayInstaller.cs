using Core.Installers;
using Core.UI;
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
        [SerializeField] private BlockOutlineSettings _outline;
        [SerializeField] private IceSettings _ice;
        [SerializeField] private CameraFramingSettings _framing;
        [SerializeField] private WinRewardSettings _reward;
        [SerializeField] private TimeWarningSettings _timeWarning;
        [SerializeField] private UIScreenCatalog _screens;

        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_art);
            builder.RegisterInstance(_palette);
            builder.RegisterInstance(_motion);
            builder.RegisterInstance(_outline);
            builder.Register<BlockOutlineMaterials>(Lifetime.Scoped);
            builder.RegisterInstance(_ice);
            builder.RegisterInstance(_framing);
            builder.RegisterInstance(_reward);
            builder.RegisterInstance(_timeWarning);
            builder.RegisterUIScreens(_screens);
            builder.Register<PooledBoardPieces>(Lifetime.Scoped);
            builder.Register<LevelStage>(Lifetime.Scoped);
            builder.Register<TouchPlane>(Lifetime.Scoped);
            builder.Register<BoardInteraction>(Lifetime.Scoped);
            builder.RegisterEntryPoint<PlaySession>(Lifetime.Scoped).AsSelf();
        }
    }
}
