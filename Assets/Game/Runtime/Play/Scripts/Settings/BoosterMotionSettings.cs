using DG.Tweening;
using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Booster Motion Settings")]
    public sealed class BoosterMotionSettings : ScriptableObject
    {
        [Header("Hammer")]
        [SerializeField, Min(0f)] private float _hammerLift = 0.3f;
        [SerializeField, Min(0f)] private float _hammerFlight = 0.35f;
        [SerializeField] private Ease _hammerFlightEase = Ease.OutQuad;
        [SerializeField] private float _hammerWindUp = 40f;
        [SerializeField, Min(0f)] private float _hammerStrike = 0.12f;
        [SerializeField] private Ease _hammerStrikeEase = Ease.InQuad;
        [SerializeField, Min(0f)] private float _hammerFade = 0.25f;
        [SerializeField, Min(0f)] private float _breakDuration = 0.18f;
        [SerializeField] private Ease _breakEase = Ease.InBack;

        [Header("Vacuum")]
        [SerializeField, Min(0f)] private float _vacuumLift = 2.5f;
        [SerializeField, Min(0f)] private float _vacuumScale = 1.25f;
        [SerializeField, Min(0f)] private float _vacuumGrow = 0.3f;
        [SerializeField] private Ease _vacuumGrowEase = Ease.OutBack;
        [SerializeField, Min(0f)] private float _vacuumPull = 0.5f;
        [SerializeField] private Ease _vacuumPullEase = Ease.InQuad;
        [SerializeField, Min(0f)] private float _vacuumStagger = 0.08f;
        [SerializeField, Min(0f)] private float _vacuumHold = 0.7f;
        [SerializeField, Min(0f)] private float _vacuumShrink = 0.2f;
        [SerializeField] private Ease _vacuumShrinkEase = Ease.InBack;

        public float HammerLift => _hammerLift;
        public float HammerFlight => _hammerFlight;
        public Ease HammerFlightEase => _hammerFlightEase;
        public float HammerWindUp => _hammerWindUp;
        public float HammerStrike => _hammerStrike;
        public Ease HammerStrikeEase => _hammerStrikeEase;
        public float HammerFade => _hammerFade;
        public float BreakDuration => _breakDuration;
        public Ease BreakEase => _breakEase;
        public float VacuumLift => _vacuumLift;
        public float VacuumScale => _vacuumScale;
        public float VacuumGrow => _vacuumGrow;
        public Ease VacuumGrowEase => _vacuumGrowEase;
        public float VacuumPull => _vacuumPull;
        public Ease VacuumPullEase => _vacuumPullEase;
        public float VacuumStagger => _vacuumStagger;
        public float VacuumHold => _vacuumHold;
        public float VacuumShrink => _vacuumShrink;
        public Ease VacuumShrinkEase => _vacuumShrinkEase;
    }
}
