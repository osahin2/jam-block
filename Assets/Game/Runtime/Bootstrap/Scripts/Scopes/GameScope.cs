using Core.Installers;
using Core.UI;
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
        [SerializeField] private BoardInputView _input;
        [SerializeField] private UIRoot _uiRoot;
        [SerializeField] private ScriptableObjectInstaller[] _installers;

        public Camera Camera => _camera;
        public BoardRoot Board => _board;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterCoreUI(_uiRoot);
            builder.RegisterComponent(_camera);
            builder.RegisterComponent(_board);
            builder.RegisterComponent(_input);
            foreach (var installer in _installers)
                installer.Install(builder);
            builder.RegisterEntryPoint<GameEntry>();
        }
    }
}
