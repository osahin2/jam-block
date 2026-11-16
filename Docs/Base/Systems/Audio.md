# Audio

> Summary: Plays sound effect cues as one-shots on pooled `AudioSource`s and loops music with fades and crossfades. Keeps the player's volume and mute choice per channel (sound effects, music) and saves it through `Core.Save`. Synchronous API; nothing to await.

## Code
- Folder: `Packages/com.base-setup.core/Runtime/Audio/`
- Assembly: `Core.Audio` (references `Core.Pooling`, `Core.Save`, and VContainer in the installer only)
- Entry points:
  - `IAudioService` ([IAudioService.cs](../../../Packages/com.base-setup.core/Runtime/Audio/IAudioService.cs)): `Play(SfxCue)`, `PlayMusic(AudioClip)`, `StopMusic()`, `SfxVolume`, `SfxMuted`, `MusicVolume`, `MusicMuted`.
  - `SfxCue` ([SfxCue.cs](../../../Packages/com.base-setup.core/Runtime/Audio/SfxCue.cs)): one sound event as a ScriptableObject (`Core/Audio/Sfx Cue`): clips, volume, pitch range.
  - `AudioServiceSettings` ([AudioServiceSettings.cs](../../../Packages/com.base-setup.core/Runtime/Audio/AudioServiceSettings.cs)): SFX pool prewarm and idle size, music fade duration.
  - `AudioInstaller` ([AudioInstaller.cs](../../../Packages/com.base-setup.core/Runtime/Audio/AudioInstaller.cs)): `RegisterCoreAudio(settings)`, in the root scope after `RegisterCoreSave()`.

## How it works
- **One asset per sound event.** A `SfxCue` is a single sound such as a tap or a win, stored in the feature that uses it and referenced from that feature's settings asset. Its clips are variations of the same sound. Each play picks one clip at random and a pitch in `[pitchMin, pitchMax]`. The cue's volume balances the sound against the others, and the player's SFX volume multiplies it. There is no central sound library with ids: ids would put game vocabulary into Core, and one shared asset would change with every feature.
- **Sound effects** (`SfxPlayer`, internal): every play takes its own source from a `PrefabPool<AudioSource>`, because pitch is a property of the source. A single source with `PlayOneShot` could not vary the pitch per play. The source is returned to the pool when `clip.length / pitch` seconds have passed. That time is counted from the clip length rather than read from `isPlaying`, so the bookkeeping does not depend on the audio thread and can be tested. There is no cap on sounds playing at once: the engine's real-voice limit already virtualizes the extra ones.
- **Music** (`MusicPlayer`, internal): two looping sources. The current track fades towards full level and the other one towards silence, so changing the track crossfades. A source whose fade reaches silence stops and drops its clip. Playing the clip that is already playing does nothing. Playing a clip that is still fading out, after `StopMusic` or a switch, fades it back in without restarting it. With a fade duration of 0 the level jumps on the next frame.
- **Volume and mute** (`AudioService`): the effective volume of a channel is its volume, or 0 while it is muted. It is applied to the sources directly, including sound effects that are already playing. Muted music keeps playing silently, so unmuting continues where it was. A muted (or zero-volume) sound effect channel does not take a source at all.
- **Persistence:** the four values are one `Core.Save` document under the key `audio-settings`. The service loads it in its constructor, so the first sound or track already plays at the saved levels. Each change is written at once. Setting the current value writes nothing.
- **Scene objects:** on the first resolve of `IAudioService`, the module creates one `Core.Audio` GameObject marked `DontDestroyOnLoad`. It holds the two music sources, an inactive template source that the pool copies, the pooled sources and an internal `AudioTicker`. Because the object is marked `DontDestroyOnLoad`, it does not depend on how the game loads scenes. The ticker advances both players in unscaled time, so fades and source returns continue while the game is paused.
- **No AudioMixer.** Volume and mute are set on the sources. A mixer with exposed parameters would add a string contract and dB conversion. It would also need a mixer asset built by hand in the editor, since Unity has no public API to create one or expose its parameters from script, and it would buy nothing for volume and mute. When a game needs effects, ducking or snapshots, `AudioMixerGroup` fields for the two channels can be added to the settings. That change is additive for callers.

## Rules & invariants
- Set a volume slider's value on release, not on every drag frame: each change writes the save file (Save writes immediately, see [Save](Save.md)).
- `Play(null)` and `PlayMusic(null)` throw `ArgumentNullException`. A cue without clips logs a warning and plays nothing.
- A cue's pitch range is 0.5 to 2. The maximum is raised to the minimum in the inspector if it is lower.
- Sound effects are one-shots: there is no handle to stop one and no looping sound effect. Add them when a game needs them.
- `AudioListener.pause` is not accounted for. Sources paused that way are still returned to the pool once their clip length has passed.
- The module creates no `AudioListener`. The game keeps exactly one, alive across scene switches; without one nothing is heard and Unity logs "There are no audio listeners in the scene" every frame (at log level, not as a warning).
- Sound effects are 2D (no position). 3D sound effects would need a position parameter and are added when a game needs them.
- `Play` and the per-frame tick allocate nothing once the pool is warm. A volume or mute change allocates when it serializes the save document.

## Tests
`Packages/com.base-setup.core/Tests/Audio/`, EditMode with clips made by `AudioClip.Create`:
- `SfxPlayerTests`: a play takes an active source with one of the cue's clips and a pitch in range. The source returns to the pool after the clip length and is reused, and a higher pitch ends the sound sooner. Volume changes reach sounds already playing. At zero volume nothing plays. A cue without clips warns.
- `MusicPlayerTests`: fade-in, crossfade, the same clip keeps playing, a clip that is fading out fades back in on the same source, stop fades out and releases the clip, volume scales the fade level, and a zero fade duration reaches full level on the next tick.
- `AudioServiceTests` (in-memory save): defaults, values saved and restored, an unchanged value does not write, volume clamped to 0..1, muted sound effects take no source, a restored music mute silences the first track and unmuting restores it.
- `AudioServiceFactoryTests`: one root with two music sources, the template and the prewarmed pool, and the ticker.

The cue-without-clips test logs one expected warning to the console.

The engine path was checked in play mode, in the running game:
- The root ends up in `DontDestroyOnLoad`.
- A generated cue played on a pooled source, and the source was back in the pool, inactive, after the clip ended.
- Music faded in to full level.
- Muting music set its source to 0 and wrote `"musicMuted":true` to `audio-settings.json`. Unmuting restored both.
- `StopMusic` faded the track out and released its clip.
- The console stayed clean. A first run had no `AudioListener` in any scene, which made everything inaudible and logged a message every frame; the boot scene now holds one.

## Related
- Register it in the root scope with an `AudioServiceSettings` asset, and keep the one `AudioListener` on its own object in the boot scene, which never unloads (sound effects are 2D, so its position does not matter).
- [Pooling](Pooling.md) (sound effect sources), [Save](Save.md) (volume and mute)
