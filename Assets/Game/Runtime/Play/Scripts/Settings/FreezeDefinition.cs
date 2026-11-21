using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Freeze Booster")]
    public sealed class FreezeDefinition : BoosterDefinition
    {
        [SerializeField, Min(0f)] private float _seconds = 10f;

        public float Seconds => _seconds;
    }
}
