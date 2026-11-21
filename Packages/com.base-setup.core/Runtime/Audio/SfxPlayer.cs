using System;
using System.Collections.Generic;
using Core.Pooling;
using UnityEngine;

namespace Core.Audio
{
    internal sealed class SfxPlayer
    {
        private struct Voice
        {
            public AudioSource Source;
            public float CueVolume;
            public float EndTime;
        }

        private readonly PrefabPool<AudioSource> _pool;
        private readonly Transform _parent;
        private readonly List<Voice> _playing = new List<Voice>();
        private float _time;
        private float _volume = 1f;

        internal int PlayingCount => _playing.Count;

        public SfxPlayer(PrefabPool<AudioSource> pool, Transform parent)
        {
            _pool = pool;
            _parent = parent;
        }

        // Pitch is per source, so every play takes its own source instead of sharing one through PlayOneShot.
        public void Play(SfxCue cue)
        {
            if (cue == null)
                throw new ArgumentNullException(nameof(cue));
            if (_volume <= 0f)
                return;
            if (!cue.HasClips)
            {
                Debug.LogWarning($"[Core.Audio] Sfx cue '{cue.name}' has no clips.", cue);
                return;
            }

            AudioClip clip = cue.PickClip();
            float pitch = cue.PickPitch();
            AudioSource source = _pool.Get(_parent);
            source.clip = clip;
            source.pitch = pitch;
            source.volume = cue.Volume * _volume;
            source.Play();

            // The end time comes from the clip length rather than isPlaying, so the bookkeeping does not depend on the audio thread.
            _playing.Add(new Voice { Source = source, CueVolume = cue.Volume, EndTime = _time + clip.length / pitch });
        }

        // Applies to sounds already playing as well as new ones.
        public void SetVolume(float volume)
        {
            _volume = volume;
            for (int i = 0; i < _playing.Count; i++)
                _playing[i].Source.volume = _playing[i].CueVolume * volume;
        }

        public void Tick(float unscaledDeltaTime)
        {
            _time += unscaledDeltaTime;
            for (int i = _playing.Count - 1; i >= 0; i--)
            {
                Voice voice = _playing[i];
                if (_time < voice.EndTime)
                    continue;

                int last = _playing.Count - 1;
                _playing[i] = _playing[last];
                _playing.RemoveAt(last);

                voice.Source.Stop();
                voice.Source.clip = null;
                _pool.Release(voice.Source);
            }
        }
    }
}
