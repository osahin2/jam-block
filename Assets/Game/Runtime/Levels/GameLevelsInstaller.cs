using Core.Installers;
using Game.Levels.Settings;
using UnityEngine;
using VContainer;

namespace Game.Levels
{
    [CreateAssetMenu(menuName = "Game/Levels/Levels Installer")]
    public sealed class GameLevelsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private LevelCatalog _catalog;

        public override void Install(IContainerBuilder builder) => builder.RegisterInstance(_catalog);
    }
}
