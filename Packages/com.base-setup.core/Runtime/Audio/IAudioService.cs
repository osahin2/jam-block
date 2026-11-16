using UnityEngine;

namespace Core.Audio
{
    // Volumes and mutes are saved through ISaveService and restored when the service is created.
    public interface IAudioService
    {
        // Plays one of the cue's clips once. Does nothing while sound effects are muted or at zero volume.
        void Play(SfxCue cue);

        // Loops the clip, crossfading from the current music. Playing the clip that is already playing does nothing.
        void PlayMusic(AudioClip clip);

        void StopMusic();

        // 0..1. Each change is saved immediately: set a slider's value on release, not on every drag frame.
        float SfxVolume { get; set; }
        bool SfxMuted { get; set; }
        float MusicVolume { get; set; }
        bool MusicMuted { get; set; }
    }
}
