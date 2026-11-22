using System;
using DG.Tweening;
using Game.Data.Levels;
using UnityEngine;

namespace Game.Play
{
    public sealed class RocketShot : BoosterShot
    {
        [SerializeField] private RectTransform _body;
        [SerializeField] private float _noseAngle = 45f;
        [SerializeField] private RectTransform _burstPoint;
        [SerializeField] private ParticleSystem _burst;

        protected override Sequence Build(Vector2 from, Cell target, Action onLanded)
        {
            var hit = ToLocal(Projection.ScreenPointOf(target, Motion.TargetLift));
            var start = new Vector2(ToLocal(from).x, ((RectTransform)transform).rect.yMin - _body.rect.height);
            var heading = hit - start;
            _body.anchoredPosition = start;
            _body.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg - _noseAngle);
            _body.gameObject.SetActive(true);
            _burstPoint.anchoredPosition = hit;
            _burst.Clear();
            var color = Board.TryFindColorShownAt(target, out var shown) ? shown : Color.white;
            return DOTween.Sequence()
                .Append(_body.DOAnchorPos(hit, Motion.RocketFlight).SetEase(Motion.RocketFlightEase))
                .AppendCallback(() => Explode(color, onLanded))
                .AppendInterval(Motion.RocketBurstHold);
        }

        private void Explode(Color color, Action onLanded)
        {
            _body.gameObject.SetActive(false);
            var main = _burst.main;
            main.startColor = color;
            _burst.Play();
            onLanded();
        }
    }
}
