using Game.Editor.LevelEditor;
using Game.Levels;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.LevelEditor
{
    public sealed class LevelLibraryTests
    {
        private const string Folder = "Assets/LevelLibraryTests";

        private LevelCatalog _catalog;
        private LevelLibrary _library;

        [SetUp]
        public void CreateCatalog()
        {
            AssetDatabase.CreateFolder("Assets", "LevelLibraryTests");
            _catalog = ScriptableObject.CreateInstance<LevelCatalog>();
            AssetDatabase.CreateAsset(_catalog, $"{Folder}/Catalog.asset");
            _library = new LevelLibrary(_catalog);
        }

        [TearDown]
        public void DeleteFolder() => AssetDatabase.DeleteAsset(Folder);

        [Test]
        public void Create_SavesAnAssetUnderTheFirstName_AndAppendsItToTheCatalog()
        {
            var level = _library.Create();

            Assert.AreEqual($"{Folder}/Level_01.asset", AssetDatabase.GetAssetPath(level));
            CollectionAssert.AreEqual(new[] { level }, _catalog.Levels);
        }

        [Test]
        public void Create_Twice_NumbersTheLevelsInPlayOrder()
        {
            var first = _library.Create();
            var second = _library.Create();

            Assert.AreEqual("Level_02", second.name);
            CollectionAssert.AreEqual(new[] { first, second }, _catalog.Levels);
        }

        [Test]
        public void Create_AfterTheLevelsWereMoved_SavesTheNewLevelNextToTheLastOne()
        {
            AssetDatabase.CreateFolder(Folder, "Moved");
            var first = _library.Create();
            AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(first), $"{Folder}/Moved/Level_01.asset");

            var second = _library.Create();

            Assert.AreEqual($"{Folder}/Moved/Level_02.asset", AssetDatabase.GetAssetPath(second));
        }

        [Test]
        public void Create_WhenTheLastEntryIsEmpty_UsesTheLastLevelBeforeIt()
        {
            AssetDatabase.CreateFolder(Folder, "Moved");
            var first = _library.Create();
            var deleted = _library.Create();
            AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(first), $"{Folder}/Moved/Level_01.asset");
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(deleted));

            var third = _library.Create();

            Assert.AreEqual($"{Folder}/Moved/Level_03.asset", AssetDatabase.GetAssetPath(third));
        }

        [Test]
        public void Move_ChangesThePlayOrder_AndSavesTheCatalog()
        {
            var first = _library.Create();
            var second = _library.Create();
            var third = _library.Create();

            _library.Move(2, 0);

            CollectionAssert.AreEqual(new[] { third, first, second }, _catalog.Levels);
            Assert.IsFalse(EditorUtility.IsDirty(_catalog));
        }

        [Test]
        public void Delete_TakesTheEntryOut_AndMovesTheFileToTheTrash()
        {
            var first = _library.Create();
            var second = _library.Create();
            var third = _library.Create();
            var path = AssetDatabase.GetAssetPath(second);

            _library.Delete(1);

            CollectionAssert.AreEqual(new[] { first, third }, _catalog.Levels, "no empty entry is left");
            Assert.IsEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets), "the file is gone");
        }

        [Test]
        public void Delete_EntryWhoseAssetWasDeleted_TakesOnlyTheEntryOut()
        {
            var first = _library.Create();
            var deleted = _library.Create();
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(deleted));

            _library.Delete(1);

            CollectionAssert.AreEqual(new[] { first }, _catalog.Levels);
        }

        [Test]
        public void NextName_SkipsANameAFileInTheFolderAlreadyHas()
        {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LevelAsset>(), $"{Folder}/Level_01.asset");

            Assert.AreEqual("Level_02", _library.NextName());
        }

        [Test]
        public void Create_SavesTheCatalog()
        {
            var level = _library.Create();

            Assert.IsFalse(EditorUtility.IsDirty(_catalog), "the catalog still has unsaved changes");
            Assert.AreSame(level, _library.Levels[0]);
        }
    }
}
