# Coins

> Summary: The player's coin balance in `Game.Logic`, saved after every change through a store it is given. Coins are only earned so far (the win reward); the balance never goes below zero. One wallet for the whole app, in the Boot scope.

## Code
- Folder: `Assets/Game/Logic/Coins/`, `Assets/Game/Data/Coins/`, `Assets/Game/Runtime/Coins/`
- Assembly: `Game.Logic`, `Game.Data`, `Game`
- Entry points: `CoinWallet` (`Logic/Coins/CoinWallet.cs`); `CoinsInstaller` (`Runtime/Coins/CoinsInstaller.cs`)

## How it works
- `new CoinWallet(store)` reads the balance once from an `ICoinStore`; `Balance` reads it; `Add(amount)` raises it and writes the new balance to the store at once.
- `ICoinStore` is the wallet's port for persistence, declared in `Game.Logic`; `SavedCoinStore` in `Game` implements it with Core.Save, one `CoinsDocument` (`Game.Data`) under the key `coins`. The logic stays engine-free and tests use an in-memory store.
- `CoinsInstaller` registers the store and one wallet in the Boot scope, so Home and Game show and change the same balance and it is the same after leaving a level ([App flow](AppFlow.md)).
- The reward amount is one global setting, paid when the player continues from the win screen ([Decisions](../Decisions.md)).

## Rules & invariants
- A negative stored balance or a negative amount throws `ArgumentOutOfRangeException`: both are programmer errors, not player actions. A rejected amount saves nothing.
- No spending yet: boosters, the shop and the out-of-time offer are out of scope. A spend rule (`TrySpend`, false without a change when the balance is short) comes with the first thing that costs coins, planned as the optional out-of-time offer.
- Only `SavedCoinStore` knows where the balance is kept.

## Tests
- `Tests/Coins/CoinWalletTests`: starting from the stored balance, add, add saves, a new wallet starts where the last one stopped, negative amount, negative stored balance.

## Related
- [Level attempt](LevelAttempt.md), [App flow](AppFlow.md), [Save](../../Base/Systems/Save.md)
