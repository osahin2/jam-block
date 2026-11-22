using System;
using Core.Save;
using VContainer;

namespace Core.Audio
{
    public static class AudioInstaller
    {
        // In the root scope, after RegisterCoreSave.
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
