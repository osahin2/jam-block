using Game.Data.Levels;
using Game.Editor.LevelEditor.Levels;
using Game.Editor.LevelEditor.Window;
using Game.Levels;
using Game.Levels.Settings;
using Game.Tests.Levels;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.LevelEditor
{
    // Saves into real assets in a folder of its own with its own catalog, removed after each test.
    public sealed class LevelDocumentTests
    {
        private const string Folder = "Assets/LevelDocumentTests";
        private const int Red = 1;

        private LevelCatalog _catalog;
        private LevelLibrary _library;
        private LevelDocument _document;

        [SetUp]
        public void CreateDocument()
        {
            AssetDatabase.CreateFolder("Assets", "LevelDocumentTests");
            _catalog = ScriptableObject.CreateInstance<LevelCatalog>();
            AssetDatabase.CreateAsset(_catalog, $"{Folder}/Catalog.asset");
            _library = new LevelLibrary(_catalog);
            _document = new LevelDocument();
            _document.EnsureDraft();
        }

        [TearDown]
        public void Delete()
        {
            _document.Close();
            AssetDatabase.DeleteAsset(Folder);
        }

        [Test]
        public void NothingOpen_HasNothingToSaveOrRevert()
        {
            Assert.IsFalse(_document.IsOpen);
            Assert.IsFalse(_document.HasUnsavedChanges);
            Assert.IsFalse(_document.CanRevert);
            Assert.IsFalse(_document.TrySave(_library));
            _document.Duplicate();
            Assert.IsFalse(_document.IsOpen, "there is nothing to copy");
        }

        [Test]
        public void StartNew_OpensAnEmptyLevelWithoutAnAsset_UnderTheNextName()
        {
            _document.StartNew();

            Assert.IsTrue(_document.IsOpen);
            Assert.IsNull(_document.Asset);
            Assert.AreEqual("Level_01 (new)", _document.Name(_library));
            Assert.IsFalse(_document.HasUnsavedChanges, "an untouched new level asks nothing when it is left");
            Assert.IsFalse(_document.CanRevert);
        }

        [Test]
        public void NewLevel_WithAChange_IsUnsaved_ButCannotBeReverted()
        {
            _document.StartNew();
            _document.Draft.Level.TimeLimitSeconds = 60;

            Assert.IsTrue(_document.HasUnsavedChanges);
            Assert.IsFalse(_document.CanRevert);
        }

        [Test]
        public void TrySave_NewLevelWithErrors_CreatesNoAsset()
        {
            _document.StartNew();

            Assert.IsFalse(_document.TrySave(_library), "an empty board has no blocks");
            Assert.IsNull(_document.Asset);
            Assert.IsEmpty(_catalog.Levels);
        }

        [Test]
        public void TrySave_PlayableNewLevel_BecomesTheCatalogsNextLevel()
        {
            _document.StartNew();
            DrawPlayableLevel();

            Assert.IsTrue(_document.TrySave(_library));

            Assert.AreEqual($"{Folder}/Level_01.asset", AssetDatabase.GetAssetPath(_document.Asset));
            CollectionAssert.AreEqual(new[] { _document.Asset }, _catalog.Levels);
            Assert.IsTrue(_document.Draft.Matches(_document.Asset.Level));
            Assert.AreEqual("Level_01", _document.Name(_library));
            Assert.IsFalse(_document.HasUnsavedChanges);
        }

        [Test]
        public void NewLevel_WithoutALibrary_IsNamedButNotSaved()
        {
            _document.StartNew();
            DrawPlayableLevel();

            Assert.AreEqual("New level", _document.Name(null));
            Assert.IsFalse(_document.TrySave(null));
            Assert.IsNull(_document.Asset);
        }

        [Test]
        public void OpenLevel_WithAChange_CanBeRevertedByOpeningItAgain()
        {
            var asset = SavedPlayableLevel();
            _document.Open(asset);
            Assert.IsFalse(_document.HasUnsavedChanges);

            _document.Draft.Level.TimeLimitSeconds = 60;
            Assert.IsTrue(_document.HasUnsavedChanges);
            Assert.IsTrue(_document.CanRevert);

            _document.Open(asset);
            Assert.IsFalse(_document.HasUnsavedChanges);
        }

        [Test]
        public void TrySave_OpenLevel_WritesIntoItsAsset()
        {
            var asset = SavedPlayableLevel();
            _document.Open(asset);
            _document.Draft.Level.TimeLimitSeconds = 60;

            Assert.IsTrue(_document.TrySave(_library));

            Assert.AreEqual(60, asset.Level.TimeLimitSeconds);
            CollectionAssert.AreEqual(new[] { asset }, _catalog.Levels, "saving an open level adds nothing to the catalog");
        }

        [Test]
        public void Duplicate_OpenLevel_StartsANewLevelWithItsContentAsInTheWindow()
        {
            var original = SavedPlayableLevel();
            _document.Open(original);
            _document.Draft.Level.TimeLimitSeconds = 60;

            _document.Duplicate();

            Assert.IsNull(_document.Asset);
            Assert.AreEqual("Level_02 (new)", _document.Name(_library));
            Assert.AreEqual(60, _document.Draft.Level.TimeLimitSeconds, "unsaved changes go with the copy");
            Assert.AreEqual(original.Level.Blocks.Count, _document.Draft.Level.Blocks.Count);
            Assert.IsTrue(_document.HasUnsavedChanges, "a copy is not saved yet");
            Assert.IsFalse(_document.CanRevert);
        }

        [Test]
        public void Duplicate_ThenSave_AddsALevelOfItsOwn_AndLeavesTheOriginal()
        {
            var original = SavedPlayableLevel();
            _document.Open(original);
            _document.Draft.Level.TimeLimitSeconds = 60;
            _document.Duplicate();

            Assert.IsTrue(_document.TrySave(_library));

            Assert.AreNotSame(original, _document.Asset);
            Assert.AreEqual("Level_02", _document.Name(_library));
            CollectionAssert.AreEqual(new[] { original, _document.Asset }, _catalog.Levels);
            Assert.AreEqual(60, _document.Asset.Level.TimeLimitSeconds);
            Assert.AreNotEqual(60, original.Level.TimeLimitSeconds, "the original keeps what was last saved");
        }

        [Test]
        public void Unload_LeavesNothingOpen()
        {
            _document.Open(SavedPlayableLevel());
            _document.Draft.Level.TimeLimitSeconds = 60;

            _document.Unload();

            Assert.IsFalse(_document.IsOpen);
            Assert.IsNull(_document.Asset);
            Assert.IsFalse(_document.HasUnsavedChanges);
        }

        // Stands in for drawing a block and its door on the board.
        private void DrawPlayableLevel() => _document.Draft.Load(PlayableLevel());

        private LevelAsset SavedPlayableLevel()
        {
            _document.StartNew();
            DrawPlayableLevel();
            _document.TrySave(_library);
            return _document.Asset;
        }

        private static LevelData PlayableLevel() =>
            new LevelBuilder(3, 3).Rect(Red, 0, 1, 2, 1).Door(Red, Side.Top, 0, 2).Build();
    }
}
