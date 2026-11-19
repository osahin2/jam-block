# Buttons

> Summary: One touch feedback for every button in the game: the button shrinks while pressed and springs back on release, tuned in one settings asset.

## Code
- Folder: `Assets/Game/Runtime/Buttons/` (`Scripts/Presentation/`, `Scripts/Settings/`, `Scripts/Installers/`; assets in `Settings/`, `Installers/`)
- Assembly: `Game`
- Entry points: `PressFeedback` (`Buttons/Scripts/Presentation/PressFeedback.cs`), a component added next to a `Button` (any `Selectable`); `ButtonFeedbackSettings` (`Buttons/Settings/ButtonFeedbackSettings.asset`); `ButtonsInstaller` (`Buttons/Installers/ButtonsInstaller.asset`, in the Boot scope's list)

## How it works
- **Press.** `PressFeedback` handles the EventSystem's pointer down and up. On a press with the primary button, while the `Selectable` is interactable, it scales the button to `PressedScale` of its size at rest; on the release it springs back. The EventSystem sends the release to the pressed object, so a finger that slides off still brings the button back.
- **Motion.** One DOTween scale tween at a time, killed before the next; it runs in unscaled time, so a pause through `Time.timeScale`, which the game does not use today, would not freeze it, and is linked to the object with `KillOnDisable`. `OnDisable` also puts the button back at its size at rest, so a popup closed by its own button opens next time at the right size.
- **Settings.** The pressed scale, the press and release durations and eases are in `ButtonFeedbackSettings`, registered in the Boot scope by `ButtonsInstaller`, so every scene's buttons share one tuning. Screens are created through the container ([UI](../../Base/Systems/UI.md)), so the component gets the settings with an `[Inject]` method.
- **Buttons.** The buttons with feedback use the `Button`'s Transition None, so the scale is their only press visual: the HUD's restart and pause, the popups' buttons, the settings popup's switches, close and home, and the Home screen's level button.

## Rules & invariants
- A button gets its feedback only from this component; no screen animates its own buttons.
- A non-interactable button does not react.
- A press allocates no garbage beyond DOTween's pooled tweens.

## Tests
- No EditMode test: the component is a tween on pointer events, which DOTween does not run in EditMode. Checked in the editor: press and hold a button, slide off and release, press a button that closes its popup and open the popup again.
- `Tests/Bootstrap/InstallerAssetTests`: `ButtonsInstaller` has its settings assigned.

## Related
- [Play](Play.md), [Preferences](Preferences.md), [App flow](AppFlow.md)
