using System;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Core.Scenes.Editor
{
    [InitializeOnLoad]
    internal static class PlayFromFirstScene
    {
        // A content scene started on its own would have no parent scope.
        static PlayFromFirstScene()
        {
            EditorBuildSettings.sceneListChanged += Apply;
            Apply();
        }

        private static void Apply()
        {
            EditorBuildSettingsScene first = Array.Find(EditorBuildSettings.scenes, scene => scene.enabled);
            EditorSceneManager.playModeStartScene = first == null ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(first.path);
        }
    }
}
