using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace Core.Scenes.Tests
{
    public sealed class SceneServiceTests
    {
        private List<string> _log;
        private FakeLoader _loader;
        private FakeTransition _transition;
        private SceneService _service;

        [SetUp]
        public void SetUp()
        {
            _log = new List<string>();
            _loader = new FakeLoader(_log);
            _transition = new FakeTransition(_log);
            _service = new SceneService(_loader, _transition);
        }

        [Test]
        public void FirstSwitch_CoversLoadsAndUncovers_WithoutUnloading()
        {
            Switch("A");

            CollectionAssert.AreEqual(new[] { "cover", "progress 0", "load A", "progress 1", "uncover" }, _log);
            Assert.AreEqual("A", _service.CurrentScene);
        }

        [Test]
        public void NextSwitch_UnloadsCurrentSceneBeforeLoading()
        {
            Switch("A");
            _log.Clear();

            Switch("B");

            CollectionAssert.AreEqual(new[] { "cover", "unload A", "progress 0", "load B", "progress 1", "uncover" }, _log);
            Assert.AreEqual("B", _service.CurrentScene);
        }

        [Test]
        public void SwitchWhileSwitching_Throws()
        {
            var pendingCover = new UniTaskCompletionSource();
            _transition.Cover = pendingCover.Task;
            UniTask first = _service.SwitchToAsync("A", CancellationToken.None);

            Assert.Throws<InvalidOperationException>(() => Switch("B"));

            pendingCover.TrySetResult();
            first.GetAwaiter().GetResult();
            Assert.AreEqual("A", _service.CurrentScene);
        }

        [Test]
        public void FailedLoad_AllowsTheNextSwitch()
        {
            _loader.FailNextLoad = true;
            Assert.Throws<ArgumentException>(() => Switch("Missing"));

            Switch("A");

            Assert.AreEqual("A", _service.CurrentScene);
        }

        // Fakes complete synchronously, so the whole switch has finished when this returns.
        private void Switch(string sceneName) => _service.SwitchToAsync(sceneName, CancellationToken.None).GetAwaiter().GetResult();

        private sealed class FakeLoader : ISceneLoader
        {
            private readonly List<string> _log;

            public bool FailNextLoad { get; set; }

            public FakeLoader(List<string> log)
            {
                _log = log;
            }

            public UniTask LoadAsync(string sceneName, IProgress<float> progress, CancellationToken cancellationToken)
            {
                if (FailNextLoad)
                {
                    FailNextLoad = false;
                    throw new ArgumentException(sceneName);
                }

                _log.Add("load " + sceneName);
                progress.Report(1f);
                return UniTask.CompletedTask;
            }

            public UniTask UnloadAsync(string sceneName, CancellationToken cancellationToken)
            {
                _log.Add("unload " + sceneName);
                return UniTask.CompletedTask;
            }
        }

        private sealed class FakeTransition : ISceneTransition
        {
            private readonly List<string> _log;

            public UniTask Cover { get; set; } = UniTask.CompletedTask;

            public FakeTransition(List<string> log)
            {
                _log = log;
            }

            public UniTask CoverAsync(CancellationToken cancellationToken)
            {
                _log.Add("cover");
                return Cover;
            }

            public void ReportProgress(float progress) => _log.Add("progress " + progress);

            public UniTask UncoverAsync(CancellationToken cancellationToken)
            {
                _log.Add("uncover");
                return UniTask.CompletedTask;
            }
        }
    }
}
