using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Play
{
    public sealed class BlockView : MonoBehaviour
    {
        // An exponential approach never reaches its target; closer than this the block is put on it.
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

        // A quadratic ease in from rest is a constant acceleration. It starts where the block is drawn, which may trail
        // its door cell during a drag.
        public void Leave(Vector3 outside, float slideDuration)
        {
            _outline.Hide();
            StopMotion();
            _target = outside;
            _motion = transform.DOLocalMove(outside, slideDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(Hide)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            IsLeaving = true;
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
