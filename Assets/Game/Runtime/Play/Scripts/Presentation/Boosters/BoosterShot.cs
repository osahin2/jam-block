using System;
using DG.Tweening;
using Game.Data.Levels;
using UnityEngine;

namespace Game.Play
{
    public abstract class BoosterShot : MonoBehaviour
    {
        public void Play(Vector2 from, Cell target, BoardInteraction board, BoosterMotionSettings motion, Action onLanded)
        {
            DOTween.Kill(this);
            gameObject.SetActive(true);
            Build(from, target, board, motion, onLanded)
                .OnComplete(Stop)
                .SetTarget(this)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }

        public void Stop() => gameObject.SetActive(false);

        protected abstract Sequence Build(Vector2 from, Cell target, BoardInteraction board, BoosterMotionSettings motion,
            Action onLanded);

        protected Vector2 ToLocal(Vector2 screen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, screen, null, out var local);
            return local;
        }
    }
}
