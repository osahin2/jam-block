# Coins

> Summary: The player's coin balance in `Game.Logic`, saved after every change through a store it is given, and how coins are shown: a counter prefab for any screen and one coin flight for the whole app that shows coins already paid flying into a counter. Coins are only earned so far (the win reward); the balance never goes below zero. One wallet and one flight, in the Boot scope.

## Code
- Folder: `Assets/Game/Logic/Coins/`, `Assets/Game/Data/Coins/`, `Assets/Game/Runtime/Coins/`
- Assembly: `Game.Logic`, `Game.Data`, `Game`
- Entry points: `CoinWallet` (`Logic/Coins/CoinWallet.cs`); `CoinFlight.Play(from, to, balance, onLanded)` (`Runtime/Coins/Scripts/Presentation/CoinFlight.cs`); `CoinCounter` (`Runtime/Coins/Prefabs/CoinCounter.prefab`, a counter any screen nests); `CoinsInstaller` (`Runtime/Coins/Scripts/Installers/CoinsInstaller.cs`)

## How it works
- `new CoinWallet(store)` reads the balance once from an `ICoinStore`; `Balance` reads it; `Add(amount)` raises it and writes the new balance to the store at once.
- `ICoinStore` is the wallet's port for persistence, declared in `Game.Logic`; `SavedCoinStore` in `Game` implements it with Core.Save, one `CoinsDocument` (`Game.Data`) under the key `coins`. The logic stays engine-free and tests use an in-memory store.
- `CoinsInstaller` registers the store and one wallet in the Boot scope, so Home and Game show and change the same balance and it is the same after leaving a level ([App flow](AppFlow.md)).
- **Counter.** `CoinCounter` shows a number of its own: `Show(coins)` writes it, `Land(coins)` adds landed coins to it and punches the coin icon, so flights that overlap each bring their own share. It lags behind the wallet while coins fly to it, and every screen's `Show` gives it the real balance. The prefab holds the pill, the coin icon, the count and the "+" (an `IconButton` with no function yet).
- **Flight.** `CoinFlight` is a presentation service: its prefab (`Runtime/Coins/Prefabs/CoinFlight.prefab`) is an overlay canvas of its own, sorted above every scene's UI (500) and below the scene fade (1000), scaled like `GameUIRoot` (a test keeps the two scalers equal), made once by `CoinsInstaller` in the Boot scope and kept across scenes (`DontDestroyOnLoad`). `Play(from, to, amount, onLanded)` makes a flight of its own on every call, so rewards that come one after another fly at the same time: coin images from a pool (Core's `PrefabPool`, filled for one flight, kept for a few) pop out at `from`, scatter, fly one after the other to the counter along an arc (different eases across and up the screen), shrinking to the size of the counter's icon as shown, since a screen may scale its counter, and each landing adds its share of `amount` to the counter (`CoinTally.Share`, the shares adding up to the amount exactly) and goes back to the pool; `onLanded` follows the last landing. A flight is linked to its counter: when the counter is destroyed with its scene, the flight dies with it, gives its coins back and never calls back, so nothing reaches a scene or a flow that is gone. Count, scatter, flight and punch are in `CoinFlightSettings`. The caller passes where the coins start and which counter they reach, so the flight looks nothing up.
- The reward amount is one global setting (`WinRewardSettings`, [Play](Play.md)), paid when the player continues from the level complete popup, also in a run started from the level editor ([Decisions](../Decisions.md)). The HUD shows the balance each time a level starts.

## Rules & invariants
- A negative stored balance or a negative amount throws `ArgumentOutOfRangeException`: both are programmer errors, not player actions. A rejected amount saves nothing.
- No spending yet: boosters, the shop and the out-of-time offer are out of scope. A spend rule (`TrySpend`, false without a change when the balance is short) comes with the first thing that costs coins, planned as the optional out-of-time offer.
- Only `SavedCoinStore` knows where the balance is kept.
- The flight only shows coins: the caller adds them to the wallet, which saves at once, before it starts, so a game killed mid-flight keeps them ([Decisions](../Decisions.md)).

## Tests
- `Tests/Coins/CoinWalletTests`: starting from the stored balance, add, add saves, a new wallet starts where the last one stopped, negative amount, negative stored balance.
- `Tests/Coins/Presentation/CoinTallyTests`: the shares of all landings add up to the amount, also when it does not split evenly or is smaller than the coin count; no share is negative; two interleaved flights end on the sum of both amounts.
- `Tests/Coins/Presentation/CoinFlightTests`: the flight's canvas scales like `GameUIRoot`.
- The flight is checked in the editor: win a level and continue (the coins fly over the popup into the HUD's counter, which counts up), quit play mode mid-flight and start again (the coins are kept).

## Related
- [Level attempt](LevelAttempt.md), [App flow](AppFlow.md), [Save](../../Base/Systems/Save.md)
