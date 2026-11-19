# Reference game

> Summary: How the reference game plays, observed on an Android 10 device (Play Store build 12.2.0, 2026-11-16) over `adb` through its first five levels, and how it looks and feels. Mechanics and feel only: no art, names or level layouts are taken into the project; the look is built from the supplied assets. The open decisions in [Plan](Plan.md) are argued from these notes.

Cells are counted from column 0 on the left and row 0 at the top.

## Board
- A rectangular grid of square cells inside a wall frame. The first five levels are 4 x 5 to 5 x 6 cells with two to six blocks.
- Doors are colored segments of the frame. A door covers a run of edge cells on one side (one to three cells on these levels) and shows an arrow pointing out of the board. The rest of the frame is wall.
- One color can have more than one door (two doors of one color on opposite sides, for example), and one side can hold doors of different colors.

## Blocks
- A block is a set of cells of one color: rectangles (1 x 2, 1 x 3, 2 x 2, 3 x 1) and L-shaped tetrominoes on these levels. Cells of a block always stay together.

## Movement
- **Free drag.** The player touches a block and drags it; the block follows the finger continuously, between cells as well, in both axes at once (a diagonal drag moves it diagonally). A held block gets an outline.
- **Collisions.** A dragged block stops at walls and at other blocks; it cannot pass through either, and the finger can run ahead of it.
- **Release.** On release the block snaps onto the nearest whole cell at once, without a visible glide.
- **No slide.** A quick flick does not throw the block; it moves only as far as the finger drags it.

## Doors and exits
- A block leaves the board as soon as it touches a side of the board at a door of its own color **and** its whole extent along that side lies within the door. Touching is enough: a block slid sideways along an edge until it lines up with its door also leaves.
- A block that touches its door with only part of its extent stays (a 2-wide block half under a 2-wide door does not leave).
- The extent counts along the door's side only: a 3 x 1 block leaves through a door one cell tall on a left or right side.
- A door of another color is wall for the block.
- Leaving: the block slides out through the door and breaks into small pieces; the drag ends.
- On leaving, the block first snaps onto the door at once, lined up with it, wherever the finger had it; the door drops and the block slides out, cut away at the door line as it passes (see "Look and feel").
- A block can start touching its door; it stays until the player touches it, and a tap, without a drag, makes it leave. A block held at its door by another block in its way leaves the same way once the way is free. (Seen by the developer in the reference.)
- Not observed: a block wider than every door of its color (the levels never had one). It could never leave, so the editor should warn about it.

## Win
- The level is won when the last block has left. The timer stops and a "level complete" screen shows the coin reward and a continue button; the coins fly into the counter on continue.
- The reward was 40 coins on each of the first five levels.
- Continue starts the next level directly, without going through the home screen.

## Timer
- Each of the first five levels starts at 05:00, shown as minutes and seconds.
- The timer starts with the first touch on the board, not when the level opens.
- The timer stops while a popup is open (restart confirmation, settings).

## Fail
- At 00:00 the board stops and an "out of time" popup offers 20 more seconds for coins (and an in-app purchase offer). Closing it shows "level failed": one life lost, a retry button and a close button (back to the home screen).
- A failed level is replayed from its start state.

## In-level controls
- HUD: level number, timer, restart button, pause button, coin count with a "+" (shop), and a row of locked booster slots under the board.
- **Restart** asks for confirmation ("you will lose 1 life"; retry or close); retry rebuilds the level in its start state with a full timer.
- **Pause** opens the settings popup over the level (vibration, sound and music toggles, placeholder buttons, a home button). Home asks for the same life-loss confirmation, then leaves to the home screen. The timer is stopped while the popup is open.

## Home screen
- Top bar: profile avatar, lives with a refill countdown and a "+", coins with a "+", settings button.
- Middle: a vertical path of level nodes (current level highlighted, later ones locked) and a large "Level N" button.
- Bottom: a tab bar of five tabs with the home tab raised in the middle; the others are shop, collection and two locked tabs.
- The level button opens a pre-level popup (booster selection, play); play starts the level.
- Coins and lives persist when leaving a level and returning.

## Settings
- A popup, from home and from inside a level: vibration, sound and music toggles, then placeholder buttons (legal terms, restore purchases, support, language). Inside a level it also has a home button.
- A toggle shows its state by knob side and color: off is a grey knob on the left of a dark track; on is a green knob on the right labelled "On".
- Closed with an "x" in the corner.

## Look and feel
**Board**
- Seen steeply from above with a wide-angle look: the far rows are noticeably narrower than the near ones. The board with its frame fills nearly the screen's width.
- The frame reads as a few smooth pieces, not one per cell: a wall between two doors, or between a door and a corner, is one rounded bar, and each corner is a rounded piece of its own.
- A door is one bar as long as its cells, in its block's color, a little thicker than the wall so it stands out on the side walls, with one small light arrow on its top at the middle pointing out of the board.
- Walls and doors are tall next to the blocks, about three times a block's height and well under half a cell thick; a block beside a wall sits clearly below its top.
- Blocks are flat plates, roughly a quarter of a cell thick, with studs on top (two per cell along each side), saturated colors (cyan, royal blue, purple, green, orange, yellow, red) and a light rim along their top edges.
- Colors overall: a dark navy frame, slate-blue ground cells on a lighter blue base that shows as thin lines between them, a light sky-blue background with a soft gradient. Light is bright and soft, with hardly any shadow on the board.

**Held block**
- A thin warm-white outline around the block's outer shape only (it follows an L's inner corner and draws nothing inside the block); gone on release. The block is not lifted or scaled.

**Leaving**
- The block snaps onto the cell in front of its door at once, and the door drops into the frame right away, leaving a flat strip of its color.
- The block slides out, starting slowly and speeding up, and is cut away at the door line as it passes: what is past the line is gone, the rest stays whole. A longer block takes longer to pass.
- Small cubes in the block's color burst from the door line while it passes, thrown outward and up with some spin and a short puff of dust, and shrink away within a second.
- The door rises again just after the block is gone. The whole exit takes a little over half a second.

**Win and next level**
- Shortly after the last block is gone the screen dims (the board stays visible, no panel), the title pops in with an overshoot over a coin pile with turning light rays, confetti bursts from the lower corners, and the continue button pops in a moment later.
- On continue, coins burst from the button and fly in an arc to the HUD's coin counter, which counts up as they land; the screen then fades to black and the next level fades in.

**Timer and fail**
- In the last 30 seconds the time box flashes red every second.
- At zero the screen dims and the out-of-time offer pops in (left out, see Fail); the level failed popup that follows grows in with an overshoot.

**Popups**
- The settings popup opens with a short pop over a dim and closes without an animation.

## What the case takes from this
- In scope: free drag with collisions, exit through a door of the block's color that covers the block's extent, win when the board is empty, countdown timer with fail at zero, fail popup with restart and home, pause, restart, a coin reward on win, home with top UI, level button and placeholder tabs, settings with three toggles.
- Placeholder or left out: lives (shown, not spent), the out-of-time offer, the pre-level booster popup, boosters, profile, shop, level path map, the other tabs.
