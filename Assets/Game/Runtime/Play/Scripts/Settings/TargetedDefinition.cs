using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Targeted Booster")]
    public sealed class TargetedDefinition : BoosterDefinition
    {
        [SerializeField] private BoosterShot _shot;
        [SerializeField] private bool _marksTargets;

        public BoosterShot Shot => _shot;
        public bool MarksTargets => _marksTargets;
    }
}
