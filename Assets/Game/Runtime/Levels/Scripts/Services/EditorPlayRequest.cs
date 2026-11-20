#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Levels
{
    public sealed class EditorPlayRequest
    {
#if UNITY_EDITOR
        // SessionState outlives the domain reload when play mode starts; a build never has a request.
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
