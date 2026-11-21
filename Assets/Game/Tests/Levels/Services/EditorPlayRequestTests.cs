using Game.Levels;
using NUnit.Framework;
using UnityEditor;

namespace Game.Tests.Levels
{
    public sealed class EditorPlayRequestTests
    {
        private EditorPlayRequest _request;

        [SetUp]
        public void CreateRequest() => _request = new EditorPlayRequest();

        // Leaves no request behind for the next play mode.
        [TearDown]
        public void TakeAnyRequest() => _request.TryTake(out _);

        [Test]
        public void TryTake_AfterARequest_GivesTheRequestedLevel()
        {
            var level = AnyLevelAsset();
            _request.Request(level);

            Assert.IsTrue(_request.TryTake(out var taken));
            Assert.AreSame(level, taken);
        }

        [Test]
        public void IsPending_IsTrueFromARequestUntilItIsTaken()
        {
            Assert.IsFalse(_request.IsPending);
            _request.Request(AnyLevelAsset());

            Assert.IsTrue(_request.IsPending);
            Assert.IsTrue(_request.IsPending, "looking does not take the request");
            _request.TryTake(out _);
            Assert.IsFalse(_request.IsPending);
        }

        [Test]
        public void TryTake_GivesARequestOnlyOnce()
        {
            _request.Request(AnyLevelAsset());
            _request.TryTake(out _);

            Assert.IsFalse(_request.TryTake(out var taken));
            Assert.IsNull(taken);
        }

        [Test]
        public void TryTake_ReadsWhatAnotherInstanceRequested()
        {
            var level = AnyLevelAsset();
            new EditorPlayRequest().Request(level);

            Assert.IsTrue(_request.TryTake(out var taken), "the request outlives the object that made it, as across a domain reload");
            Assert.AreSame(level, taken);
        }

        private static LevelAsset AnyLevelAsset()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(LevelAsset)}");
            Assert.IsNotEmpty(guids, "No level assets found.");
            return AssetDatabase.LoadAssetAtPath<LevelAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
