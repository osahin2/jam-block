# Buttons

> One touch feedback for every button: it shrinks while pressed and springs back, with a `Light` haptic on the press, tuned in one settings asset in the Boot scope.

**Code:** `Runtime/Buttons/`. **Entry:** `PressFeedback` (next to any `Selectable`), the prefabs `TextButton`, `TextButtonGreen`, `IconButton`, `CloseButton`.

## Rules
- A button's only press visual is this component (Transition None); no screen animates its own buttons. A non-interactable button does not react.
- The release reaches the pressed object even when the finger slides off, so the button always comes back. `OnDisable` restores the rest size, so a popup closed by its own button reopens right.
- Unscaled time, one tween at a time.

Related: [Popups](Popups.md)
