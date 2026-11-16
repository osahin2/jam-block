using System;
using Core.Save;
using UnityEngine;

namespace Core.Audio
{
    // Owns the player's volume and mute choices: restores them on creation, saves every change and applies the
    // effective volume (zero while muted) to the sound effect and music players.
    internal sealed class AudioService : IAudioService
    {
        internal const string SaveKey = "audio-settings";

        [Serializable]
        internal sealed class Document
        {
            public float sfxVolume = 1f;
            public float musicVolume = 1f;
            public bool sfxMuted;
            public bool musicMuted;
        }

        private readonly ISaveService _save;
        private readonly SfxPlayer _sfx;
        private readonly MusicPlayer _music;
        private readonly Document _document;

        public float SfxVolume
        {
            get => _document.sfxVolume;
            set
            {
                float volume = Mathf.Clamp01(value);
                if (Mathf.Approximately(volume, _document.sfxVolume))
                    return;

                _document.sfxVolume = volume;
                ApplySfxVolume();
                Save();
            }
        }

        public bool SfxMuted
        {
            get => _document.sfxMuted;
            set
            {
                if (value == _document.sfxMuted)
                    return;

                _document.sfxMuted = value;
                ApplySfxVolume();
                Save();
            }
        }

        public float MusicVolume
        {
            get => _document.musicVolume;
            set
            {
                float volume = Mathf.Clamp01(value);
                if (Mathf.Approximately(volume, _document.musicVolume))
                    return;

                _document.musicVolume = volume;
                ApplyMusicVolume();
                Save();
            }
        }

        public bool MusicMuted
        {
            get => _document.musicMuted;
            set
            {
                if (value == _document.musicMuted)
                    return;

                _document.musicMuted = value;
                ApplyMusicVolume();
                Save();
            }
        }

        public AudioService(ISaveService save, SfxPlayer sfx, MusicPlayer music)
        {
            _save = save;
            _sfx = sfx;
            _music = music;
            _document = save.Load<Document>(SaveKey);
            ApplySfxVolume();
            ApplyMusicVolume();
        }

        public void Play(SfxCue cue) => _sfx.Play(cue);

        public void PlayMusic(AudioClip clip) => _music.Play(clip);

        public void StopMusic() => _music.Stop();

        private void ApplySfxVolume() => _sfx.SetVolume(_document.sfxMuted ? 0f : _document.sfxVolume);

        private void ApplyMusicVolume() => _music.SetVolume(_document.musicMuted ? 0f : _document.musicVolume);

        private void Save() => _save.Save(SaveKey, _document);
    }
}
