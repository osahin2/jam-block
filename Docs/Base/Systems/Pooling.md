# Pooling

> Reuses instances of one prefab: idle ones wait inactive under a root, `Get` hands one out under a parent, `Release` takes it back. A thin layer over `ObjectPool<T>`.

**Code:** `Core.Pooling`. **Entry:** `PrefabPool<T>(prefab, root, prewarmCount, maxSize)`, `Get(parent)`, `Release`, `Clear`.

## Rules
- A utility, not a service: each owner builds and keeps its own pool.
- `Get` resets the local pose to the prefab's before activating, so it matches `Instantiate` and a reused instance carries no pose from its last use.
- `maxSize` caps idle instances, not the total; an instance released past it is destroyed.
- A double release throws in the editor only; in a player it corrupts the pool. Release only to the pool an instance came from.
- `Get` and `Release` allocate nothing once warm.
