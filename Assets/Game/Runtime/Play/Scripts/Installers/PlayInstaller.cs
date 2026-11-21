using System;
using System.Collections.Generic;
using Core.Installers;
using Core.UI;
using Game.Logic.Boosters;
using Game.Logic.Play;
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
        [SerializeField] private BoosterMotionSettings _boosterMotion;
        [SerializeField] private FreezeDefinition _freeze;
        [SerializeField] private TargetedDefinition _rocket;
        [SerializeField] private TargetedDefinition _hammer;
        [SerializeField] private TargetedDefinition _vacuum;

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
            builder.Register<BlockPieceBuilder>(Lifetime.Scoped);
            builder.Register<BlockViewBuilder>(Lifetime.Scoped);
            builder.Register<LevelStage>(Lifetime.Scoped);
            builder.Register<TouchPlane>(Lifetime.Scoped);
            builder.Register<BoardInteraction>(Lifetime.Scoped);
            builder.RegisterEntryPoint<PlaySession>(Lifetime.Scoped).AsSelf();
            InstallBoosters(builder);
        }

        private void InstallBoosters(IContainerBuilder builder)
        {
            var startingStock = new Dictionary<string, int>();
            builder.RegisterInstance(_boosterMotion);
            builder.Register<IBoosterStore, SavedBoosterStore>(Lifetime.Scoped);
            builder.Register(resolver => new BoosterInventory(resolver.Resolve<IBoosterStore>(), startingStock),
                Lifetime.Scoped);
            builder.Register<BoosterPurchase>(Lifetime.Scoped);
            RegisterBooster(builder, startingStock, _freeze, inventory => new FreezeBooster(_freeze, inventory));
            RegisterBooster(builder, startingStock, _rocket,
                inventory => new TargetedBooster(_rocket, new RocketEffect(), inventory));
            RegisterBooster(builder, startingStock, _hammer,
                inventory => new TargetedBooster(_hammer, new HammerEffect(), inventory));
            RegisterBooster(builder, startingStock, _vacuum,
                inventory => new TargetedBooster(_vacuum, new VacuumEffect(), inventory));
        }

        private static void RegisterBooster(IContainerBuilder builder, Dictionary<string, int> startingStock,
            BoosterDefinition definition, Func<BoosterInventory, IBooster> create)
        {
            startingStock.Add(definition.Id, definition.StartingStock);
            builder.Register(resolver => create(resolver.Resolve<BoosterInventory>()), Lifetime.Scoped);
        }
    }
}
