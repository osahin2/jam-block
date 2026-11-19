using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Levels
{
    // The levels of the game in play order.
    [CreateAssetMenu(menuName = "Game/Levels/Level Catalog")]
    public sealed class LevelCatalog : ScriptableObject
    {
        // Created from code, a ScriptableObject keeps a field without an initializer null.
        [SerializeField] private LevelAsset[] _levels = Array.Empty<LevelAsset>();

        public IReadOnlyList<LevelAsset> Levels => _levels;

        // The level's place in the play order, or -1 when the catalog does not list it.
        public int IndexOf(LevelAsset level) => Array.IndexOf(_levels, level);
    }
}
