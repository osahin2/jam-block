# VContainer for Zenject users

> A quick map from Zenject concepts to their VContainer equivalents, limited to what this project uses. For anything else, see the official docs at https://vcontainer.hadashikick.jp.

## Concept map

| Zenject | VContainer | Notes |
|---|---|---|
| `ProjectContext` | Root `LifetimeScope` | App-wide services; survives scene loads |
| `SceneContext` | Child `LifetimeScope` in the scene | Its parent is the root scope |
| `MonoInstaller.InstallBindings()` | `LifetimeScope.Configure(IContainerBuilder builder)` | One override per scope |
| `Installer<T>` (reusable installer) | An `IContainerBuilder` extension method | This is what each Core module exposes, e.g. `RegisterCoreAudio(...)` |
| `Container.Bind<IFoo>().To<Foo>().AsSingle()` | `builder.Register<Foo>(Lifetime.Singleton).As<IFoo>()` | |
| `AsTransient()` | `Lifetime.Transient` | New instance per resolve |
| `BindInterfacesTo<Foo>()` | `builder.Register<Foo>(...).AsImplementedInterfaces()` | |
| `BindInstance(x)` | `builder.RegisterInstance(x)` | |
| `FromComponentInHierarchy()` | `builder.RegisterComponentInHierarchy<T>()` | |
| `FromComponentInNewPrefab(prefab)` | `builder.RegisterComponentInNewPrefab(prefab, lifetime)` | |
| `IInitializable`, `ITickable`, `IDisposable` on a bound class | Same interface names exist, plus `IStartable` and `IAsyncStartable`; register with `builder.RegisterEntryPoint<T>()` | Entry points are created eagerly |
| `[Inject]` on a method or field | Same attribute | Used here only on MonoBehaviours |
| Constructor injection | Same, no attribute needed | One constructor per class |
| `PlaceholderFactory<TParam, T>` | `builder.RegisterFactory<TParam, T>(...)`, injected as `Func<TParam, T>` | |
| `DiContainer.InstantiatePrefab(prefab)` | `IObjectResolver.Instantiate(prefab)` | Injects the new object; used only inside factories |
| `SignalBus` | Nothing built in | Not used: systems talk through injected contracts, there is no event bus |
| `MemoryPool` | Nothing built in | Use `Core.Pooling` |

## Differences that matter

- **Scene objects are not injected automatically.** Zenject's `SceneContext` injects every MonoBehaviour in the scene. In VContainer a MonoBehaviour receives injection only if it is registered (`RegisterComponent...`), created through the resolver, or listed in the scope's auto-inject list. This is the most common surprise.
- **`Singleton` vs `Scoped`.** `Lifetime.Singleton` is one instance for the scope it is registered in and all its children. `Lifetime.Scoped` is one instance per scope. App-wide services are singletons in the root scope; per-scene objects are registered in the scene scope.
- **Nothing runs unless something asks for it.** A registered class is not created until it is resolved. Classes that must run on their own (ticking, startup logic) are registered as entry points.
- **Fewer features, on purpose.** There are no conditional bindings (`WhenInjectedInto`), no decorators and no sub-container installers. This project needs none of them.
- **Stripping.** No `[Inject]` on constructors here. If the IL2CPP managed stripping level is raised above the default, add the source generator or a `link.xml` first.

## How this project uses it

- One root scope in the boot scene, one child scope in the gameplay scene, both in the game's `Bootstrap/` folder. The boot scene stays loaded and the gameplay scene is loaded additively by `Core.Scenes`; the child scope names the root scope's type in its `parentReference` field (Zenject's parent contract names), and VContainer finds it among the loaded scenes.
- Each Core module is added with its single public installer extension.
- Service classes take dependencies through their constructor and do not reference VContainer.
- `IObjectResolver` is never stored in a service.
