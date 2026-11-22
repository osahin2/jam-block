# Display

> Summary: Applies the app's display settings once at startup: the target frame rate and whether the screen stays on. Both come from a `DisplaySettings` asset.

## Code
- Folder: `Packages/com.base-setup.core/Runtime/Display/`
- Assembly: `Core.Display` (VContainer in the installer only)
- Entry points:
  - `DisplaySettings` ([DisplaySettings.cs](../../../Packages/com.base-setup.core/Runtime/Display/DisplaySettings.cs)): target frame rate (default 60) and keep screen on (default true).
  - `DisplayInstaller` ([DisplayInstaller.cs](../../../Packages/com.base-setup.core/Runtime/Display/DisplayInstaller.cs)): `RegisterCoreDisplay(settings)`, in the root scope.

## How it works
- **Applied when the root container is built** (`RegisterBuildCallback`): `Application.targetFrameRate` and `Screen.sleepTimeout` are set once, before any entry point starts.
- **Why a frame rate at all:** Android and iOS run at 30 fps unless `targetFrameRate` is set, so motion looks choppy on every phone. 60 is the default on purpose: 90 and 120 Hz screens would cost battery and heat for a casual game.
- **Why the screen stays on:** a game the player watches without touching (a timer running, an animation) must not dim or lock.
- **No service and no interface:** nothing calls the module after startup, so a service would only forward two assignments. Add one when a game changes these values at run time (a battery saver option, for example).
- **vSync:** on desktop and in the editor `QualitySettings.vSyncCount` above 0 overrides `targetFrameRate`; Android and iOS ignore vSync. Keep `vSyncCount` at 0 in the quality levels the game uses, so the frame rate is this module's on every platform.

## Rules & invariants
- `RegisterCoreDisplay(null)` throws `ArgumentNullException`.
- Registering changes nothing; building the container applies the settings.
- No scene objects and no per-frame work.

## Tests
`Packages/com.base-setup.core/Tests/Display/DisplayInstallerTests.cs`, EditMode: building the container applies the defaults, registering alone changes nothing, null settings throw. Each test restores the editor's frame rate and sleep timeout.

Not verified yet: the frame rate and the screen staying on in a player on a device.

## Related
- Register it in the root scope, first, so the frame rate holds from the boot scene on.
