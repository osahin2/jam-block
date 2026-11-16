using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Audio
{
    // One sound event. Clips are variations of the same sound; each play picks one and a pitch in the range.
    [CreateAssetMenu(menuName = "Core/Audio/Sfx Cue", fileName = "SfxCue")]
    public sealed class SfxCue : ScriptableObject
    {
        [SerializeField] private AudioClip[] _clips = Array.Empty<AudioClip>();
        [SerializeField, Range(0f, 1f)] private float _volume = 1f;
        [SerializeField, Range(0.5f, 2f)] private float _pitchMin = 1f;
        [SerializeField, Range(0.5f, 2f)] private float _pitchMax = 1f;

        internal float Volume => _volume;

        internal bool HasClips => _clips.Length > 0;

        internal AudioClip PickClip() => _clips[Random.Range(0, _clips.Length)];

        internal float PickPitch() => Random.Range(_pitchMin, _pitchMax);

        private void OnValidate()
        {
            if (_pitchMax < _pitchMin)
                _pitchMax = _pitchMin;
        }
    }
}
