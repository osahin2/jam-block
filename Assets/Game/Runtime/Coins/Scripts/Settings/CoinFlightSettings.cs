using DG.Tweening;
using UnityEngine;

namespace Game.Coins
{
    [CreateAssetMenu(menuName = "Game/Coins/Coin Flight Settings")]
    public sealed class CoinFlightSettings : ScriptableObject
    {
        [Tooltip("Coins that fly to a coin counter; the amount is split between them.")]
        [SerializeField, Min(1)] private int _coins = 10;
        [Header("Burst")]
        [Tooltip("How far the coins scatter around where they start, in reference pixels (1080 x 1920 wide).")]
        [SerializeField, Min(0f)] private float _burstRadius = 160f;
        [SerializeField, Min(0f)] private float _burstDuration = 0.3f;
        [SerializeField] private Ease _burstEase = Ease.OutQuad;
        [Header("Flight")]
        [Tooltip("Seconds for one coin to fly to the counter.")]
        [SerializeField, Min(0f)] private float _flightDuration = 0.55f;
        [Tooltip("Seconds between two coins leaving for the counter.")]
        [SerializeField, Min(0f)] private float _flightStagger = 0.06f;
        [Tooltip("Different eases across and up the screen bend the path into an arc.")]
        [SerializeField] private Ease _acrossEase = Ease.InSine;
        [SerializeField] private Ease _upEase = Ease.OutQuad;
        [Header("Counter")]
        [Tooltip("How much the counter's coin icon grows for a moment as a coin lands, as a fraction of its size.")]
        [SerializeField, Min(0f)] private float _landPunch = 0.25f;
        [SerializeField, Min(0f)] private float _landPunchDuration = 0.2f;

        public int Coins => _coins;
        public float BurstRadius => _burstRadius;
        public float BurstDuration => _burstDuration;
        public Ease BurstEase => _burstEase;
        public float FlightDuration => _flightDuration;
        public float FlightStagger => _flightStagger;
        public Ease AcrossEase => _acrossEase;
        public Ease UpEase => _upEase;
        public float LandPunch => _landPunch;
        public float LandPunchDuration => _landPunchDuration;
    }
}
