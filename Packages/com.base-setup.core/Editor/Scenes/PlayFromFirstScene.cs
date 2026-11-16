using System;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Core.Scenes.Editor
{
    // Play mode always starts from the first enabled build scene (the boot scene with the root scope), so pressing Play
    // with a content scene open does not start that scene without its parent scope.
    [InitializeOnLoad]
    internal static class PlayFromFirstScene
    {
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
