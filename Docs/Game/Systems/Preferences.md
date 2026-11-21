# Preferences

> Summary: The settings popup: sound, music and vibration switches written straight to Core's audio and haptics services, which save them. The Game scene opens it as its pause; the Home screen opens the same popup in phase 5.

## Code
- Folder: `Assets/Game/Runtime/Preferences/` (`Scripts/UI/`, `Scripts/Installers/`; assets in `Prefabs/`, `Settings/`, `Installers/`)
- Assembly: `Game`
- Entry points: `SettingsPopup.Show(onClose, onHome)` (`Preferences/Scripts/UI/SettingsPopup.cs`), opened through Core's `IUIService`; `PreferencesInstaller` (`Preferences/Installers/PreferencesInstaller.asset`), which registers the screen catalog `Preferences/Settings/PreferencesScreens.asset`

## How it works
- **Popup.** `SettingsPopup` is a Core screen on the popup layer, with a full-screen dimmer under its panel. As in the reference, the panel (a `PopupPanel`) holds one box with a row per switch (`Preferences/Prefabs/SwitchRow`: an icon and a `SwitchView`; each row overrides the icon; `SwitchSettings` is registered by `PreferencesInstaller`) (vibration, sound, music: an icon and the switch, no caption), the close button in its corner and the green home button under the box. `Show` reads the current choices from `IAudioService` (`SfxMuted`, `MusicMuted`) and `IHapticService` (`Enabled`) into the switches and keeps the two callbacks. Close and home clear both callbacks before calling the chosen one, so a double tap does nothing; close closes the popup first, home leaves it open, since the scene fade covers the screen before the scene goes and the dimmer never drops over the board.
- **Switches.** `SwitchView` is one on/off control, as in the reference: one knob, larger than the track, on a rounded track. Off, a pale knob sits over the track's left end on a navy track; on, the green knob with "On" (the supplied `btn_toggle_on`) sits over its right end on a dark green track, so the text shows only when on. `Show(isOn)` sets a state at once (opening the popup); a press flips it and slides the knob in one DOTween tween of a single progress value that moves the knob, cross-fades the two knob faces and blends the track's color (`SwitchSettings`: duration, ease, the two track colors; built once, no closure per press). The knob travels half the track's width less its height, so at either end it covers the track's rounded end; resizing the track in the prefab is enough. A click tells its listener the new state; the popup's listeners write it to Core, which saves it ([Audio](../../Base/Systems/Audio.md), [Haptics](../../Base/Systems/Haptics.md)). There is no copy of the choices in the game.
- **As pause.** `GameEntry` pauses the play session and opens the popup with resume as close and `SceneFlow.ToHome` as home ([Play](Play.md)).
- **Its own feature.** The popup belongs to neither Play nor Home, which both open it, so it has its own folder, catalog and installer; a scene that opens it lists `PreferencesInstaller` (today the Game scene).

## Rules & invariants
- The popup shows Core's saved state every time it opens; a choice is saved the moment its switch flips.
- The popup decides nothing about the level: what close and home do is the caller's.

## Tests
- `Tests/Preferences/UI/SwitchViewTests`: on puts the knob over the track's right end with the on face shown and the on track color, off over the left end with the off face and the off track color. The slide is checked in the editor (DOTween does not run in EditMode): flip each switch in the settings popup.
- Saving the choices is Core's, tested there. The popup is checked in the editor: pause a level, flip each switch, close and pause again (the switches keep their state), restart play mode (still kept).

## Related
- [Play](Play.md), [Buttons](Buttons.md), [UI](../../Base/Systems/UI.md), [Audio](../../Base/Systems/Audio.md), [Haptics](../../Base/Systems/Haptics.md)
