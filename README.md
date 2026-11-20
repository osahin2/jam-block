# rollic-case-study

## Third-party code
- [Quick Outline](https://github.com/chrisnolet/QuickOutline) by Chris Nolet, MIT license, in `Assets/Plugins/QuickOutline/` with its license file. The first commit that adds it holds the original files; the changes made for this project follow in their own commits and are listed at the top of `Outline.cs`.
- [UI Particle](https://github.com/mob-sakai/ParticleEffectForUGUI) by mob-sakai, MIT license, version 4.14.2 from OpenUPM (`com.coffee.ui-particle`): draws the win screen's confetti, a ParticleSystem, inside the UI canvas.
- [Lilita One](https://fonts.google.com/specimen/Lilita+One) by Juan Montoreano, SIL Open Font License 1.1, in `Assets/Plugins/LilitaOne/` with its license file and the TextMesh Pro font asset made from it: the font of every screen, as the supplied assets have none.

## Feedback on the supplied assets
The problems found in the supplied assets are listed in [Supplied assets](Docs/Game/SuppliedAssets.md). One of them changed the project: the block pieces are about 1.6 units deep for a plate a quarter of a cell thick, so the blocks had to be sunk into the ground, and the held block's outline, which works on the whole mesh, spilled below them. The project makes flat plates from the supplied pieces in Blender (`Tools/Blender/make_block_plates.py`, into `Assets/Art/Models/BlockPlates.fbx`); the supplied `BlockParts.fbx` is unchanged.
