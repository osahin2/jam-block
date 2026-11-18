using Game.Data.Levels;
using Game.Levels;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Game.Editor.LevelEditor.Preview
{
    // Decides which level the scene preview shows: the level open in the level editor, live; otherwise the level
    // asset last selected in the Project window. Changes are gathered into one rebuild per editor update. In play mode
    // the game builds the level itself, so the preview is removed.
    // A static hook because it must follow the Project selection while no level editor window is open; only the
    // window calls it.
    [InitializeOnLoad]
    internal static class LevelPreviewSource
    {
        private static readonly LevelScenePreview Preview = new LevelScenePreview();
        private static LevelData _edited;
        private static LevelAsset _selected;
        private static bool _rebuildRequested;

        static LevelPreviewSource()
        {
            Selection.selectionChanged += OnSelectionChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorSceneManager.sceneOpened += (_, _) => RequestRebuild();
            AssemblyReloadEvents.beforeAssemblyReload += Preview.Clear;
            OnSelectionChanged();
        }

        public static void ShowEdited(LevelData level)
        {
            _edited = level;
            RequestRebuild();
        }

        public static void ReleaseEdited()
        {
            _edited = null;
            RequestRebuild();
        }

        private static void OnSelectionChanged()
        {
            if (Selection.activeObject is not LevelAsset asset)
                return;
            _selected = asset;
            RequestRebuild();
        }

        private static void OnPlayModeChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingEditMode)
                Preview.Clear();
            else if (change == PlayModeStateChange.EnteredEditMode)
                RequestRebuild();
        }

        private static void RequestRebuild()
        {
            if (_rebuildRequested)
                return;
            _rebuildRequested = true;
            EditorApplication.delayCall += Rebuild;
        }

        private static void Rebuild()
        {
            _rebuildRequested = false;
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Preview.Clear();
                return;
            }
            Preview.Show(_edited ?? (_selected != null ? _selected.Level : null));
        }
    }
}
