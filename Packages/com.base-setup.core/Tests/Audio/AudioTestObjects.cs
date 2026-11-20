using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.Audio.Tests
{
    internal sealed class AudioTestObjects
    {
        private readonly List<Object> _created = new List<Object>();

        public AudioClip Clip(float seconds)
        {
            const int frequency = 1000;
            var clip = AudioClip.Create("Clip", Mathf.RoundToInt(seconds * frequency), 1, frequency, false);
            _created.Add(clip);
            return clip;
        }

        public SfxCue Cue(float volume, float pitchMin, float pitchMax, params AudioClip[] clips)
        {
            var cue = ScriptableObject.CreateInstance<SfxCue>();
            cue.name = "Cue";
            var serialized = new SerializedObject(cue);
            SerializedProperty clipsProperty = serialized.FindProperty("_clips");
            clipsProperty.arraySize = clips.Length;
            for (int i = 0; i < clips.Length; i++)
                clipsProperty.GetArrayElementAtIndex(i).objectReferenceValue = clips[i];
            serialized.FindProperty("_volume").floatValue = volume;
            serialized.FindProperty("_pitchMin").floatValue = pitchMin;
            serialized.FindProperty("_pitchMax").floatValue = pitchMax;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            _created.Add(cue);
            return cue;
        }

        public AudioSource Source(string name)
        {
            var source = new GameObject(name).AddComponent<AudioSource>();
            source.playOnAwake = false;
            _created.Add(source.gameObject);
            return source;
        }

        public Transform Root(string name)
        {
            var root = new GameObject(name).transform;
            _created.Add(root.gameObject);
            return root;
        }

        public void Track(Object created) => _created.Add(created);

        public void DestroyAll()
        {
            foreach (Object created in _created)
                if (created != null)
                    Object.DestroyImmediate(created);
            _created.Clear();
        }
    }
}
