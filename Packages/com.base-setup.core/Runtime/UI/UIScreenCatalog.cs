using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI
{
    [CreateAssetMenu(menuName = "Core/UI Screen Catalog", fileName = "Screens")]
    public sealed class UIScreenCatalog : ScriptableObject
    {
        [SerializeField] private UIScreen[] _screens = Array.Empty<UIScreen>();

        public IReadOnlyList<UIScreen> Screens => _screens;

        internal static UIScreenCatalog Create(string name, params UIScreen[] screens)
        {
            var catalog = CreateInstance<UIScreenCatalog>();
            catalog.name = name;
            catalog._screens = screens;
            return catalog;
        }
    }
}
