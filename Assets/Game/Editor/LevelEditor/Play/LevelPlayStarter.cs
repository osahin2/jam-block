using Game.Levels;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Game.Editor.LevelEditor.Play
{
    // Plays a level from the scene a build starts with, leaving the scenes open in the editor as they are.
    // A static hook because the start scene must be cleared after the domain reload that entering play mode brings,
    // when no window object from before is left; only the level editor window calls it.
    [InitializeOnLoad]
    internal static class LevelPlayStarter
    {
        static LevelPlayStarter() => EditorApplication.playModeStateChanged += OnPlayModeChanged;

        // Checks everything that would stop play mode before writing anything, so a refused play leaves no request and
        // no start scene behind for Unity's own Play button. The problem is a sentence for the designer.
        public static bool TryPlay(LevelAsset level, out string problem)
        {
            var startScene = FirstBuildScene();
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                problem = "The game is already playing. Stop it first.";
            else if (EditorUtility.scriptCompilationFailed)
                problem = "Scripts have compile errors. Fix them in the Console first.";
            else if (startScene == null)
                problem = "No scene to start from. Add the game's scenes in File > Build Settings.";
            else
                problem = null;
            if (problem != null)
                return false;

            new EditorPlayRequest().Request(level);
            EditorSceneManager.playModeStartScene = startScene;
            EditorApplication.EnterPlaymode();
            return true;
        }

        // The start scene is for this play only; Play in the toolbar starts from the open scene again.
        private static void OnPlayModeChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
                EditorSceneManager.playModeStartScene = null;
        }

        private static SceneAsset FirstBuildScene()
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                    return AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
            }
            return null;
        }
    }
}
