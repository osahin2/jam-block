using Core.Audio;
using Core.Haptics;
using Core.Installers;
using Core.Save;
using Core.Scenes;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class BootScope : LifetimeScope
    {
        [SerializeField] private AudioServiceSettings _audio;
        [SerializeField] private ScreenFadeTransition _transition;
        [SerializeField] private ScriptableObjectInstaller[] _installers;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterCoreSave();
            builder.RegisterCoreAudio(_audio);
            builder.RegisterCoreHaptics();
            builder.RegisterCoreScenes(_transition);
            // The Game scene covers a level change with the same fade as a scene switch.
            builder.RegisterInstance<ISceneTransition>(_transition);
            foreach (var installer in _installers)
                installer.Install(builder);
            builder.Register<SceneFlow>(Lifetime.Singleton);
            builder.RegisterEntryPoint<BootEntry>();
        }
    }
}
