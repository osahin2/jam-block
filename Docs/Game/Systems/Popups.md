# Popups

> One entrance for every popup: the dimmer fades in and the parts grow in one after the other, tuned in one settings asset in the Boot scope. Popups close without animation, as in the reference.

**Code:** `Runtime/Popups/`. **Entry:** `PopupEntrance` (on a popup's root), the prefabs `PopupDimmer` and `PopupPanel`.

## Rules
- The dimmer takes touches from the first frame, so nothing under it reacts during the entrance.
- Rest scales and alpha are read once in `Awake`; the sequence is built on every opening from the settings as they are then.
- A part is never a button with press feedback: both set the scale. A button that pops in sits inside a part of its own.
- Popups stay variants of Core's `UIScreenTemplate`; Unity cannot rebase an existing variant, so shared parts are nested prefabs.

Related: [Buttons](Buttons.md), [Play](Play.md)
