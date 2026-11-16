using System.Collections.Generic;
using UnityEngine;

namespace Game.Levels.Settings
{
    // The levels of the game in play order.
    [CreateAssetMenu(menuName = "Game/Levels/Level Catalog")]
    public sealed class LevelCatalog : ScriptableObject
    {
        [SerializeField] private LevelAsset[] _levels;

        public IReadOnlyList<LevelAsset> Levels => _levels;
    }
}
