using VContainer;

namespace Core.Scenes
{
    public static class ScenesInstaller
    {
        // In the root scope, whose scene stays loaded. Without a transition, scenes switch with nothing covering them.
        public static void RegisterCoreScenes(this IContainerBuilder builder, ISceneTransition transition = null) =>
            builder.RegisterInstance<ISceneService>(new SceneService(new UnitySceneLoader(), transition ?? new NoSceneTransition()));
    }
}
