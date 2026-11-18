using Game.Data.Levels;
using Game.Editor.LevelEditor.Preview;
using Game.Play.Presentation;
using Game.Tests.Levels;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Tests.LevelEditor
{
    public sealed class LevelScenePreviewTests
    {
        private const string GameScenePath = "Assets/Game/Runtime/Scenes/Game.unity";
        private const int Red = 1;

        private Scene _scene;
        private LevelScenePreview _preview;

        [SetUp]
        public void OpenGameScene()
        {
            Assume.That(SceneManager.GetSceneByPath(GameScenePath).isLoaded, Is.False,
                "Close the Game scene first: the test opens and closes it, and the editor's own preview would be in it.");
            _scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Additive);
            _preview = new LevelScenePreview();
        }

        [TearDown]
        public void CloseGameScene()
        {
            if (_preview == null)
                return;
            _preview.Clear();
            EditorSceneManager.CloseScene(_scene, true);
        }

        [Test]
        public void Show_BuildsTheLevelInTheGameScene_WithoutChangingTheScene()
        {
            var level = new LevelBuilder(3, 3).Rect(Red, 0, 0, 2, 1).Rect(Red, 0, 2, 1, 1).Door(Red, Side.Top, 0, 2).Build();

            _preview.Show(level);

            var root = FindRoot();
            Assert.IsNotNull(root, "the preview is in the Game scene");
            Assert.AreEqual(2, root.GetComponentsInChildren<BlockView>().Length);
            foreach (var child in root.GetComponentsInChildren<Transform>())
                Assert.IsTrue((child.gameObject.hideFlags & HideFlags.DontSave) == HideFlags.DontSave, child.name);
            Assert.IsFalse(_scene.isDirty, "the preview does not make the scene need saving");
        }

        [Test]
        public void Show_ReplacesTheLevelShownBefore_AndClearRemovesIt()
        {
            _preview.Show(new LevelBuilder(3, 3).Rect(Red, 0, 0, 1, 1).Build());
            _preview.Show(new LevelBuilder(4, 4).Rect(Red, 0, 0, 1, 1).Rect(Red, 2, 2, 1, 1).Build());

            Assert.AreEqual(2, FindRoot().GetComponentsInChildren<BlockView>().Length);
            _preview.Clear();
            Assert.IsNull(FindRoot());
        }

        private GameObject FindRoot()
        {
            GameObject found = null;
            foreach (var root in _scene.GetRootGameObjects())
            {
                if (root.name == LevelScenePreview.RootName)
                {
                    Assert.IsNull(found, "only one preview");
                    found = root;
                }
            }
            return found;
        }
    }
}
