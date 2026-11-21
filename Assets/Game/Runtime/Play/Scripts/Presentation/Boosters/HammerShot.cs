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

        protected override Sequence Build(Vector2 from, Cell target, BoardInteraction board, BoosterMotionSettings motion,
            Action onLanded)
        {
            var headOffset = Vector2.Scale(_body.rect.size, _head - _body.pivot);
            var strike = ToLocal(board.ScreenPointOf(target, motion.HammerLift)) - headOffset;
            _body.anchoredPosition = ToLocal(from) - headOffset;
            _body.localRotation = Quaternion.identity;
            _image.color = Color.white;
            return DOTween.Sequence()
                .Append(_body.DOAnchorPos(strike, motion.HammerFlight).SetEase(motion.HammerFlightEase))
                .Join(_body.DOLocalRotate(new Vector3(0f, 0f, motion.HammerWindUp), motion.HammerFlight))
                .Append(_body.DOLocalRotate(Vector3.zero, motion.HammerStrike).SetEase(motion.HammerStrikeEase))
                .AppendCallback(() => Strike(target, board, motion, onLanded))
                .Append(_image.DOFade(0f, motion.HammerFade));
        }

        private static void Strike(Cell target, BoardInteraction board, BoosterMotionSettings motion, Action onLanded)
        {
            board.VanishGone(target, 0f, motion.BreakDuration, 0f, motion.BreakEase);
            onLanded();
        }
    }
}
