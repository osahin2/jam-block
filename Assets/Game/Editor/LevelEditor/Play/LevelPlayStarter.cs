using Game.Levels;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Game.Editor.LevelEditor.Play
{
    // Plays a level from the scene a build starts with, leaving the scenes open in the editor as they are. Core.Scenes
    // keeps the play mode start scene on the first build scene (Boot), which sees the request and opens the game.
    internal static class LevelPlayStarter
    {
        // Checks everything that would stop play mode before writing anything, so a refused play leaves no request
        // behind. The problem is a sentence for the designer.
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
