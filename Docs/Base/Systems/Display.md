# Display

> Applies the target frame rate and keep-screen-on once, when the root container is built, from a `DisplaySettings` asset.

**Code:** `Core.Display`. **Entry:** `DisplaySettings`; `RegisterCoreDisplay(settings)`.

## Rules
- Phones run at 30 fps unless a target is set; 60 is the default, as 90 and 120 Hz would cost battery and heat for a casual game.
- The screen stays on, as a player may watch without touching.
- No service: nothing changes these at run time. Keep `vSyncCount` at 0, or it overrides the target on desktop and in the editor.
