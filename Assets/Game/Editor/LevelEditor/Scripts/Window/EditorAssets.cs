using UnityEditor;
using UnityEngine;

namespace Game.Editor.LevelEditor
{
    // Editor tools run outside the game's scopes, so they find the settings assets they need in the project.
    internal static class EditorAssets
    {
        public static T FindFirst<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            return guids.Length == 0 ? null : AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
