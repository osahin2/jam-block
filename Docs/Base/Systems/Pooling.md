# Pooling

> Summary: Reuses instances of one prefab instead of instantiating and destroying them: idle instances wait inactive under a root, `Get` hands one out active under a parent, `Release` takes it back. A thin layer over `UnityEngine.Pool.ObjectPool<T>`.

## Code
- Folder: `Packages/com.base-setup.core/Runtime/Pooling/`
- Assembly: `Core.Pooling` (no references beyond the engine)
- Entry points:
  - `PrefabPool<T> where T : Component` ([PrefabPool.cs](../../../Packages/com.base-setup.core/Runtime/Pooling/PrefabPool.cs)): `PrefabPool(prefab, root, prewarmCount, maxSize)`, `T Get(Transform parent)`, `Release(T instance)`, `Clear()`.

## How it works
- **A concrete generic utility, not a service.** There is no interface and no installer, because it has no side effects beyond its own objects and every owner needs its own pool: the owner of a pool (a spawner, an audio service) constructs it with its prefab and root and keeps it. A pool's lifetime is its owner's lifetime.
- **Prewarm.** The constructor creates `prewarmCount` instances under `root` and leaves them inactive. Prewarming never activates an instance, so `OnEnable` does not run until the first `Get`; `Awake` runs at creation, as with any `Instantiate`.
- **Get** takes an idle instance (or creates one if none is idle), reparents it under `parent` (`null` means the scene root), resets its local position, rotation and scale to the prefab's, then activates it. The result matches `Instantiate(prefab, parent)`, so a reused instance never carries over a pose from its last use (for example a scale tweened to zero on despawn). Reparenting happens before activation, so `OnEnable` sees the final parent.
- **Release** deactivates the instance and puts it back under `root`.
- **maxSize** caps the idle instances, not the total: `Get` always succeeds and creates instances when none is idle, but an instance released while `maxSize` instances are already idle is destroyed.
- **Clear** destroys the idle instances. Instances that are handed out stay with their owner and may still be released later; the pool remains usable.
- **Destroying** uses `Object.Destroy` in play mode and `Object.DestroyImmediate` outside it, so edit-mode code and tests can use the pool.

## Rules & invariants
- `prefab` and `root` must not be null; `maxSize > 0`; `0 <= prewarmCount <= maxSize`. Violations throw in the constructor.
- Releasing the same instance twice throws `InvalidOperationException` in the editor (`ObjectPool`'s collection check); in a player build the check is off for speed, and a double release corrupts the pool.
- `Release(null)` throws `ArgumentNullException`.
- Only release instances that came from the same pool.
- The pool does not track instances that are handed out. When their parent is destroyed (for example with a scene), they are destroyed with it and simply never come back.
- `Get` and `Release` allocate nothing once the pool is warm; the constructor allocates the prewarm buffer.

## Tests
`Packages/com.base-setup.core/Tests/Pooling/` (`PrefabPoolTests`), EditMode with plain GameObjects as the prefab: prewarm creates inactive instances under the root; `Get` activates and reparents and restores the prefab's local pose; `Release` deactivates and returns under the root; `Get` after `Release` reuses the same instance; a double release throws; `Clear` destroys idle instances and leaves handed-out ones; beyond `maxSize`, `Get` still creates and the extra released instance is destroyed; a prewarm above `maxSize` is rejected.

## Related
- Used by [Audio](Audio.md) for its sound effect sources, and by the game's board pieces ([Play](../../Game/Systems/Play.md)). A game's spawners use it for objects spawned often during play.
