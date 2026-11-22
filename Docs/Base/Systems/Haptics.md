# Haptics

> Semantic haptic presets (light, medium, heavy, success, failure) through a platform backend, with the player's on/off choice saved through Save. iOS has its own native backend; elsewhere it is silent.

**Code:** `Core.Haptics`, `Runtime/Haptics/Plugins/iOS/CoreHapticFeedback.mm`. **Entry:** `IHapticService` (`Play`, `Enabled`), `IHapticBackend`; `RegisterCoreHaptics()` or `RegisterCoreHaptics(backend)`.

## Rules
- Presets, not parameters: callers say what the feedback means.
- The service checks the setting; a backend only plays, so a game's own backend (an adapter for a third-party package) never checks it. The backend is replaceable through the installer, the service is not.
- `HapticPreset` numbers index the native table: never reorder, only append.
- Android has no backend yet and is silent.
