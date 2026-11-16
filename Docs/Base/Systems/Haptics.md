# Haptics

> Summary: Plays semantic haptic presets (light, medium, heavy, success, failure) through a platform backend and keeps the player's on/off choice, saved through `Core.Save`. iOS has a native backend of its own; the editor and other platforms are silent. A game can supply its own backend.

## Code
- Folder: `Packages/com.base-setup.core/Runtime/Haptics/`
- Assembly: `Core.Haptics` (references `Core.Save`, and VContainer in the installer only)
- Entry points:
  - `IHapticService` ([IHapticService.cs](../../../Packages/com.base-setup.core/Runtime/Haptics/IHapticService.cs)): `Play(HapticPreset)`, `Enabled`.
  - `HapticPreset` ([HapticPreset.cs](../../../Packages/com.base-setup.core/Runtime/Haptics/HapticPreset.cs)): `Light`, `Medium`, `Heavy`, `Success`, `Failure`.
  - `IHapticBackend` ([IHapticBackend.cs](../../../Packages/com.base-setup.core/Runtime/Haptics/IHapticBackend.cs)): `Play(HapticPreset)`, for a game's own backend.
  - `HapticInstaller` ([HapticInstaller.cs](../../../Packages/com.base-setup.core/Runtime/Haptics/HapticInstaller.cs)): `RegisterCoreHaptics()` with the native backend, or `RegisterCoreHaptics(backend)` with the game's. In the root scope after `RegisterCoreSave()`.

## How it works
- **Presets, not parameters.** A caller says what the feedback means; each backend maps a preset to its platform's closest effect. There are no intensities, durations or custom patterns: a hybrid-casual game's tap and result feedback needs none, and each would have to be faked on platforms that cannot do it.
- **Service and backend** (`HapticService`, internal): the service loads the player's setting and passes a preset to the backend only while haptics are enabled. The backend only produces the effect, so a backend written by a game never has to check the setting.
- **Why the backend is replaceable and the service is not:** a game may already use a third-party haptics package, which Core cannot reference (it lives under `Assets/`, licensed per project). The game writes an adapter and passes it to the installer. Replacing the whole service would lose the saved setting and the guarantee that a disabled channel stays silent. The override is an explicit installer argument, not a second container registration whose winner is not visible.
- **iOS backend** (`IosHapticBackend`, internal, compiled for iOS only): calls `CoreHapticFeedback_Play` in `Plugins/iOS/CoreHapticFeedback.mm`, which drives the UIKit feedback generators: `UIImpactFeedbackGenerator` light, medium and heavy for the first three presets, `UINotificationFeedbackGenerator` success and error for the last two. The generators are created on first use and re-prepared after each play, so a repeated preset starts with less latency. The native file indexes a table of functions by the preset's number, which is why `HapticPreset` values are fixed and new presets are only appended. It is written to be correct with and without ARC.
- **Platform choice** (`NativeHapticBackend`, internal): at compile time, the iOS backend in an iOS player, the silent backend (`NoHapticBackend`) in the editor and everywhere else. Android has no backend yet and is silent.
- **Persistence:** one `Core.Save` document under the key `haptic-settings` with `enabled` (default true). The service loads it in its constructor; each change is written at once; setting the current value writes nothing.
- **No scene objects** and no per-frame work.

## Rules & invariants
- `RegisterCoreHaptics(null)` throws `ArgumentNullException`.
- `HapticPreset` numbers are part of the native contract. Never reorder them; append a new preset and add its entry to the table in `CoreHapticFeedback.mm`. An unknown number is ignored on the native side.
- The native plugin is imported for iOS only (Unity's default for `.mm`), so the editor and other platforms never link it. `IosHapticBackend` sits behind `#if UNITY_IOS` because IL2CPP links an `"__Internal"` import directly.
- `Play` allocates nothing. Changing `Enabled` allocates when it serializes the save document.
- **With a third-party haptics package:** pass an adapter to `RegisterCoreHaptics(backend)`. Feedback that the package triggers on its own (for example from its effect players) bypasses this service, so the game mirrors `Enabled` into the package's global switch.

## Tests
`Packages/com.base-setup.core/Tests/Haptics/`, EditMode with a recording backend and an in-memory save:
- `HapticServiceTests`: enabled by default, presets reach the backend in order, nothing reaches it while disabled, disabling is saved and restored, an unchanged value does not write.
- `HapticInstallerTests`: a game backend registered through the installer receives the presets, a null backend throws, the native registration resolves one service, and in the editor the native backend is the silent one.

Not verified yet: the iOS backend on a device. The iOS Simulator has no haptic engine, the build is not set up, and this machine has no iOS SDK, so `CoreHapticFeedback.mm` has not been compiled. With the active build target on iOS, `IosHapticBackend` compiles in the editor. First device check: build to an iPhone, play each preset, and toggle `Enabled` off and on.

## Related
- Register it in the root scope after `RegisterCoreSave()`.
- [Save](Save.md) (on/off setting)
