# Reference game

> Summary: How the reference game plays, observed on an Android 10 device (Play Store build 12.2.0, 2026-11-16) over `adb` through its first five levels. Mechanics and flow only: no art, names or level layouts are taken into the project. The open decisions in [Plan](Plan.md) are argued from these notes.

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
- On leaving, the block first snaps onto the door at once, lined up with it, wherever the finger had it; after a short pause the door breaks the block apart piece by piece as it passes. (Seen by the developer in play; too quick for the screenshots.)
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

## What the case takes from this
- In scope: free drag with collisions, exit through a door of the block's color that covers the block's extent, win when the board is empty, countdown timer with fail at zero, fail popup with restart and home, pause, restart, a coin reward on win, home with top UI, level button and placeholder tabs, settings with three toggles.
- Placeholder or left out: lives (shown, not spent), the out-of-time offer, the pre-level booster popup, boosters, profile, shop, level path map, the other tabs.
