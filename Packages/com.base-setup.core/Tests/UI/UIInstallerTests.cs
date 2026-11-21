using System.Collections.Generic;
using NUnit.Framework;
using VContainer;
using Object = UnityEngine.Object;

namespace Core.UI.Tests
{
    public sealed class UIInstallerTests
    {
        // Each feature registers its own catalog, so one scope often holds several.
        [Test]
        public void SeveralCatalogsInOneScope_AllReachTheService()
        {
            UIScreenCatalog first = UIScreenCatalog.Create("First");
            UIScreenCatalog second = UIScreenCatalog.Create("Second");
            var builder = new ContainerBuilder();
            builder.RegisterUIScreens(first);
            builder.RegisterUIScreens(second);

            using (IObjectResolver container = builder.Build())
            {
                IReadOnlyList<UIScreenCatalog> catalogs = container.Resolve<IReadOnlyList<UIScreenCatalog>>();

                Assert.That(catalogs, Is.EquivalentTo(new[] { first, second }));
            }

            Object.DestroyImmediate(first);
            Object.DestroyImmediate(second);
        }
    }
}
