using System.Collections.Generic;
using DG.Tweening;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    public sealed class BlockView : MonoBehaviour
    {
        private const float SnapDistanceSqr = 0.000001f;

        [SerializeField] private Outline _outline;

        private readonly List<Renderer> _pieces = new List<Renderer>();
        private BlockMotionSettings _settings;
        private Vector3 _target;
        private Tween _motion;

        public bool IsLeaving { get; private set; }

        public void Init(BlockMotionSettings settings)
        {
            _settings = settings;
            _pieces.Clear();
            transform.localScale = Vector3.one;
        }

        private void Awake() => enabled = false;

        private void Update()
        {
            var fraction = ExponentialFollow.Fraction(_settings.DragHalfLife, Time.deltaTime);
            var position = Vector3.Lerp(transform.localPosition, _target, fraction);
            if ((_target - position).sqrMagnitude < SnapDistanceSqr)
            {
                position = _target;
                enabled = false;
            }
            transform.localPosition = position;
        }

        public void SetPose(Vector3 position)
        {
            StopMotion();
            _target = position;
            transform.localPosition = position;
        }

        public void Follow(Vector3 target)
        {
            if (enabled && target == _target)
                return;
            StopMotion();
            _target = target;
            enabled = true;
        }

        public void AddPiece(BoardPiece piece) => _pieces.Add(piece.Renderer);

        public void UseOutline(BlockOutlineMaterials materials) =>
            _outline.SetRenderers(_pieces, materials.Mask, materials.Fill);

        public void Hold() => _outline.Show();

        public void SnapTo(Vector3 position)
        {
            _outline.Hide();
            StopMotion();
            _target = position;
            _motion = Snap(position).SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }

        public void Leave(Vector3 doorCell, Vector3 outside, float slideDuration)
        {
            _outline.Hide();
            StopMotion();
            _target = outside;
            _motion = DOTween.Sequence()
                .Append(Snap(doorCell))
                .Append(transform.DOLocalMove(outside, slideDuration).SetEase(Ease.InQuad))
                .OnComplete(Hide)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            IsLeaving = true;
        }

        public bool TryVanish(Vector3 target, float duration, float delay, Ease ease)
        {
            if (IsLeaving || !gameObject.activeSelf)
                return false;
            _outline.Hide();
            StopMotion();
            _target = target;
            _motion = DOTween.Sequence()
                .AppendInterval(delay)
                .Append(transform.DOLocalMove(target, duration).SetEase(ease))
                .Join(transform.DOScale(0f, duration).SetEase(ease))
                .OnComplete(Hide)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            IsLeaving = true;
            return true;
        }

        public void CatchUp(BoardBlock block)
        {
            if (block.IsGone && !IsLeaving)
                Hide();
        }

        private Tween Snap(Vector3 position) =>
            transform.DOLocalMove(position, _settings.SnapDuration).SetEase(_settings.SnapEase);

        private void StopMotion()
        {
            enabled = false;
            IsLeaving = false;
            _motion?.Kill();
            _motion = null;
        }

        private void Hide()
        {
            IsLeaving = false;
            gameObject.SetActive(false);
        }
    }
}
