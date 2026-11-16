using System;
using UnityEngine;

namespace Core.Audio
{
    // Two looping sources: the current track fades towards full level, the other one towards silence, which gives a
    // crossfade when the track changes. A track keeps playing at zero volume while music is muted.
    internal sealed class MusicPlayer
    {
        private sealed class Track
        {
            public readonly AudioSource Source;
            public float Level;
            public float Target;

            public Track(AudioSource source)
            {
                Source = source;
                Source.loop = true;
            }
        }

        private readonly float _fadeDuration;
        private Track _current;
        private Track _other;
        private float _volume = 1f;

        public MusicPlayer(AudioSource first, AudioSource second, float fadeDuration)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            _current = new Track(first);
            _other = new Track(second);
            _fadeDuration = fadeDuration;
        }

        public void Play(AudioClip clip)
        {
            if (clip == null)
                throw new ArgumentNullException(nameof(clip));
            // Already playing, or fading out after Stop: fade back in without restarting.
            if (_current.Source.clip == clip)
            {
                _current.Target = 1f;
                return;
            }

            _current.Target = 0f;
            (_current, _other) = (_other, _current);

            // The other source may still be fading out a track; reuse it if it is this clip, otherwise cut it.
            if (_current.Source.clip != clip)
            {
                _current.Source.Stop();
                _current.Source.clip = clip;
                _current.Level = 0f;
                _current.Source.Play();
            }

            _current.Target = 1f;
            Apply(_current);
        }

        public void Stop()
        {
            _current.Target = 0f;
        }

        public void SetVolume(float volume)
        {
            _volume = volume;
            Apply(_current);
            Apply(_other);
        }

        public void Tick(float unscaledDeltaTime)
        {
            float step = _fadeDuration > 0f ? unscaledDeltaTime / _fadeDuration : 1f;
            Fade(_current, step);
            Fade(_other, step);
        }

        private void Fade(Track track, float step)
        {
            if (track.Source.clip == null)
                return;

            track.Level = Mathf.MoveTowards(track.Level, track.Target, step);
            Apply(track);

            if (track.Level <= 0f && track.Target <= 0f)
            {
                track.Source.Stop();
                track.Source.clip = null;
            }
        }

        private void Apply(Track track)
        {
            track.Source.volume = track.Level * _volume;
        }
    }
}
