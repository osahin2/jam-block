# Installers

> `ScriptableObjectInstaller`: a VContainer installer that is an asset. Each feature has one that registers its settings and services; a scene's scope runs a list of them.

**Code:** `Core.Installers`. **Entry:** `ScriptableObjectInstaller.Install(builder)`.

## Rules
- An asset, not a component: a test can load it and check its references without opening a scene, and it works in any scene.
- One per feature, registering only that feature; Core modules are registered by the scope.
- No state: `Install` may run on every scope build and never writes to the asset.
