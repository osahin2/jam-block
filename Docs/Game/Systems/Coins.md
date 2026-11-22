# Coins

> The coin balance, saved on every change, and how coins are shown: a counter prefab any screen nests and one app-wide flight of coins into it. One wallet and one flight in the Boot scope.

**Code:** `Logic/Coins/`, `Data/Coins/`, `Runtime/Coins/`. **Entry:** `CoinWallet` (`Add`, `TrySpend`), `CoinFlight.Play(from, to, amount, onLanded)`, `CoinCounter`.

## Rules
- `ICoinStore.Save` returns only once the balance is persisted; the wallet calls it on every change. Only `SavedCoinStore` (Core.Save) knows where it is kept.
- The balance never goes below zero; a negative amount or stored balance throws (a programmer error).
- Coins are added before any flight starts; the flight only shows them, so a game killed mid-flight keeps them.
- Each `Play` is its own flight, so rewards overlap. A flight is tied to its counter: when the counter's scene unloads, the flight dies with it and never calls back. Landing shares add up to the amount exactly (`CoinTally`).
- The flight's canvas scales like `GameUIRoot` (a test keeps them equal) and sorts above scene UI, below the fade.

Related: [App flow](AppFlow.md), [Boosters](Boosters.md)
