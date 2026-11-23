# Preferences

> The settings popup: sound, music and vibration switches written straight to Core's audio and haptics, which save them. Opened as pause in the Game scene (with a home button) and from Home's gear (without).

**Code:** `Runtime/Preferences/`. **Entry:** `SettingsPopup.Show(onClose)`, `ShowWithHome(onClose, onHome)`; `PreferencesInstaller` in every scene that opens it.

## Rules
- The game keeps no copy of the choices: the popup reads Core's state on every open and writes a choice the moment its switch flips.
- The home button shows exactly when there is a home action; without it the panel shortens.
- Close and home clear both callbacks before calling one, so a double tap does nothing. Home leaves the popup open, since the scene fade covers the screen first.
- The popup decides nothing about the level; what close and home do is the caller's.
- Its own feature, because both Play and Home open it.

Related: [Play](Play.md), [Home](Home.md), [Audio](../../Base/Systems/Audio.md), [Haptics](../../Base/Systems/Haptics.md)
