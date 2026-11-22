using System;
using DG.Tweening;
using Game.Data.Levels;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Play
{
    public sealed class HammerShot : BoosterShot
    {
        [SerializeField] private RectTransform _body;
        [SerializeField] private Image _image;
        [SerializeField] private Vector2 _head = new Vector2(0.55f, 0.6f);

        protected override Sequence Build(Vector2 from, Cell target, Action onLanded)
        {
            var headOffset = Vector2.Scale(_body.rect.size, _head - _body.pivot);
            var strike = ToLocal(Projection.ScreenPointOf(target, Motion.HammerLift)) - headOffset;
            _body.anchoredPosition = ToLocal(from) - headOffset;
            _body.localRotation = Quaternion.identity;
            _image.color = Color.white;
            return DOTween.Sequence()
                .Append(_body.DOAnchorPos(strike, Motion.HammerFlight).SetEase(Motion.HammerFlightEase))
                .Join(_body.DOLocalRotate(new Vector3(0f, 0f, Motion.HammerWindUp), Motion.HammerFlight))
                .Append(_body.DOLocalRotate(Vector3.zero, Motion.HammerStrike).SetEase(Motion.HammerStrikeEase))
                .AppendCallback(() => Strike(target, onLanded))
                .Append(_image.DOFade(0f, Motion.HammerFade));
        }

        private void Strike(Cell target, Action onLanded)
        {
            Board.VanishGone(target, 0f, Motion.BreakDuration, 0f, Motion.BreakEase);
            onLanded();
        }
    }
}
