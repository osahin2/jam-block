# Level editor

> A UI Toolkit window where a designer draws levels on a top-down board. It edits a working copy, so undo covers every change and only a level without errors is written to its asset. Play starts the game with the level.

**Code:** `Editor/LevelEditor/`. **Entry:** `LevelEditorWindow` (menu **Game > Level Editor**, or double-click a level asset).

## Rules
- `LevelDocument` is the open level and decides what save, unsaved changes and Revert mean; every way of opening a level goes through one `Load`. The window adds only dialogs and views.
- The working copy (`LevelDraft`) records whole copies for undo: Unity's difference-based undo restored a list wrongly. One pointer stroke is one undo step, recorded only if it changed something.
- A new level becomes an asset only on its first save without errors, next to the catalog's last level, and is appended to the catalog. Delete moves the file to the trash.
- Tools are strategies (`ILevelEditorTool`) the window forwards to with no switch. `Press`, `Drag` and `RightClick` return whether the level changed (false for a press that only selects), so only real changes become undo steps; edits live in `LevelEdits`, `ArrowEdits` and `IceEdits`, plain code tested without a window. Tool cells run from -1 to the board's size: the ring outside is where doors go.
- A selection (`ILevelSelection`) holds the object, not its index, so removing an earlier one never retargets it. `MoveBy(offset)` is always relative to where the move began.
- Mechanics draw through `IBlockMechanicDrawing` in two layers: `DrawOnBlock` for what lies on the block's cells, then `DrawOnTop` for what must stay above every mechanic's drawing, so tint, arrow and badge stay readable together. Neither changes the level. A new mechanic adds a drawing to the list.
- Shortcuts use `[Shortcut]` and can be rebound; they are ignored while typing in a field. The focused element is then the field's inner text input, so the window looks for a field among it and its ancestors.
- Problems from `LevelValidator` are shown in plain words and marked on the board; clicking one selects its block or door.
- The scene preview uses the game's `LevelStage` with unsaved, non-editable copies and never dirties the scene. It loads its settings on first use, as the asset database may not be ready during a script reload.
- Play refuses with a reason (play mode, compile errors, no build scene) before writing anything. The request crosses the domain reload in `SessionState` and is taken once.
- After every change without errors the level is solved ([Solver](Solver.md)) for at most `SolverSettings.QuickCheckMilliseconds`, a drag once when it ends, so editing never stalls; a level left undecided offers Check fully, up to `FullCheckBudget` arrangements under a cancelable progress bar. The last solution is kept and the level solved again only when it changed.
- Show solution plays the fewest moves back on the board from a copy of the level; while it plays the board takes no edits (`LevelBoardView.TakesEdits`) and no selection is shown.
- Save, Play and close ask before saving an unsolvable level ("Save anyway" or "Keep editing"): the case asks for a warning, not a refusal. An undecided level is saved without asking.
- A stroke ends when the board loses the pointer capture, not on pointer up, so a stroke cut short still records its undo step.
- No IMGUI in the window.

Related: [Levels](Levels.md), [Play](Play.md)
