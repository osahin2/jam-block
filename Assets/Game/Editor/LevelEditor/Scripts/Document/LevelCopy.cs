using Game.Data.Levels;
using UnityEngine;

namespace Game.Editor.LevelEditor
{
    public static class LevelCopy
    {
        public static LevelData Of(LevelData level) => JsonUtility.FromJson<LevelData>(JsonUtility.ToJson(level));
    }
}
