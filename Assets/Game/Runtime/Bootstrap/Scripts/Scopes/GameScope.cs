using Core.Installers;
using Game.Play;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class GameScope : LifetimeScope
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private BoardRoot _board;
        [SerializeField] private ScriptableObjectInstaller[] _installers;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_camera);
            builder.RegisterComponent(_board);
            foreach (var installer in _installers)
                installer.Install(builder);
            builder.RegisterEntryPoint<GameEntry>();
        }
    }
}
