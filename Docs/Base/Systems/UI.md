# UI

> Summary: A synchronous UI service that opens screens and popups by type from prefab catalogs registered per feature, creates each screen once through the container, and fits its content to the safe area once.

## Code
- Folder: `Packages/com.base-setup.core/Runtime/UI/`
- Assembly: `Core.UI` (references uGUI, and VContainer in the installer only)
- Entry points:
  - `IUIService` ([IUIService.cs](../../../Packages/com.base-setup.core/Runtime/UI/IUIService.cs)): `T Open<T>()`, `Close(screen)`.
  - `UIScreen` ([UIScreen.cs](../../../Packages/com.base-setup.core/Runtime/UI/UIScreen.cs)): base class of every screen; `Layer` (screen or popup), `protected Close()`.
  - `UIScreenCatalog` ([UIScreenCatalog.cs](../../../Packages/com.base-setup.core/Runtime/UI/UIScreenCatalog.cs)): a feature's list of screen prefabs.
  - `UIRoot` ([UIRoot.cs](../../../Packages/com.base-setup.core/Runtime/UI/UIRoot.cs)) and the `UIRoot.prefab`: overlay canvas (1080x1920 reference, match 0.5) with a `Screens` and a `Popups` layer.
  - `UIScreenTemplate.prefab`: full-screen root with a `Content` child. Game screens are variants of it.
  - `UIInstaller` ([UIInstaller.cs](../../../Packages/com.base-setup.core/Runtime/UI/UIInstaller.cs)): `RegisterCoreUI(root)` once per scope, `RegisterUIScreens(catalog)` once per feature.

## How it works
- **Catalogs.** Each feature registers its own `UIScreenCatalog`, so a scope often holds several; the service resolves them all as one list. `RegisterUIScreens` registers each through a scoped factory, not `RegisterInstance`: VContainer's collection rejects two singleton registrations of the same implementation type (`Conflict implementation type`), and `RegisterInstance` is always a singleton. The service indexes every prefab in every catalog by its concrete `UIScreen` type; the same type in two catalogs, or an empty entry, throws when the service is created and names the catalogs.
- **Open.** `Open<T>()` creates the screen the first time (under the layer's parent in `UIRoot`), then reuses it. Opening activates the screen and moves it to the top of its layer. Data goes through the screen's own typed method (`Show(...)`), never through `Open`.
- **Creation.** The installer passes a creation function that calls `IObjectResolver.Instantiate`, so screens can take game services through `[Inject]` methods. The service itself does not reference VContainer.
- **Safe area.** When a screen is created, its `Content` rect is anchored to `Screen.safeArea`, once. The full-screen root stays unfitted so a popup's dimmer also covers the notch. The safe area belongs to the device, so it is known only at run time: a prefab places its elements inside `Content` with its own margin from the edges, prefab mode shows `Content` full screen, and a notched phone is checked in play mode in the Device Simulator. A device chosen in the simulator during play mode reaches only screens created after it, since a phone's safe area never changes while the game runs.
- **Buttons.** A screen whose buttons the caller reacts to takes one named `Action` per button in its `Show` method. On click it clears the stored callback, calls `Close()`, then invokes it, so a double click does nothing. A screen that stays open while the caller works (a HUD) keeps its callbacks and does not close itself. Callbacks, not events: screens are reused, each `Show` replaces the previous callback, and there is nothing to unsubscribe. No async: the caller reacts to the button instead of awaiting the screen.
- **Close.** `Close` deactivates the screen; it is kept for the next `Open`.

## Rules & invariants
- Every screen prefab is a variant of `UIScreenTemplate` and has `Content` assigned; a screen without it throws when first opened.
- The event system is not part of `UIRoot`: the input module depends on the game's input backend, so it stays in the scene.
- Popup or screen is a property of the screen class (`Layer`), not of where the prefab is listed.
- The order of open screens is their sibling order within a layer; there is no separate stack.
- No show or hide animations in Core; a game screen animates itself in `OnEnable` if it needs to.

## Tests
`Packages/com.base-setup.core/Tests/UI/`:
- `UIInstallerTests`: several catalogs registered in one scope all reach the service.
- `UIServiceTests`: a screen is created once and reused, screens and popups go under their layer, `Close` deactivates and `Open` reactivates, screens from several catalogs open, the same type in two catalogs is rejected naming both, an unlisted screen names its type, and the safe-area fit sets anchors from a given safe area.
