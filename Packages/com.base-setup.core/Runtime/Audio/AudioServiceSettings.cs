using UnityEngine;

namespace Core.Audio
{
    // Not named AudioSettings: that would clash with UnityEngine.AudioSettings in any file using both namespaces.
    [CreateAssetMenu(menuName = "Core/Audio/Audio Service Settings", fileName = "AudioServiceSettings")]
    public sealed class AudioServiceSettings : ScriptableObject
    {
        [Tooltip("Sound effect sources created up front.")]
        [SerializeField, Min(0)] private int _sfxPrewarmCount = 4;

        [Tooltip("Idle sound effect sources kept for reuse; more can play at once, the extra ones are destroyed when they finish.")]
        [SerializeField, Min(1)] private int _sfxMaxIdle = 16;

        [Tooltip("Seconds for music to fade in, fade out or crossfade. 0 switches at once.")]
        [SerializeField, Min(0f)] private float _musicFadeDuration = 1f;

        internal int SfxPrewarmCount => Mathf.Min(_sfxPrewarmCount, _sfxMaxIdle);
        internal int SfxMaxIdle => _sfxMaxIdle;
        internal float MusicFadeDuration => _musicFadeDuration;
    }
}
