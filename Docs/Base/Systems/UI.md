# UI

> A synchronous UI service that opens screens and popups by type from per-feature prefab catalogs, creates each once through the container and fits its content to the safe area.

**Code:** `Core.UI`. **Entry:** `IUIService` (`Open<T>`, `Close`), `UIScreen`, `UIScreenCatalog`, `UIRoot.prefab`, `UIScreenTemplate.prefab`; `RegisterCoreUI(root)`, `RegisterUIScreens(catalog)`.

## Rules
- Catalogs are registered through a scoped factory: VContainer rejects two singleton instances of one type in a collection. A screen type in two catalogs throws at creation.
- A screen is created once and reused; data goes through its own `Show(...)`, never through `Open`.
- Only `Content` is fitted to `Screen.safeArea`, once at creation, so a popup's dimmer still covers the notch. The safe area is known only at run time; check notched phones in the Device Simulator.
- A screen takes one callback per button in `Show`; on click it clears it, closes, then calls it, so a double click does nothing. A screen that stays open (a HUD) keeps its callbacks.
- Every screen prefab is a variant of `UIScreenTemplate`. No show/hide animations in Core. The EventSystem stays in the scene, since its input module is the game's choice.
