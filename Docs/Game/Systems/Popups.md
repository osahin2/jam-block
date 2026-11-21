# Popups

> Summary: One entrance for every popup: the dimmer fades in and the popup's parts grow in one after the other with an overshoot, tuned in one settings asset. Popups close without an animation.

## Code
- Folder: `Assets/Game/Runtime/Popups/` (`Scripts/Presentation/`, `Scripts/Settings/`, `Scripts/Installers/`; assets in `Settings/`, `Installers/`)
- Assembly: `Game`
- Entry points: `PopupEntrance` (`Popups/Scripts/Presentation/PopupEntrance.cs`), a component on a popup prefab's root; `PopupMotionSettings` (`Popups/Settings/PopupMotionSettings.asset`); `PopupsInstaller` (`Popups/Installers/PopupsInstaller.asset`, in the Boot scope's list)

## How it works
- **Entrance.** `PopupEntrance` holds the popup's dimmer and an ordered list of parts. Each time the popup opens (`OnEnable`, as Core.UI activates a screen to open it), the dimmer fades from transparent to its alpha in the prefab and each part grows from `StartScale` of its size to its size, the next part `Stagger` seconds after the one before.
- **Parts per popup.** The settings popup and the level failed popup have one part, their panel. The level complete popup has no panel, as in the reference: its title, its large coin and its continue button (with the reward) pop one after the other.
- **Motion.** One DOTween sequence per opening, built from `PopupMotionSettings` as they are then, so a value changed in play mode shows on the next opening; the dimmer's alpha and the parts' scales at rest are taken once in `Awake`, so a popup closed mid-entrance opens again from its real rest; unscaled time; killed when the popup is disabled.
- **Settings.** The dim duration, the start scale, the pop duration and ease and the stagger are in `PopupMotionSettings`, registered in the Boot scope by `PopupsInstaller`, so every scene's popups share one tuning. Screens are created through the container ([UI](../../Base/Systems/UI.md)), so the component gets the settings with an `[Inject]` method.
- **Closing.** No close animation: the reference closes the settings popup at once, and the level popups close under the fade between levels ([Play](Play.md)).

- **Prefabs.** `Popups/Prefabs/PopupDimmer` is every popup's full-screen dimmer (the entrance fades it); `PopupPanel` is the popup body: the panel, the header ribbon with a navy title, the corner `CloseButton` and the darker inner box, which a popup fills with its own content (the failed popup's clock and message, the settings popup's switch rows) and sizes with overrides. The popups stay variants of Core's `UIScreenTemplate`: Unity does not change the base of an existing variant, so a shared popup base would mean rebuilding them ([Decisions](../Decisions.md)).

## Rules & invariants
- The dimmer is a raycast target from the first frame, while it fades in, so the board and the HUD take no touch during an entrance.
- A part's size in the prefab is its size at rest; the entrance only scales from below it to it.
- A part is never a button with press feedback: both set the scale, and the feedback would keep the entrance's start scale as its rest scale. A button that pops in sits inside a part of its own (the complete popup's `Continue` holds its continue button).

## Tests
- No EditMode test: the component is a tween on enable, which DOTween does not run in EditMode. Checked in the editor: pause (the settings popup pops in over a fading dim), win a level (header, reward and continue one after the other), let the time run out (the failed popup), open each popup a second time.
- `Tests/Popups/Presentation/PopupEntrancePartsTests`: no entrance part in the game's prefabs has a `PressFeedback`.
- `Tests/Bootstrap/InstallerAssetTests`: `PopupsInstaller` has its settings assigned.

## Related
- [Play](Play.md), [Preferences](Preferences.md), [Buttons](Buttons.md)
