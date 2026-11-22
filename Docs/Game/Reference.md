# Reference game

> Summary: Color Block Jam, the game the case asks for a vertical slice of: its mechanics, its screens and how it looks. The project takes mechanics and feel from it; art, names and level layouts are not taken, and the look is built from the supplied assets.

## Board
- A rectangular grid of square cells inside a wall frame. The first five levels are 4 x 5 to 5 x 6 cells with two to six blocks.
- Doors are colored segments of the frame, one to three cells long, each with an arrow pointing out of the board; the rest of the frame is wall.
- One color can have more than one door, and one side can hold doors of different colors.

## Blocks
- A block is a set of cells of one color: rectangles and L shapes. Its cells always stay together; only the rocket booster takes a cell away.

## Movement
- **Free drag.** A touched block follows the finger continuously, between cells too and in both axes at once. A held block gets an outline.
- **Collisions.** A dragged block stops at walls and other blocks; the finger can run ahead of it.
- **Release.** On release the block snaps onto the nearest whole cell.
- **No slide.** A flick does not throw the block; it moves only as far as it is dragged.

## Doors and exits
- A block leaves as soon as it touches a side at a door of its own color and its whole extent along that side lies within the door, from any drag direction. A block only partly under its door stays.
- A door of another color is wall.
- A block can rest against its door; a tap sends it out. A block held at its door by another block in its way leaves the same way once the way is free.
- Leaving: the block snaps in front of its door, the door drops, the block slides out and is cut away at the frame's edge while small cubes of its color burst from it, and the door rises again.

## Win and fail
- The level is won when the last block has left: a "level complete" screen shows the coin reward (40 coins on each of the first five levels) and continue starts the next level, as the coins fly into the counter.
- Each of the first five levels starts at 05:00. The timer starts with the first touch on the board and stops while a popup is open; it flashes red in the last 30 seconds.
- At 00:00 an "out of time" popup offers 20 more seconds for coins; closing it shows "level failed" with retry and a way home. A failed level is replayed from its start state.

## In-level controls
- HUD: level number, timer, restart, pause, coin count with a "+" for the shop, and four booster slots under the board.
- **Restart** asks for confirmation ("you will lose 1 life") and rebuilds the level with a full timer.
- **Pause** opens the settings popup with a home button; the timer is stopped while it is open.

## Home screen
- Top bar with avatar, lives, coins and settings; a vertical path of level nodes with the current one highlighted; a large "Level N" button; a tab bar of five tabs (shop, home, collection and two locked ones) with the selected tab raised.
- The level button opens a pre-level booster popup, then the level.

## Settings
- A popup from home and from inside a level: vibration, sound and music switches, placeholder buttons (legal terms, restore purchases, support, language) and, inside a level, a home button.

## Look
- Seen steeply from above. Blocks are flat plates with studs in saturated colors; walls and doors are tall rounded bars, a door in its block's color; the board sits on dark navy and slate blue under a light sky-blue background, with hardly any shadow.
- A held block gets a thin warm-white outline around its outer shape only.

## Boosters and ice
Seen on levels after the first five.
- **Ice.** An iced block cannot move. Its number drops by one for every block that leaves the board, however it goes, and for every hammer or rocket hit on it; at zero it plays as any other block. Later levels also put ice on doors.
- **Slots.** Freeze, rocket, hammer, vacuum, each with a stock; at zero a popup sells 3 for coins or 1 for watching an ad.
- **Aiming** (rocket, hammer, vacuum): the screen dims except the board and the active button; any tap that is not a target cancels; the stock goes down only when a shot lands; the timer keeps running.
- **Freeze** stops the timer for 10 seconds.
- **Rocket** blows away one cell; what remains splits into separate blocks where its cells no longer touch. On an iced block it takes one ice instead.
- **Hammer** breaks one block; on an iced block it takes one ice instead.
- **Vacuum** takes every block of the chosen color except iced ones.

## What the case takes from this
- In scope: free drag with collisions, exits through matching doors, win on an empty board, countdown with fail at zero, fail popup with restart and home, pause, restart, a coin reward, home with top UI, level button and placeholder tabs, settings with three switches.
- Placeholder or left out: lives (shown, not spent), the out-of-time offer, the pre-level booster popup, profile, shop, level path progress, the other tabs.
- Optional scope, built: the ice blocker on blocks, the four boosters with stock and purchase, the arrow block. Left out: ice on doors, buying by watching an ad.
