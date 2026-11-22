using System;
using DG.Tweening;
using Game.Data.Levels;
using UnityEngine;

namespace Game.Play
{
    public abstract class BoosterShot : MonoBehaviour
    {
        protected BoardProjection Projection { get; private set; }
        protected BoardPresenter Board { get; private set; }
        protected BoosterMotionSettings Motion { get; private set; }

        public void Init(BoardProjection projection, BoardPresenter board, BoosterMotionSettings motion)
        {
            Projection = projection;
            Board = board;
            Motion = motion;
        }

        public void Play(Vector2 from, Cell target, Action onLanded)
        {
            DOTween.Kill(this);
            gameObject.SetActive(true);
            Build(from, target, onLanded)
                .OnComplete(Stop)
                .SetTarget(this)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }

        public void Stop() => gameObject.SetActive(false);

        protected abstract Sequence Build(Vector2 from, Cell target, Action onLanded);

        protected Vector2 ToLocal(Vector2 screen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, screen, null, out var local);
            return local;
        }
    }
}
