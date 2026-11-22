# Case requirements

> Summary: What the case asks for, condensed from the case PDF ("Game Developer Case 2026", 8 pages) so that sessions do not need the PDF. When in doubt, the PDF wins; record any interpretation in [Decisions](Decisions.md).

## Goal and constraints
- A small vertical slice of Color Block Jam (the reference game; mechanics only, see [Reference](Reference.md)): a playable level flow and a level editor.
- Evaluated: code quality, architecture, editor usability. Art is not evaluated.
- Unity **2022.3.62f2**. Reference resolution **1080 x 1920 portrait**.
- Use the supplied assets (Google Drive folder linked in the PDF); do not buy or import other art unless absolutely necessary.
- Any LLM tool is allowed; the README must say which tools were used for which parts.
- Time: 7 days.

## Deliverables (one link: Git repository or archive)
1. The Unity project: `Assets/`, `Packages/`, `ProjectSettings/`; not `Library/`, `Temp/`, `Build/`.
2. `README.md` (see below).
3. An Android `.apk` **or** a screen recording of at most 3 minutes showing the full flow.

## Required scope and acceptance criteria
### Home screen
- Tab navigation bar with placeholder tabs (the other sections are not implemented), level button, top UI, settings button. Other buttons give visual feedback on touch and need no function.
- [ ] The level button starts the current level.
- [ ] The settings button opens the settings screen.
- [ ] Layout correct at 1080 x 1920 and still correct on a 20:9 device.

### Settings
- All buttons are placeholders but each gives visual feedback. Vibration, sound and music toggles show their on/off state.
- [ ] Each toggle shows a different visual state for on and off.
- [ ] The player can close the screen and return home.

### Gameplay
- The first 5 levels, built from the supplied assets. Working timer, coin inventory, fail popup, pause, restart, home navigation. Boosters need no function; the life system can be a placeholder.
- [ ] The player can complete a level and continue to the next.
- [ ] The timer counts down and stops the level at zero.
- [ ] The fail popup opens when the level fails, with a way to restart and a way to go home.
- [ ] Restart sets the level back to its start state; nothing stays from the previous attempt.
- [ ] The coin count stays correct after leaving the level and returning.

### Level editor (for non-technical users)
- Controls board width and height, door placement, block placement, timer value.
- [ ] A designer can make a new level, save it and play it without writing code or editing a file by hand.
- [ ] The editor shows the level visually, not only numeric fields.
- [ ] The saved data has one format, and the game reads the same format.
- [ ] The 5 levels come from this editor.

## Optional scope (only after the required scope)
Level validation (warn before saving an unsolvable level), functional boosters, ice blocker, arrow block.

## Evaluation, in order of weight
1. Architecture: separation of data, logic and view; low coupling; easy to extend.
2. Editor usability: a designer can use it without help.
3. Code quality: clear names, no dead code, consistent style.
4. Correctness: the acceptance criteria above.
5. Performance: no unnecessary allocation in the update loop; stable frame rate on a mid-range device.
6. Scope control: small and complete beats large and incomplete.

## README must contain
How to open and run the project; how to open the editor and make a level; architecture decisions and the reason for each; known problems and incomplete parts; LLM tools used and for which parts; approximate work time.

## Notes from the case
Follow industry best practices. Report problems found in the supplied assets (feedback welcome). When a requirement is unclear, decide, continue, and write the decision in the README; the Talent Team can also be asked via LinkedIn or email.
