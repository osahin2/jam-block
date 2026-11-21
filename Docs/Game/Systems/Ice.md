# Ice

> Summary: The ice blocker from end to end: a number on a block that locks it until that many blocks have left the board (or boosters chip it off), the level rule that every ice can melt, the editor's ice field and drawing, and the `IceCover` view that turns a block to ice, cracks and melts.

## Code
- Folder: `Assets/Game/Logic/Play/Mechanics/Ice/` (rules), `Assets/Game/Logic/Levels/Validation/IceMeltsCheck.cs` (level rule), `Assets/Game/Runtime/Play/Scripts/Presentation/Blocks/IceCover.cs` (view), `Assets/Game/Runtime/Play/Scripts/Settings/IceSettings.cs` (tuning), `Assets/Game/Editor/LevelEditor/Scripts/Selection/` (the editor's field)
- Assembly: `Game.Data` (`BlockData.Ice`, `ShapeMiddle`), `Game.Logic`, `Game`, `Game.Editor`
- Entry points: `IceMechanic` (through `LevelMechanics.For(level).Ice`), `IceMeltsCheck` (in `LevelValidator`'s list), `IceCover` (a child of `Block.prefab`, reached as `BlockView.Ice`)

## How it works
- **Rule.** `BlockData.Ice` is the ice at the start (0: none). `IceMechanic` is a move rule and a guard ([Board](Board.md), Mechanics): a block with ice left cannot step or leave and takes a booster's hit as one ice. Its ice left is the starting ice minus the board's `GoneCount` minus its hits, never below zero, read when asked.
- **Locked.** `Board.IsLocked(block)` is true when no move rule allows any direction, so it covers every mechanic that stops a block entirely, not only ice. `LevelAttempt.Grab` holds no locked block: it starts the timer, returns true and gives `DragPose.Locked(block, cell)` (`IsLocked`), so `BoardInteraction` plays the block's refusal (`BlockView.Refuse`, a short sideways punch from `BlockMotionSettings`'s Locked values) instead of an outline.
- **Level rule.** `IceMeltsCheck` (`IceNeverMelts`, error): ice melts only by blocks gone, so with the blocks' ice sorted from the smallest, the i-th (0-based) must not exceed i. The check finds how many blocks can ever leave (the first i whose ice is above i, or every block) and reports each block whose ice is above that number: every block it locks for good, not only the first. Boosters are left out: a level must be solvable without them.
- **Editor.** The Ice tool (shortcut I): a click on a block adds one ice, a right click takes one off. A block's selection also has an Ice field (a `SelectionField`, a delayed number field; [Level editor](LevelEditor.md), Selections); a change records an undo step ("Set Ice") and goes through `LevelEdits.SetIce(block, ice)`, which keeps the ice at 0 or more. The board draws an iced block's cells under a pale ice tint, so its color still shows, and the count as a dark round badge in the corner of the block's top-left cell, so it never meets an arrow on the same block. The selection's label names the ice.
- **Middle.** `ShapeMiddle.Of(shape)` is where the count stands on the block in the game: the shape's center when the cell under it belongs to the shape, else the shape's cell nearest the center (an L with long arms).
- **View.** `BlockViewBuilder` builds every block view and then calls `IceCover.Show(ice, shape, blockMaterial, art)` with the palette material the pieces got: the level's starting ice for a staged block, none for a part appended at run time or a view rebuilt after a rocket hit (a block with ice left loses no cell) (the melt fades back to its colors), so the level editor's scene preview shows ice too. `IceCover` turns the block's pieces to ice with one `MaterialPropertyBlock` on their renderers: the ice colors of `IceSettings` in place of the block's color and shadow color, and the supplied `icetexture.jpg` as the `Game/Block` shader's surface map, laid in the board's space so it runs on across the block's cells (the pieces' UVs point into an atlas that was never supplied). A property block leaves the renderers' materials alone, so the outline's material arrays stay right. `Show(0)` takes the property block off, which also cleans a pooled piece that was iced in the level before. The count is a `TextMeshPro` in Lilita One with the Navy preset, lying on the block's top.
- **Catching up.** `PlaySession.Start` builds `LevelMechanics.For(level)` once, hands it to the attempt and its `Ice` to `BoardInteraction.Begin`. `BoardInteraction.CatchUp` gives each block view `IceMechanic.IceLeft` along with its `BoardBlock`; `IceCover.CatchUp(iceLeft)` compares with the count it shows: lower and above zero cracks (the new count, a punch on it, a few shards per cell), zero melts (the count shrinks away, the block fades from ice to its color over `MeltDuration` and the property block is taken off, more shards). Ice never rises, so a higher or equal count does nothing.
- **Shards.** One particle system under the cover, made from the door burst's (cubes, shrinking away) with local simulation, a one-cell box shape pointing up, gravity and no emission of its own; `IceCover` emits a number per cell at each cell's top with `EmitParams`, so the shards come from the whole block whatever its shape. Their material is `IceShard` (URP Lit with the ice texture).

## Rules & invariants
- The view never decides: the ice left comes from the logic on every catch up, and the count only goes down.
- A block without cells (an `EmptyBlock` level error, which the scene preview may still show) shows no ice; `ShapeMiddle.Of` needs at least one cell.
- The crack and the melt are tweens with the cover as their target, stopped with `DOTween.Kill(this)` before the next one, never through a kept handle.
- The ice texture is imported at 512 px: it is tiled at a quarter repeat per board unit and the full 2048 would cost memory for no visible gain.
- A block made at run time (a rocket's split) has no starting ice: its view must be shown with `Show(0)` so a pooled piece carries no ice from an earlier level.

## Tests
- `Tests/Levels/LevelValidatorTests`: ice that melts as blocks leave, ice on the only block, every block locked by too much ice, two blocks iced only by each other.
- `Tests/Levels/ShapeMiddleTests`: a square's center, an L's center, a shape whose center is off it.
- `Tests/Play/Mechanics/Ice/IceMechanicTests`, `Tests/Play/Board/BoardRoleTests`: an iced block is locked, a melted one and a block a rule stops in one direction only are not.
- `Tests/Play/Attempt/LevelAttemptTests`: a grab on a locked block holds nothing, names it in a locked pose, starts the timer and moves nothing.
- `Tests/LevelEditor/Selection/SelectionFieldsTests`: a block's Ice field reads and writes its ice, keeps its block when an earlier one is removed, never goes below zero; a door has no fields.
- `Tests/Play/Presentation/Blocks/IceCoverTests`: showing ice turns the pieces to ice and shows the count, showing none takes both off, a block without cells shows none, a lower count cracks, the same or a higher count changes nothing, zero melts. `BlockViewTests`: a refusal shakes from the block's cell. The look, the crack and the melt are checked by eye in play mode.

## Related
- [Board](Board.md), [Level attempt](LevelAttempt.md), [Levels](Levels.md), [Play](Play.md), [Level editor](LevelEditor.md), [Reference](../Reference.md) ("Boosters and ice"), [Decisions](../Decisions.md)
