using Game.Data.Levels;
using UnityEngine;

namespace Game.Levels
{
    public sealed class LevelAsset : ScriptableObject
    {
        [SerializeField] private LevelData _level = new LevelData();

        public LevelData Level => _level;
    }
}
