using System;
using NUnit.Framework;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Core.Display.Tests
{
    public sealed class DisplayInstallerTests
    {
        private int _frameRate;
        private int _sleepTimeout;
        private DisplaySettings _settings;

        [SetUp]
        public void SetUp()
        {
            _frameRate = Application.targetFrameRate;
            _sleepTimeout = Screen.sleepTimeout;
            _settings = ScriptableObject.CreateInstance<DisplaySettings>();
            Application.targetFrameRate = -1;
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }

        [TearDown]
        public void TearDown()
        {
            Application.targetFrameRate = _frameRate;
            Screen.sleepTimeout = _sleepTimeout;
            Object.DestroyImmediate(_settings);
        }

        [Test]
        public void BuildingTheContainer_AppliesTheDefaults()
        {
            using IObjectResolver container = Build();

            Assert.That(Application.targetFrameRate, Is.EqualTo(60));
            Assert.That(Screen.sleepTimeout, Is.EqualTo(SleepTimeout.NeverSleep));
        }

        [Test]
        public void RegisteringAlone_ChangesNothing()
        {
            var builder = new ContainerBuilder();

            builder.RegisterCoreDisplay(_settings);

            Assert.That(Application.targetFrameRate, Is.EqualTo(-1));
        }

        [Test]
        public void NullSettings_Throw()
        {
            var builder = new ContainerBuilder();

            Assert.Throws<ArgumentNullException>(() => builder.RegisterCoreDisplay(null));
        }

        private IObjectResolver Build()
        {
            var builder = new ContainerBuilder();
            builder.RegisterCoreDisplay(_settings);
            return builder.Build();
        }
    }
}
