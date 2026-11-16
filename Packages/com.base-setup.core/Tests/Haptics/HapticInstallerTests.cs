using System;
using Core.Save;
using NUnit.Framework;
using VContainer;

namespace Core.Haptics.Tests
{
    public sealed class HapticInstallerTests
    {
        [Test]
        public void AGameBackend_ReceivesThePresets()
        {
            var backend = new RecordingBackend();
            using IObjectResolver container = Build(builder => builder.RegisterCoreHaptics(backend));

            container.Resolve<IHapticService>().Play(HapticPreset.Medium);

            Assert.That(backend.Played, Is.EqualTo(new[] { HapticPreset.Medium }));
        }

        [Test]
        public void ANullBackend_Throws()
        {
            var builder = new ContainerBuilder();

            Assert.Throws<ArgumentNullException>(() => builder.RegisterCoreHaptics(null));
        }

        [Test]
        public void TheNativeRegistration_ResolvesOneService()
        {
            using IObjectResolver container = Build(builder => builder.RegisterCoreHaptics());

            IHapticService service = container.Resolve<IHapticService>();

            Assert.That(service, Is.SameAs(container.Resolve<IHapticService>()));
            Assert.DoesNotThrow(() => service.Play(HapticPreset.Heavy));
        }

        [Test]
        public void InTheEditor_TheNativeBackendIsSilent()
        {
            Assert.That(NativeHapticBackend.Create(), Is.TypeOf<NoHapticBackend>());
        }

        private static IObjectResolver Build(Action<IContainerBuilder> register)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance<ISaveService>(new MemorySave());
            register(builder);
            return builder.Build();
        }
    }
}
