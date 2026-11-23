using Game.Data.Levels;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.LevelEditor
{
    public sealed class LevelDraft : ScriptableObject
    {
        [SerializeField] private LevelData _level = new LevelData();

        private string _strokeStart;

        public LevelData Level => _level;

        public static LevelDraft Create()
        {
            var draft = CreateInstance<LevelDraft>();
            draft.hideFlags = HideFlags.DontSave;
            return draft;
        }

        public void Load(LevelData source)
        {
            _level = LevelCopy.Of(source);
            Undo.ClearUndo(this);
        }

        public void WriteTo(LevelData target) => JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(_level), target);

        public bool Matches(LevelData other) => JsonUtility.ToJson(_level) == JsonUtility.ToJson(other);

        public void Record(string name) => Undo.RegisterCompleteObjectUndo(this, name);

        public void BeginStroke() => _strokeStart = JsonUtility.ToJson(_level);

        public void EndStroke(string name)
        {
            if (JsonUtility.ToJson(_level) == _strokeStart)
                return;
            var edited = _level;
            _level = JsonUtility.FromJson<LevelData>(_strokeStart);
            Undo.IncrementCurrentGroup();
            Record(name);
            _level = edited;
        }

        public void Discard()
        {
            Undo.ClearUndo(this);
            DestroyImmediate(this);
        }
    }
}
