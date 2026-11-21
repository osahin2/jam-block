using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Levels
{
    [CreateAssetMenu(menuName = "Game/Levels/Level Catalog")]
    public sealed class LevelCatalog : ScriptableObject
    {
        [SerializeField] private LevelAsset[] _levels = Array.Empty<LevelAsset>();

        public IReadOnlyList<LevelAsset> Levels => _levels;

        public int IndexOf(LevelAsset level) => Array.IndexOf(_levels, level);
    }
}
