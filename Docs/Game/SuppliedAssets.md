# Supplied assets

> Summary: What the case supplied under `Assets/Art/`, how the models fit the board grid, and the problems found in them. Measured in Unity 2022.3 from the imported meshes: bounds, face directions and renders.

## What arrived
| Folder | Content | Import |
|---|---|---|
| `Assets/Art/UI/` | 47 PNG: backgrounds, popup parts, buttons, icons (coin, heart, clock, boosters, tabs, settings) | Sprite, single, no mipmaps |
| `Assets/Art/Models/` | `BlockParts.fbx`, `WallAndDoor.fbx`, `GroundGrid.fbx`, `Arrows.fbx` (Blender exports) | defaults; materials import as URP Lit |
| `Assets/Art/Textures/` | `icetexture.jpg` (2048 x 2048) | default |

The folder and the zip next to it have the same content. `Arrows.fbx` and `icetexture.jpg` belong to optional mechanics (arrow block, ice blocker).

## How the models fit the grid
Units below are model units after import. The board is read as one cell = 2 units:
- `Door_1x1` is 2 units long; the `GroundGrid` tile is 1.95 units square (a 2-unit cell with a gap); the arrows are 1.6, 3.5 and 5.4 units long, i.e. 1, 2 and 3 cells minus a margin.

**Blocks** are built from 1-unit quarter pieces, four per cell, one stud each. The top faces `-Z`.
| Piece | Extent | Shape |
|---|---|---|
| `ModularBlock_Center` | (0,0)-(1,1) | top plate, no rim |
| `ModularBlock_Edge` | (0,0)-(1,1) | rim on the `+Y` side |
| `ModularBlock_OuterCorner` | (0,0)-(1,1) | rim on `+X` and `+Y`, rounded corner |
| `ModularBlock_InnerCorner` | (-1,-1)-(1,1) | three studs around a rounded concave corner at `+X +Y` |

- The pivot of the 1-unit pieces is the corner at (0,0), which is the cell's center when the piece is the cell's upper-right quarter; the other three quarters are the same piece turned in 90 degree steps around the cell center.
- A quarter's piece follows from its two side neighbours and the diagonal one: no side neighbour gives an outer corner, one gives an edge, both with the diagonal give the center.
- `ModularBlock_InnerCorner` is centered on the concave corner point of an L block and covers the three quarters around it, which then get no piece of their own.
- A 1 x 1 block is four outer corners. Any rectangle or L shape can be built.

**Frame** pieces are bars with a rounded closed end at `-Y` (the visible cap) and an open end at `+Y`; the cap sits 0.35 units lower than the block top.
| Piece | Footprint |
|---|---|
| `Wall` | 1 x 1 unit (half a cell) |
| `Door_1x1` | 2 x 1 units (one cell) |
| `Corner_1`, `corner_5` | 1 x 1, `corner_5` with a rounded outer corner |
| `corner_3` | L of three 1-unit squares, rounded outer corner |
| `corner_3.002` | 2 x 2 with one rounded corner |
| `corner_4` | 1 x 1, 1.9 units longer than the other bars |
| `Door_Arrow` | 0.4 units, a separate arrow for a door |

**Ground**: `GroundGrid` is one rounded tile per cell, facing `+Y`.

**Colors**: none are supplied. Every block piece uses one grey material (`ModularBlock_Base`); the door uses one white material named `Gate_Orange`; no texture is supplied for either. Block and door colors come from the game's own color settings, keyed by the level's color ids.

## Problems found
1. **Axis conventions differ between files.** Block pieces and arrows face `-Z`, the ground tile faces `+Y`, the frame bars face `-Y`. Each file needs its own rotation to sit on one board plane; one convention (and the same export settings) for all four files would let them be placed together without per-file fixes.
2. **No block or door colors.** One grey block material and one white door material, so the color set of the levels has to be chosen in the project. The door material's name (`Gate_Orange`) does not match its color.
3. **Only a one-cell door.** Doors of two and three cells (as in the reference) have no mesh; they are made by repeating `Door_1x1`.
4. **The door arrow is inside the door.** `Door_Arrow` sits 1.85 units below the door's cap, inside the door body, so with the opaque door material it is not visible; it has to be moved onto the cap.
5. **Missing and stray references.** `WallAndDoor.fbx` references a texture by an absolute path on the author's machine; the file is not supplied. The UVs of all pieces point into an atlas that is not supplied either.
6. **Frame pieces without a guide.** Five corner variants, one with a Blender duplicate name (`corner_3.002`) and one longer than the rest (`corner_4`); which corner goes where is not documented.
7. **Mixed export units.** `GroundGrid` and `Arrows` are exported in centimetres (file scale 0.01), the other two in metres. Unity converts both, so sizes match after import.
8. **Toggle has only an "on" state.** `btn_toggle_on` is a green knob with "On"; there is no off knob and no track. The case requires a different visual for on and off.
9. **Other UI gaps.** One tab background (`ActiveTab`) and no inactive one; no font; no sound or music clips for the sound and music toggles; no texture for the block break effect.
10. **The frame material does not render as its color.** `Base_Side` has a grey base color but emission switched on, and it shows turquoise under URP lighting; the project uses its own frame material.
11. **UI sizes and names.** `bg_home_screen` is 2048 x 2048 for a portrait screen; `bg_block_texture` is 1547 x 3357 (not a power of two); the booster icons are 1024 x 1024 for small buttons. Names mix prefixes and case (`Btn_`, `btn_`, `bg_`, `Blockjam_`, `Lego_Brickv1`). No nine-slice borders are set, which is normal for a delivery and done in the project.
