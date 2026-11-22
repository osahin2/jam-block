# Haptics

> Semantic haptic presets (light, medium, heavy, success, failure, selection) through a platform backend, with the player's on/off choice saved through Save. iOS and Android 8+ have native backends; elsewhere it is silent.

**Code:** `Core.Haptics`, `Runtime/Haptics/Plugins/iOS/CoreHapticFeedback.mm`, `AndroidHapticBackend`, `Editor/Haptics/AndroidVibratePermission`. **Entry:** `IHapticService` (`Play`, `Enabled`), `IHapticBackend`; `RegisterCoreHaptics()` or `RegisterCoreHaptics(backend)`.

## Rules
- Presets, not parameters: callers say what the feedback means.
- The service checks the setting; a backend only plays, so a game's own backend (an adapter for a third-party package) never checks it. The backend is replaceable through the installer, the service is not.
- `HapticPreset` numbers index the iOS table and `AndroidHapticBackend.Effects`: never reorder, only append to both.
- iOS: the UIKit feedback generators (impact, notification, selection).
- Android: `Vibrator.vibrate(VibrationEffect)`. On API 29+ light, heavy and selection use the hardware-tuned predefined click, heavy click and tick; the rest, and everything on API 26-28, are waveforms (amplitudes are ignored by vibrators without amplitude control). API 24-25 and devices without a vibrator are silent.
- Android effects and JNI arguments are made once, so a play allocates nothing; a failed call clears its Java exception.
- Android needs `VIBRATE`: Unity adds it only for `Handheld.Vibrate`, so `AndroidVibratePermission` writes it into the generated manifest of every Android build.
