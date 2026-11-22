# Arrow block

> A block that moves only along its arrow's axis, both ways, and leaves only through a door on that axis. The designer draws the arrow in the editor; the view shows the supplied arrow where it was drawn.

**Code:** `Logic/Play/Mechanics/Arrow/`, `Editor/LevelEditor/Scripts/` (`ArrowTool`, `ArrowEdits`, `ArrowDrawing`), `Runtime/Play/Scripts/Presentation/Blocks/` (`ArrowFeature`, `BlockArrow`). **Entry:** `ArrowMechanic` (via `LevelMechanics.Arrow`), `ArrowTool`, `ArrowFeature`.

## Rules
- A move rule only: it never guards a block or locks it. With ice, the ice locks first.
- The axis decides the movement; where the arrow lies decides only which part of a rocket's split keeps it. `ArrowOf` gives a part the level block's arrow when the part holds all its cells, so a hit on an arrow cell takes the arrow off.
- `ArrowData.LiesOn(shape)` is the one rule the validator (`ArrowOffBlock`) and the editor's edits share. Erasing a cell under the arrow removes it.
- The tool draws at most 3 cells, the longest supplied arrow; a test holds this to the prefab.
- `Allows` and `ArrowOf` do not allocate (asked every drag frame).

Related: [Board](Board.md), [Ice](Ice.md), [Level editor](LevelEditor.md)
