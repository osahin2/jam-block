# Coins

> Summary: The player's coin balance in `Game.Logic`. Coins are only earned so far (the win reward); the balance never goes below zero.

## Code
- Folder: `Assets/Game/Logic/Coins/`
- Assembly: `Game.Logic`
- Entry points: `CoinWallet` (`Logic/Coins/CoinWallet.cs`)

## How it works
- `new CoinWallet(balance)` starts from a saved balance; `Balance` reads it; `Add(amount)` raises it.
- The reward amount is one global setting, paid when the player continues from the win screen ([Decisions](../Decisions.md)).

## Rules & invariants
- A negative starting balance or a negative amount throws `ArgumentOutOfRangeException`: both are programmer errors, not player actions.
- No spending yet: boosters, the shop and the out-of-time offer are out of scope. A spend rule (`TrySpend`, false without a change when the balance is short) comes with the first thing that costs coins, planned as the optional out-of-time offer.
- Saving the balance through Core.Save is the `Game` side's job (phase 4); the wallet does not know where its balance is kept.

## Tests
- `Tests/Coins/CoinWalletTests`: starting balance, add, negative amount, negative balance.

## Related
- [Level attempt](LevelAttempt.md), [Save](../../Base/Systems/Save.md)
