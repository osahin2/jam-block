using Game.Levels;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Game.Editor.LevelEditor
{
    internal static class LevelPlayStarter
    {
        public static bool TryPlay(LevelAsset level, out string problem)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                problem = "The game is already playing. Stop it first.";
            else if (EditorUtility.scriptCompilationFailed)
                problem = "Scripts have compile errors. Fix them in the Console first.";
            else if (EditorSceneManager.playModeStartScene == null)
                problem = "No scene to start from. Add the game's scenes in File > Build Settings.";
            else
                problem = null;
            if (problem != null)
                return false;

            new EditorPlayRequest().Request(level);
            EditorApplication.EnterPlaymode();
            return true;
        }
    }
}
