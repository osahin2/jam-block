# Preferences

> Summary: The settings popup: sound, music and vibration switches written straight to Core's audio and haptics services, which save them. The Game scene opens it as its pause; the Home screen opens the same popup in phase 5.

## Code
- Folder: `Assets/Game/Runtime/Preferences/` (`Scripts/UI/`, `Scripts/Installers/`; assets in `Prefabs/`, `Settings/`, `Installers/`)
- Assembly: `Game`
- Entry points: `SettingsPopup.Show(onClose, onHome)` (`Preferences/Scripts/UI/SettingsPopup.cs`), opened through Core's `IUIService`; `PreferencesInstaller` (`Preferences/Installers/PreferencesInstaller.asset`), which registers the screen catalog `Preferences/Settings/PreferencesScreens.asset`

## How it works
- **Popup.** `SettingsPopup` is a Core screen on the popup layer, with a full-screen dimmer under its panel. `Show` reads the current choices from `IAudioService` (`SfxMuted`, `MusicMuted`) and `IHapticService` (`Enabled`) into the switches and keeps the two callbacks. Close and home clear both callbacks, close the popup and call the chosen one, so a double tap does nothing.
- **Switches.** `SwitchView` is one on/off control: a button over a track with an on knob and an off knob, one of them shown. A click flips the shown state and tells its listener the new one; the popup's listeners write it to Core, which saves it ([Audio](../../Base/Systems/Audio.md), [Haptics](../../Base/Systems/Haptics.md)). There is no copy of the choices in the game.
- **As pause.** `GameEntry` pauses the play session and opens the popup with resume as close and `SceneFlow.ToHome` as home ([Play](Play.md)).
- **Its own feature.** The popup belongs to neither Play nor Home, which both open it, so it has its own folder, catalog and installer; a scene that opens it lists `PreferencesInstaller` (today the Game scene).

## Rules & invariants
- The popup shows Core's saved state every time it opens; a choice is saved the moment its switch flips.
- The popup decides nothing about the level: what close and home do is the caller's.

## Tests
- `Tests/Preferences/UI/SwitchViewTests`: on shows only the on knob, off only the off knob.
- Saving the choices is Core's, tested there. The popup is checked in the editor: pause a level, flip each switch, close and pause again (the switches keep their state), restart play mode (still kept).

## Related
- [Play](Play.md), [Buttons](Buttons.md), [UI](../../Base/Systems/UI.md), [Audio](../../Base/Systems/Audio.md), [Haptics](../../Base/Systems/Haptics.md)
