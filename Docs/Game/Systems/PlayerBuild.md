# Player build

> Builds the Android APK (development or release) and the iOS Xcode project from the Build menu or batch mode, and logs a size report. The mobile settings it builds with, and why.

**Code:** `Editor/PlayerBuilds/Scripts/Builds/`. **Entry:** `PlayerBuild` (menu `Build/...`, same methods for batch mode), `BuildReportLog`.

## Settings and reasons
- An APK, not an app bundle: the case asks for one to install directly. Signed with Unity's debug keystore; a release keystore never enters the repository. Outputs go to `Build/`, never committed.
- Android: IL2CPP, ARM64, API 24 to 36, Vulkan with GLES3 fallback, portrait, as the reference is built.
- Managed stripping Minimal: VContainer calls constructors through reflection.
- MSAA off, render scale 1: with 4x a low-end phone could not hold 60 fps; the aliasing is small at phone densities. Intermediate texture Auto: no feature needs a full-screen copy.
- vSync off, so `Core.Display`'s 60 fps applies everywhere.
- Mobile settings change only through an editor script and are recorded here.

Related: [Display](../../Base/Systems/Display.md)
