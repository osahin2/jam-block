using System;
using System.Collections.Generic;
using System.IO;
using Game.Levels;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Editor.LevelEditor
{
    public sealed class LevelLibrary
    {
        private const string LevelsProperty = "_levels";

        private readonly LevelCatalog _catalog;

        // May hold empty entries, left by a level asset deleted in the Project window.
        public IReadOnlyList<LevelAsset> Levels => _catalog.Levels;

        public LevelLibrary(LevelCatalog catalog) => _catalog = catalog;

        public string NextName() => Path.GetFileNameWithoutExtension(NextPath());

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

        public void Move(int from, int to) => ChangeCatalog(levels => levels.MoveArrayElement(from, to));

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

        private void ChangeCatalog(Action<SerializedProperty> change)
        {
            var catalog = new SerializedObject(_catalog);
            change(catalog.FindProperty(LevelsProperty));
            catalog.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssetIfDirty(_catalog);
        }

        private static string PathOf(string folder, int number) => $"{folder}/Level_{number:00}.asset";

        private static bool Exists(string path) =>
            !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets));

        private static string FolderOf(Object asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            return path.Substring(0, path.LastIndexOf('/'));
        }

        private string NextPath()
        {
            var folder = Folder();
            var number = _catalog.Levels.Count + 1;
            while (Exists(PathOf(folder, number)))
                number++;
            return PathOf(folder, number);
        }

        // Next to the catalog's last level, so moving the levels' folder needs no change here.
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
