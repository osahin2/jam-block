using Game.Data.Levels;
using UnityEngine;

namespace Game.Editor.LevelEditor.Window
{
    // The copy of a level the editor works on. Undo records it, and only a level without errors is written back to
    // its asset, so a broken level never reaches the game.
    public sealed class LevelDraft : ScriptableObject
    {
        [SerializeField] private LevelData _level = new LevelData();

        public LevelData Level => _level;

        public void Load(LevelData source) => _level = JsonUtility.FromJson<LevelData>(JsonUtility.ToJson(source));

        public void WriteTo(LevelData target) => JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(_level), target);

        public bool Matches(LevelData other) => JsonUtility.ToJson(_level) == JsonUtility.ToJson(other);
    }
}
