# Installers

> Summary: `ScriptableObjectInstaller`, a VContainer `IInstaller` that is an asset. Each game feature has one: it references the feature's settings assets and registers its services, and a scene's scope runs a serialized list of them.

## Code
- Folder: `Packages/com.base-setup.core/Runtime/Installers/`
- Assembly: `Core.Installers` (references VContainer)
- Entry points:
  - `ScriptableObjectInstaller` ([ScriptableObjectInstaller.cs](../../../Packages/com.base-setup.core/Runtime/Installers/ScriptableObjectInstaller.cs)): abstract `ScriptableObject` with `Install(IContainerBuilder builder)`.

## How it works
- A feature derives one sealed installer from it, adds a `[CreateAssetMenu]`, keeps `[SerializeField]` references to its settings assets, prefabs and UI catalogs, and in `Install` registers them (`RegisterInstance`) with its services.
- The scope of a scene holds `[SerializeField] ScriptableObjectInstaller[] _installers` and calls `installer.Install(builder)` for each in `Configure`, after registering its scene references. Adding a feature to a scene is adding its asset to that list; the scope's code does not change.
- Why an asset and not a `MonoInstaller`-style component: settings references live on one asset per feature that a test can load and check without opening a scene, and the same installer works in any scene.

## Rules & invariants
- One installer per feature. It registers only that feature's settings and services; Core modules are registered by the scope (`RegisterCore<Module>`), not by a feature installer.
- No state: `Install` may run more than once (every time a scope is built) and must not write to the asset.

## Tests
None in Core: the class has no behavior of its own. A game should add an EditMode test that loads every installer asset (`AssetDatabase.FindAssets("t:ScriptableObjectInstaller")`) and fails when the installer or one of its settings assets has an unassigned object reference, so a missing prefab fails in the test run instead of at scene start.

## Related
- [UI](UI.md): a feature installer usually calls `RegisterUIScreens(catalog)`.
