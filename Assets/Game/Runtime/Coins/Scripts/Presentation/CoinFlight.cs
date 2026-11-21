using System;
using Core.Pooling;
using DG.Tweening;
using UnityEngine;
using VContainer;

namespace Game.Coins
{
    public sealed class CoinFlight : MonoBehaviour
    {
        private const int PooledFlights = 4;

        [SerializeField] private RectTransform _coinTemplate;

        private CoinFlightSettings _settings;
        private PrefabPool<RectTransform> _coins;

        [Inject]
        public void Construct(CoinFlightSettings settings) => _settings = settings;

        private void Awake() =>
            _coins = new PrefabPool<RectTransform>(_coinTemplate, transform, _settings.Coins, _settings.Coins * PooledFlights);

        public void Play(Vector3 from, CoinCounter to, int amount, Action onLanded)
        {
            var flight = new Flight(_coins, to, amount, _settings.Coins, onLanded);
            var radius = _settings.BurstRadius * transform.lossyScale.x;
            var burst = _settings.BurstDuration;
            var duration = _settings.FlightDuration;
            var landing = to.Landing;
            var sequence = DOTween.Sequence().SetUpdate(true).SetLink(to.gameObject);
            for (var i = 0; i < flight.Coins.Length; i++)
            {
                var coin = _coins.Get(transform);
                flight.Coins[i] = coin;
                coin.position = from;
                coin.localScale = Vector3.zero;
                var scatter = from + (Vector3)(UnityEngine.Random.insideUnitCircle * radius);
                sequence.Insert(0f, coin.DOScale(1f, burst).SetEase(_settings.BurstEase));
                sequence.Insert(0f, coin.DOMove(scatter, burst).SetEase(_settings.BurstEase));
                var leave = burst + i * _settings.FlightStagger;
                sequence.Insert(leave, coin.DOMoveX(landing.x, duration).SetEase(_settings.AcrossEase));
                sequence.Insert(leave, coin.DOMoveY(landing.y, duration).SetEase(_settings.UpEase));
                sequence.InsertCallback(leave + duration, flight.LandNext);
            }

            sequence.OnComplete(flight.Finish).OnKill(flight.ReleaseRest);
        }

        private sealed class Flight
        {
            private readonly PrefabPool<RectTransform> _pool;
            private readonly CoinCounter _counter;
            private readonly int _amount;
            private Action _onLanded;
            private int _landed;

            public RectTransform[] Coins { get; }

            public Flight(PrefabPool<RectTransform> pool, CoinCounter counter, int amount, int coins, Action onLanded)
            {
                _pool = pool;
                _counter = counter;
                _amount = amount;
                _onLanded = onLanded;
                Coins = new RectTransform[coins];
            }

            public void LandNext()
            {
                _pool.Release(Coins[_landed]);
                _landed++;
                _counter.Land(CoinTally.Share(_landed, Coins.Length, _amount));
            }

            public void Finish()
            {
                var onLanded = _onLanded;
                _onLanded = null;
                onLanded?.Invoke();
            }

            public void ReleaseRest()
            {
                for (var i = _landed; i < Coins.Length; i++)
                    _pool.Release(Coins[i]);
                _landed = Coins.Length;
            }
        }
    }
}
