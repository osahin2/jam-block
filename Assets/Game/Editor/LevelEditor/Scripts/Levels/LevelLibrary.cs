using System;
using System.Collections.Generic;
using System.IO;
using Game.Levels;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Editor.LevelEditor
{
    // The game's levels as the editor sees them: the catalog in play order, and where a new level is saved.
    public sealed class LevelLibrary
    {
        private const string LevelsProperty = "_levels";

        private readonly LevelCatalog _catalog;

        // May hold empty entries, left by a level asset deleted in the Project window.
        public IReadOnlyList<LevelAsset> Levels => _catalog.Levels;

        public LevelLibrary(LevelCatalog catalog) => _catalog = catalog;

        public string NextName() => Path.GetFileNameWithoutExtension(NextPath());

        // Creates an empty level asset under the next name and appends it to the catalog; the caller writes the level.
        public LevelAsset Create()
        {
            var asset = ScriptableObject.CreateInstance<LevelAsset>();
            AssetDatabase.CreateAsset(asset, NextPath());
            ChangeCatalog(levels =>
            {
                levels.arraySize++;
                levels.GetArrayElementAtIndex(levels.arraySize - 1).objectReferenceValue = asset;
            });
            return asset;
        }

        // Changes the play order: the level at from ends up at to.
        public void Move(int from, int to) => ChangeCatalog(levels => levels.MoveArrayElement(from, to));

        // Takes the entry out of the catalog, leaving no empty entry, and moves its level's file to the trash, so a level
        // deleted by mistake can be brought back from there. An entry whose asset was deleted elsewhere is only taken out.
        public void Delete(int index)
        {
            var level = _catalog.Levels[index];
            ChangeCatalog(levels =>
            {
                // Moved to the end and cut off, the same for object references in every Unity version.
                levels.MoveArrayElement(index, levels.arraySize - 1);
                levels.arraySize--;
            });
            if (level != null)
                AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(level));
        }

        // Through the serialized catalog, the editor's way to change an asset, saved at once.
        private void ChangeCatalog(Action<SerializedProperty> change)
        {
            var catalog = new SerializedObject(_catalog);
            change(catalog.FindProperty(LevelsProperty));
            catalog.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssetIfDirty(_catalog);
        }

        private static string PathOf(string folder, int number) => $"{folder}/Level_{number:00}.asset";

        // Asks the asset database without loading the asset.
        private static bool Exists(string path) =>
            !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets));

        private static string FolderOf(Object asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            return path.Substring(0, path.LastIndexOf('/'));
        }

        // The number after the catalog's last level, skipping names a file in the folder already has.
        private string NextPath()
        {
            var folder = Folder();
            var number = _catalog.Levels.Count + 1;
            while (Exists(PathOf(folder, number)))
                number++;
            return PathOf(folder, number);
        }

        // Next to the catalog's last level, so moving the levels' folder needs no change here; the catalog's own folder
        // while it lists no level.
        private string Folder()
        {
            var levels = _catalog.Levels;
            for (var i = levels.Count - 1; i >= 0; i--)
            {
                if (levels[i] != null)
                    return FolderOf(levels[i]);
            }
            return FolderOf(_catalog);
        }
    }
}
