using System;
using Core.Save;
using VContainer;

namespace Core.Haptics
{
    public static class HapticInstaller
    {
        // In the root scope, after RegisterCoreSave.
        // Uses the platform's native backend (iOS, Android 8+; silent in the editor and on other platforms).
        public static void RegisterCoreHaptics(this IContainerBuilder builder) =>
            Register(builder, NativeHapticBackend.Create());

        // Uses the game's backend instead, for example an adapter over a third-party haptics package.
        public static void RegisterCoreHaptics(this IContainerBuilder builder, IHapticBackend backend)
        {
            if (backend == null)
                throw new ArgumentNullException(nameof(backend));

            Register(builder, backend);
        }

        private static void Register(IContainerBuilder builder, IHapticBackend backend) =>
            builder.Register<IHapticService>(
                resolver => new HapticService(resolver.Resolve<ISaveService>(), backend),
                Lifetime.Singleton);
    }
}
