using Core.Installers;
using Core.Levels;
using UnityEngine;
using VContainer;

namespace Game.Levels
{
    [CreateAssetMenu(menuName = "Game/Levels/Levels Installer")]
    public sealed class GameLevelsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private LevelCatalog _catalog;

        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_catalog);
            builder.RegisterCoreLevels(new LevelLoop(_catalog.Levels.Count, 0));
            builder.Register<LevelRuns>(Lifetime.Singleton);
            builder.Register<EditorPlayRequest>(Lifetime.Singleton);
        }
    }
}
