# Communication and dependency injection

> Summary: VContainer wires everything; dependencies arrive through one constructor. Systems reach each other through explicit contracts, in a fixed order of preference. An event bus is for facts that unrelated observers watch, never for flow; none is built until such an observer exists.

## How A reaches B
Take the first option that fits:

1. **Return value.** If the caller needs to know what happened, the operation returns it.
2. **A feature's entry class.** Another feature starts it and gets its result back through a return value or the callback it passed (`PlaySession.Start(level, onEnded)`).
3. **Direct call on an injected contract.** Commands and queries: "play this sound", "save", "what is the current level".
4. **One orchestrator for a multi-step flow.** When things must happen in order, one class owns the sequence and reads top to bottom. Order is never left to subscription order.
5. **A notification on the owning contract.** Only for state with several writers and several readers (a currency balance). The subscriber injects the owning contract, so the edge stays visible in its constructor.
6. **Event bus.** For facts in the past tense (`LevelCompleted`, `CoinsSpent`) watched by observers the publisher must not know about.

Why a bus comes last: a `Publish` does not say who reacts, ordering between subscribers is implicit, and subscriptions bring lifetime bugs. The cost is paid on every read of the code, so the decoupling has to be worth it.

## When an event bus fits
Use it when all of these hold:
- The observers are cross-cutting and unrelated to the publisher: analytics, achievements and quest progress, tutorial triggers, debug overlays, features switched on and off (live events, experiments).
- The publisher needs no answer and does not care whether anyone listens.
- The order between observers does not matter.

Never for:
- Flow: "won, then the result popup, then the next level" is a sequence; one orchestrator owns it.
- Requests that need an answer: a direct call.
- State with one owner (a coin balance shown in the HUD): a notification on the owning contract (option 5).
- Per-frame or high-frequency data (drag positions, timer ticks).

Shape when the first real case appears (a `Core.Events` module, skill `add-core-module`): a small typed bus with struct messages, synchronous delivery on the main thread, `IDisposable` subscriptions, one instance per lifetime scope (never static). Preferred over MessagePipe, whose async handlers, filters and request/response would have no consumer and which adds two packages.

## Dependency injection (VContainer)
- **One constructor, no `[Inject]` on it.** `[Inject]` methods only on MonoBehaviours, which cannot have constructors.
- **No static singletons, no service locator.** `IObjectResolver` is used only inside factories registered in a scope, never stored in a service.
- **Composition roots live in the game.** A root `LifetimeScope` in the boot scene holds app-wide services; a child scope per content scene holds scene objects and runs the feature installers.
- **Interfaces at boundaries.** Core services are interfaces with internal implementations. Inside the game, classes are concrete until a second implementation or a test fake exists.
- **Async only where something is awaited** (a scene load), with UniTask and a `CancellationToken`. Everything else is synchronous; a flow reacts to input, animation ends and callbacks instead of awaiting.
- **Stripping:** nothing is needed at the default IL2CPP stripping level. Before raising it, add the VContainer source generator or a `link.xml`.

For the mapping from Zenject concepts, see [VContainer for Zenject users](../Base/Architecture/VContainer-for-Zenject-users.md).
