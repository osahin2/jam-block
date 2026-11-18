using Core.Installers;
using Core.Levels;
using Game.Levels.Settings;
using UnityEngine;
using VContainer;

namespace Game.Levels
{
    // In the Boot scope: the catalog and the player's progress through it are the same in every scene.
    [CreateAssetMenu(menuName = "Game/Levels/Levels Installer")]
    public sealed class GameLevelsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private LevelCatalog _catalog;

        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_catalog);
            builder.RegisterCoreLevels(new LevelLoop(_catalog.Levels.Count, 0));
            builder.Register<LevelSequence>(Lifetime.Singleton);
            builder.Register<EditorPlayRequest>(Lifetime.Singleton);
        }
    }
}
