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

        protected override Sequence Build(Vector2 from, Cell target, BoardInteraction board, BoosterMotionSettings motion,
            Action onLanded)
        {
            var hit = ToLocal(board.ScreenPointOf(target, motion.TargetLift));
            var start = new Vector2(ToLocal(from).x, ((RectTransform)transform).rect.yMin - _body.rect.height);
            var heading = hit - start;
            _body.anchoredPosition = start;
            _body.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg - _noseAngle);
            _body.gameObject.SetActive(true);
            _burstPoint.anchoredPosition = hit;
            _burst.Clear();
            var color = board.TryFindColorShownAt(target, out var shown) ? shown : Color.white;
            return DOTween.Sequence()
                .Append(_body.DOAnchorPos(hit, motion.RocketFlight).SetEase(motion.RocketFlightEase))
                .AppendCallback(() => Explode(color, onLanded))
                .AppendInterval(motion.RocketBurstHold);
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
