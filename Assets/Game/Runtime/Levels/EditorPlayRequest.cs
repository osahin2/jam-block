#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Levels
{
    // A level the level editor asked to play. It goes through SessionState, which outlives the domain reload when play
    // mode starts, and is taken once, so pressing Play in the toolbar afterwards starts the game as usual. A build
    // never has one.
    public sealed class EditorPlayRequest
    {
#if UNITY_EDITOR
        private const string Key = "Game.EditorPlayRequest";
#endif

        public bool IsPending
        {
            get
            {
#if UNITY_EDITOR
                return SessionState.GetString(Key, string.Empty).Length > 0;
#else
                return false;
#endif
            }
        }

#if UNITY_EDITOR
        public void Request(LevelAsset level) =>
            SessionState.SetString(Key, AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(level)));
#endif

        public bool TryTake(out LevelAsset level)
        {
#if UNITY_EDITOR
            var guid = SessionState.GetString(Key, string.Empty);
            SessionState.EraseString(Key);
            level = guid.Length == 0 ? null : AssetDatabase.LoadAssetAtPath<LevelAsset>(AssetDatabase.GUIDToAssetPath(guid));
            return level != null;
#else
            level = null;
            return false;
#endif
        }
    }
}
