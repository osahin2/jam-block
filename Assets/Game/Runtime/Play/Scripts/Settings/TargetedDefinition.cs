using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Targeted Booster")]
    public sealed class TargetedDefinition : BoosterDefinition
    {
        [SerializeField] private BoosterShot _shot;

        public BoosterShot Shot => _shot;
    }
}
