using System;
using DG.Tweening;
using Game.Data.Levels;
using UnityEngine;

namespace Game.Play
{
    public sealed class VacuumShot : BoosterShot
    {
        [SerializeField] private RectTransform _body;

        protected override Sequence Build(Vector2 from, Cell target, Action onLanded)
        {
            _body.anchoredPosition = ToLocal(Projection.ScreenPointOf(target, Motion.VacuumLift));
            _body.localScale = Vector3.zero;
            return DOTween.Sequence()
                .Append(_body.DOScale(Motion.VacuumScale, Motion.VacuumGrow).SetEase(Motion.VacuumGrowEase))
                .AppendCallback(() => Pull(target, onLanded))
                .AppendInterval(Motion.VacuumHold)
                .Append(_body.DOScale(0f, Motion.VacuumShrink).SetEase(Motion.VacuumShrinkEase));
        }

        private void Pull(Cell target, Action onLanded)
        {
            Board.VanishGone(target, Motion.VacuumLift, Motion.VacuumPull, Motion.VacuumStagger, Motion.VacuumPullEase);
            onLanded();
        }
    }
}
