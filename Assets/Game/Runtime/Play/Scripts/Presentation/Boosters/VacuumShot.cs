using System;
using DG.Tweening;
using Game.Data.Levels;
using UnityEngine;

namespace Game.Play
{
    public sealed class VacuumShot : BoosterShot
    {
        [SerializeField] private RectTransform _body;

        protected override Sequence Build(Vector2 from, Cell target, BoardInteraction board, BoosterMotionSettings motion,
            Action onLanded)
        {
            _body.anchoredPosition = ToLocal(board.ScreenPointOf(target, motion.VacuumLift));
            _body.localScale = Vector3.zero;
            return DOTween.Sequence()
                .Append(_body.DOScale(motion.VacuumScale, motion.VacuumGrow).SetEase(motion.VacuumGrowEase))
                .AppendCallback(() => Pull(target, board, motion, onLanded))
                .AppendInterval(motion.VacuumHold)
                .Append(_body.DOScale(0f, motion.VacuumShrink).SetEase(motion.VacuumShrinkEase));
        }

        private static void Pull(Cell target, BoardInteraction board, BoosterMotionSettings motion, Action onLanded)
        {
            board.VanishGone(target, motion.VacuumLift, motion.VacuumPull, motion.VacuumStagger, motion.VacuumPullEase);
            onLanded();
        }
    }
}
