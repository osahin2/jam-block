using DG.Tweening;
using Game.Play.Settings;
using UnityEngine;

namespace Game.Play.Presentation
{
    // Follows its target smoothly while dragged (per frame, hand-written). One-shot motions are tweens: the snap onto
    // a cell on release, and the snap in front of the door, the hold and the slide out when the block leaves.
    public sealed class BlockView : MonoBehaviour
    {
        // An exponential approach never reaches its target; closer than this the block is put on it.
        private const float SnapDistanceSqr = 0.000001f;

        private BlockMotionSettings _settings;
        private Vector3 _target;
        private Tween _motion;

        public void Init(BlockMotionSettings settings) => _settings = settings;

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

        public void SnapTo(Vector3 position)
        {
            StopMotion();
            _target = position;
            _motion = Snap(position).SetLink(gameObject);
        }

        public void Leave(Vector3 doorCell, Vector3 outside)
        {
            StopMotion();
            _motion = DOTween.Sequence()
                .Append(Snap(doorCell))
                .AppendInterval(_settings.LeaveHoldDuration)
                .Append(transform.DOLocalMove(outside, _settings.LeaveDuration).SetEase(_settings.LeaveEase))
                .OnComplete(Hide)
                .SetLink(gameObject);
        }

        private Tween Snap(Vector3 position) =>
            transform.DOLocalMove(position, _settings.SnapDuration).SetEase(_settings.SnapEase);

        private void StopMotion()
        {
            enabled = false;
            _motion?.Kill();
            _motion = null;
        }

        private void Hide() => gameObject.SetActive(false);
    }
}
