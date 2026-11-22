# Player build

> Summary: Builds the Android APK (development or release) and the iOS Xcode project from a menu or from batch mode, and logs a short report: result, output size, time and the largest assets. The mobile player, URP and quality settings it builds with are listed here with their reasons.

## Code
- Folder: `Assets/Game/Editor/PlayerBuilds/`
- Assembly: `Game.Editor`
- Entry points:
  - `PlayerBuild` ([PlayerBuild.cs](../../../Assets/Game/Editor/PlayerBuilds/PlayerBuild.cs)): menu `Build/Android APK (Development)`, `Build/Android APK (Release)`, `Build/iOS Xcode Project`; the same methods for batch mode.
  - `BuildReportLog` ([BuildReportLog.cs](../../../Assets/Game/Editor/PlayerBuilds/BuildReportLog.cs)): the report written to the log, every line starting with `Build:`.

## How it works
- **Scenes** come from the build settings (Boot, Home, Game), enabled ones only.
- **Outputs** under `Build/` (ignored by git): `Build/Android/RollicCaseStudy-<version>-dev.apk`, `...-release.apk`, `Build/iOS/` (Xcode project).
- **Development**: development player, profiler connects on start, LZ4. For the Unity Profiler over USB and for logs; its timings are not the release game's.
- **Release**: LZ4HC (smaller download, same load speed as LZ4). The delivered APK and the one measured.
- **APK, not an app bundle**: the case asks for an APK to install directly; `buildAppBundle` and the Gradle export are turned off before each Android build.
- **Signing**: Unity's debug keystore. A release keystore is the developer's own and never enters the repository.
- **Batch mode**: `unity run . -- -buildTarget Android -executeMethod Game.Editor.PlayerBuilds.PlayerBuild.AndroidRelease`; a failed build exits with code 1.

## Mobile settings
Set by an editor script. The reference game is built with the same Unity version (2022.3.62f2), IL2CPP, ARM64, minimum API 24, target API 36, GLES3 required and Vulkan optional.
- **Android**: IL2CPP, ARM64 only; minimum API 24, target API 36 (the editor's bundled SDK has 34 to 36); Vulkan first with GLES3 as fallback; Vulkan swapchain at Unity's default of three buffers; optimized frame pacing (Swappy) off, as in Unity's default; portrait only, drawn outside the safe area (screens are laid out inside it).
- **Both platforms**: managed stripping Minimal, because VContainer calls constructors through reflection and higher levels could strip them; stack traces off for logs and warnings, kept for errors, exceptions and asserts; accelerometer off.
- **iOS**: Metal API validation off (a debugging aid that slows the player); target iOS 12, iPhone and iPad, full screen, status bar hidden. Signing is left to the developer's team.
- **URP**: intermediate texture Auto (no post-processing or renderer feature needs one, so no full-screen copy each frame); terrain holes and LOD cross-fade off (unused, fewer shader variants); HDR, shadows and additional lights already off; MSAA off and render scale 1, as in the reference: with 4x a low-end phone's GPU could not hold 60 fps at its native resolution, and without it the board's slanted edges alias only a little at phone pixel densities.
- **Quality**: vSync off, so the frame rate is `Core.Display`'s 60 everywhere (phones ignore vSync).
- **Frame rate and screen on**: `Core.Display` in the Boot scope ([Display](../../Base/Systems/Display.md)).

## Rules & invariants
- Change mobile settings through an editor script and record them here.
- `Build/` is never committed.

## Tests
None: a build is checked by running it. The release APK on a low-end Android phone (Mali GPU, 19.5:9): home, a level, the held block's outline, a block leaving through its door (cut at the door line, cubes bursting) and the win screen draw correctly on Vulkan and on GLES3 (started with `-force-gles`). On Vulkan the game holds 60 fps with a few milliseconds of GPU work per frame; on GLES3 about one frame in ten takes two refreshes, so Vulkan stays first.

Not verified yet: the development build with the profiler, the iOS Xcode project.

## Related
- [App flow](AppFlow.md) (the scenes it builds)
