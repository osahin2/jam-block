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
            // Not HideAndDontSave: it includes NotEditable, which makes bound fields refuse edits.
            draft.hideFlags = HideFlags.DontSave;
            return draft;
        }

        // The undo history of the level loaded before does not apply to this one.
        public void Load(LevelData source)
        {
            _level = JsonUtility.FromJson<LevelData>(JsonUtility.ToJson(source));
            Undo.ClearUndo(this);
        }

        public void WriteTo(LevelData target) => JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(_level), target);

        public bool Matches(LevelData other) => JsonUtility.ToJson(_level) == JsonUtility.ToJson(other);

        // Undo keeps whole copies: Unity restores differences in a list wrongly once an item was removed from its middle.
        public void Record(string name) => Undo.RegisterCompleteObjectUndo(this, name);

        public void BeginStroke() => _strokeStart = JsonUtility.ToJson(_level);

        public void EndStroke(string name)
        {
            if (JsonUtility.ToJson(_level) == _strokeStart)
                return;
            // Undo copies the draft as recorded, so the level from before the stroke stands in for a moment; its objects
            // stay the same, so a selection keeps its block or door.
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
