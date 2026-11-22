# Ice

> A number on a block that locks it until that many blocks have left the board; boosters chip one off. Rule in `Game.Logic`, a level check, an editor tool and the `IceCover` view.

**Code:** `Logic/Play/Mechanics/Ice/`, `Logic/Levels/Validation/IceMeltsCheck.cs`, `Runtime/Play/Scripts/Presentation/Blocks/` (`IceFeature`, `IceCover`), `Editor/LevelEditor/Scripts/` (`IceEdits`, `IceDrawing`). **Entry:** `IceMechanic` (via `LevelMechanics.Ice`), `IceFeature`.

## Rules
- Ice left = starting ice − `GoneCount` − hits on the block, never below zero, computed when asked, so nothing has to tell the ice a block went. A block with ice left is a move rule's "no" and a guard's "yes". Parts made by a split have no ice.
- `IceMechanic` implements `TakeHit` explicitly, so whoever shows the ice cannot melt it.
- `IceNeverMelts`: with the blocks' ice sorted ascending, the i-th must not exceed i. Boosters are left out: a level must be solvable without them.
- A grab on a locked block holds nothing and returns `DragPose.Locked`; the view shakes the block.
- `IceCover` uses a `MaterialPropertyBlock`, so the outline's material arrays stay intact; showing zero ice removes it, which also cleans a pooled piece. The count only goes down; the view reads it from the logic on every catch-up.
- The ice texture is imported at 512 px: it is tiled small, the full 2048 costs memory for no visible gain.

Related: [Board](Board.md), [Level editor](LevelEditor.md), [Play](Play.md)
