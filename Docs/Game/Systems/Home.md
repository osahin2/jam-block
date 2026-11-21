# Home

> Summary: The home screen laid out after the reference: a top bar (avatar, lives, coins, settings gear), the level path with the current level and the locked ones above it, the level button, and a five-tab nav bar whose middle three tabs switch panels with an animated selection.

## Code
- Folder: `Assets/Game/Runtime/Home/` (`Scripts/UI/`, `Scripts/Presentation/Path/`, `Scripts/Presentation/Tabs/`, `Scripts/Settings/`, `Scripts/Installers/`; assets in `Prefabs/UI/`, `Prefabs/Path/`, `Prefabs/Tabs/`, `Settings/`, `Installers/`)
- Assembly: `Game`
- Entry points: `HomeScreen.Show(levelNumber, coins, onLevel, onSettings)` (`Home/Scripts/UI/HomeScreen.cs`), opened by `HomeEntry` ([App flow](AppFlow.md)); `HomeInstaller` (`Home/Installers/HomeInstaller.asset`): the screen catalog and `TabBarSettings`

## How it works
- **Screen.** `HomeScreen` shows the level number on the level button and the path, and the coin count on a `CoinCounter` ([Coins](Coins.md)) restyled for home (cream pill, "+" on its left, coin on its right). The level button calls `onLevel` (`SceneFlow.ToGame`), the gear `onSettings` (the settings popup without home, [Preferences](Preferences.md)). The avatar and the coins' "+" are placeholder buttons with press feedback; the lives pill ("Full" and 5 in a heart) is static, as lives are not spent ([Decisions](../Decisions.md)).
- **Layout.** Sizes follow the reference's home on a tall phone. The level button and the path hang from the safe area's bottom, so a taller screen adds room above the path, not under the button; the top bar hangs from the safe area's top. The tab bar's background, its dividers and the selected card run past the safe area's bottom to the screen's edge, so no strip shows under the bar on phones with a home indicator. The level button is the supplied green square sliced with sharper corners than the source (`pixelsPerUnitMultiplier` 2).
- **Level path.** One straight track (two gold rails over a blue band) from the level button to past the top of the screen, with the current level's node (`LevelNodeCurrent`, green) and two locked nodes above it (`LevelNode`, slate blue with a lock). The nodes are one prefab and a variant, placed in the screen; `LevelPathView.Show(current)` writes the numbers upward from the current level. The nodes do not take touches.
- **Tab bar.** Five cells on `bg_menu`: locked, shop, home, collection, locked ("Lvl 50"). The middle three are uGUI `Toggle`s in a `ToggleGroup` that does not allow switching off, with no press feedback; each has a `TabView`. As in the reference, the tab's width (its `LayoutElement` in the bar's horizontal layout, so the others narrow) and its card change at once when its toggle changes (the card is `ActiveTab`, wider than the cell by the sprite's soft shadow so its body fills the cell, with the supplied brick pattern `bg_block_texture` tinted over it and clipped inside its frame); only the icon moves, rising and growing on the tab that turns on and settling back on the one that turns off, while the new tab's label grows in from nothing and the old one's label hides at once. Each change starts fresh tweens from where the icon is, so a quick second tap never jumps. The tab saved as on (home) takes its look at once in `Start`, so the screen opens settled; Home is rebuilt with its scene, so returning from a level always lands on home. Locked tabs are `LockedTabView`s that shake their icon on a press and are never selected. Values in `TabBarSettings`.
- **Panels.** Each switching tab's `onValueChanged` sets its panel active in the screen prefab (a persistent listener to `GameObject.SetActive`), so panels swap at once, as in the reference; the home panel holds the path and the level button, shop and collection are instances of `ComingSoonPanel` with their tab's icon. The top bar stays over all of them.

## Rules & invariants
- Exactly one switching tab is on; the panel shown is the one of the tab that is on.
- Nothing on the screen holds game state: level number and coins come in through `Show`.

## Tests
- `Tests/Buttons/Presentation/ScreenButtonsTests`: every tab in the game's prefabs takes touches; every switching tab has a `TabView`.
- `Tests/Bootstrap/PrefabReferencesTests` and `InstallerAssetTests` cover the screen's and the installer's references.
- The selection motion, the panel swap and the locked shake are checked in the editor (DOTween and persistent listeners do not run in EditMode): open Home, tap shop, collection and home, then a locked tab. In edit mode a `Toggle` sends its value again on a canvas update, which restarts the label's growth; batch renders complete the tweens after the canvas update.

## Related
- [App flow](AppFlow.md), [Preferences](Preferences.md), [Coins](Coins.md), [Buttons](Buttons.md), [Reference](../Reference.md)
