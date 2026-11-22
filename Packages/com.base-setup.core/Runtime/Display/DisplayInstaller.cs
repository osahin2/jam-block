using System;
using VContainer;

namespace Core.Display
{
    public static class DisplayInstaller
    {
        // In the root scope; the settings are applied once, when the container is built.
        public static void RegisterCoreDisplay(this IContainerBuilder builder, DisplaySettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            builder.RegisterBuildCallback(_ => settings.Apply());
        }
    }
}
