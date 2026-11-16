using System.Collections.Generic;
using VContainer;
using VContainer.Unity;

namespace Core.UI
{
    public static class UIInstaller
    {
        // Once per scope; screens are created through the container, so they can receive game services with [Inject].
        public static void RegisterCoreUI(this IContainerBuilder builder, UIRoot root) =>
            builder.Register<IUIService>(
                resolver => new UIService(
                    resolver.Resolve<IReadOnlyList<UIScreenCatalog>>(),
                    root,
                    (prefab, parent) => resolver.Instantiate(prefab, parent)),
                Lifetime.Singleton);

        // Once per feature that brings screens.
        public static void RegisterUIScreens(this IContainerBuilder builder, UIScreenCatalog catalog) =>
            builder.RegisterInstance(catalog);
    }
}
