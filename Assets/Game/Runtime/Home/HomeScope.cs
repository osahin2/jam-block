using Core.Installers;
using Core.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Home
{
    // The Home scene's scope, a child of the Boot scope.
    public sealed class HomeScope : LifetimeScope
    {
        [SerializeField] private UIRoot _uiRoot;
        [SerializeField] private ScriptableObjectInstaller[] _installers;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterCoreUI(_uiRoot);
            foreach (var installer in _installers)
                installer.Install(builder);
        }
    }
}
