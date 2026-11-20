using Game.Data.Levels;
using Game.Levels;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Game.Editor.LevelEditor
{
    [InitializeOnLoad]
    internal static class LevelPreviewSource
    {
        private static readonly LevelScenePreview Preview = new LevelScenePreview();
        private static LevelData _edited;
        private static LevelAsset _selected;
        private static bool _rebuildRequested;

        // Static: it follows the Project selection while no level editor window is open.
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
