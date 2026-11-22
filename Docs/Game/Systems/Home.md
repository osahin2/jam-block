# Home

> The home screen after the reference: top bar (avatar, lives, coins, settings), the level path, the level button and a five-tab nav bar whose middle tabs switch panels.

**Code:** `Runtime/Home/`. **Entry:** `HomeScreen.Show(levelNumber, coins, onLevel, onSettings)`, opened by `HomeEntry`; `HomeInstaller`.

## Rules
- The screen holds no game state: level number and coins come in through `Show`. Avatar, lives and the coins' "+" are placeholders, as the case allows.
- Sizes follow the reference on a tall phone in height-matched units. Path and level button hang from the safe area's bottom; the tab bar runs to the screen's edge so no strip shows under it.
- Tabs are `Toggle`s in a group that cannot switch off; exactly one is on, and its panel is the one shown. Width and card change at once, only icon and label tween, as in the reference. Locked tabs only shake.
- Panels swap through each toggle's persistent listener in the prefab.

Related: [App flow](AppFlow.md), [Preferences](Preferences.md), [Coins](Coins.md)
