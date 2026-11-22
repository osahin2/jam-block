# Audio

> Sound effect cues as one-shots on pooled sources, looping music with crossfades, and the player's volume and mute per channel, saved through Save.

**Code:** `Core.Audio`. **Entry:** `IAudioService` (`Play(SfxCue)`, `PlayMusic`, `StopMusic`, volume and mute), `SfxCue`; `RegisterCoreAudio(settings)`.

## Rules
- One `SfxCue` asset per sound event, kept in the feature that uses it; no central library of ids, which would put game vocabulary into Core.
- Each play takes its own pooled source, because pitch is per source. The source returns after `clip.length / pitch`, counted rather than read from `isPlaying`, so it is testable.
- No AudioMixer: volume and mute are set on the sources. A mixer would add a string contract and a hand-made asset for nothing.
- Muted music keeps playing silently. A volume change writes the save file, so sliders set it on release.
- The module creates no `AudioListener`; the game keeps one in the boot scene. Sound effects are 2D.
- `Play` and the tick allocate nothing once warm.
