using VContainer;

namespace Core.Levels
{
    // Needs ISaveService in the same container or a parent (RegisterCoreSave).
    public static class LevelsInstaller
    {
        public static void RegisterCoreLevels(this IContainerBuilder builder, LevelLoop loop)
        {
            builder.RegisterInstance(loop);
            builder.Register<ILevelProgression, LevelProgression>(Lifetime.Singleton);
        }
    }
}
