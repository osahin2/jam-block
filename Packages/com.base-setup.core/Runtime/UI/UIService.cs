using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI
{
    internal sealed class UIService : IUIService
    {
        private readonly Dictionary<Type, UIScreen> _prefabs = new Dictionary<Type, UIScreen>();
        private readonly Dictionary<Type, string> _catalogOf = new Dictionary<Type, string>();
        private readonly Dictionary<Type, UIScreen> _instances = new Dictionary<Type, UIScreen>();
        private readonly UIRoot _root;
        private readonly Func<UIScreen, Transform, UIScreen> _create;

        // create instantiates a prefab under a parent; the installer passes one that also injects the screen.
        public UIService(IEnumerable<UIScreenCatalog> catalogs, UIRoot root, Func<UIScreen, Transform, UIScreen> create)
        {
            _root = root;
            _create = create;

            foreach (UIScreenCatalog catalog in catalogs)
            {
                for (int i = 0; i < catalog.Screens.Count; i++)
                {
                    UIScreen prefab = catalog.Screens[i];
                    if (prefab == null)
                        throw new InvalidOperationException($"Screen catalog '{catalog.name}' has an empty entry at {i}.");

                    Type type = prefab.GetType();
                    if (_catalogOf.TryGetValue(type, out string other))
                        throw new InvalidOperationException($"{type.Name} is listed in two screen catalogs: '{other}' and '{catalog.name}'.");

                    _prefabs.Add(type, prefab);
                    _catalogOf.Add(type, catalog.name);
                }
            }
        }

        public T Open<T>() where T : UIScreen
        {
            if (!_instances.TryGetValue(typeof(T), out UIScreen screen))
                screen = Create(typeof(T));

            screen.transform.SetAsLastSibling();
            screen.gameObject.SetActive(true);
            return (T)screen;
        }

        public void Close(UIScreen screen) => screen.gameObject.SetActive(false);

        private UIScreen Create(Type type)
        {
            if (!_prefabs.TryGetValue(type, out UIScreen prefab))
                throw new InvalidOperationException($"No registered screen catalog lists {type.Name}. Add it to its feature's catalog and register that catalog with RegisterUIScreens.");

            if (prefab.Content == null)
                throw new InvalidOperationException($"{type.Name} has no Content assigned. Make the prefab a variant of the Core screen template.");

            UIScreen screen = _create(prefab, _root.LayerParent(prefab.Layer));
            screen.Bind(this);
            SafeAreaFit.Apply(screen.Content, Screen.safeArea, new Vector2(Screen.width, Screen.height));
            _instances.Add(type, screen);
            return screen;
        }
    }
}
