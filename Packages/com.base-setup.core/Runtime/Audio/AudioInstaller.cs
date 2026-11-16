using System;
using Core.Save;
using VContainer;

namespace Core.Audio
{
    // In the root scope, after RegisterCoreSave. The audio objects are created when IAudioService is first resolved.
    public static class AudioInstaller
    {
        public static void RegisterCoreAudio(this IContainerBuilder builder, AudioServiceSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            builder.Register<IAudioService>(
                resolver => AudioServiceFactory.Create(settings, resolver.Resolve<ISaveService>()),
                Lifetime.Singleton);
        }
    }
}
