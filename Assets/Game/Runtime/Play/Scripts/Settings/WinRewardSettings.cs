using UnityEngine;

namespace Game.Play
{
    // The coins a won level pays, the same for every level, as in the reference's first levels.
    [CreateAssetMenu(menuName = "Game/Play/Win Reward Settings")]
    public sealed class WinRewardSettings : ScriptableObject
    {
        [Tooltip("Coins paid when the player continues from the level complete popup.")]
        [SerializeField, Min(0)] private int _coins = 40;

        public int Coins => _coins;
    }
}
