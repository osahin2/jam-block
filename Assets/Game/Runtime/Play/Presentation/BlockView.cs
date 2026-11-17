using UnityEngine;

namespace Game.Play.Presentation
{
    // Follows a target pose smoothly while dragged or settling, and slides out when the block leaves. Stops updating
    // once it has reached its target; a new target starts it again.
    public sealed class BlockView : MonoBehaviour
    {
        // An exponential approach never reaches its target; closer than this the block is put on it.
        private const float SnapDistanceSqr = 0.000001f;

        private Vector3 _target;
        private float _halfLife;
        private bool _isLeaving;
        private Vector3 _leaveVelocity;
        private float _leaveTimeLeft;

        private void Awake() => enabled = false;

        private void Update()
        {
            if (_isLeaving)
                SlideOut();
            else
                Approach();
        }

        public void SetPose(Vector3 position)
        {
            _target = position;
            transform.localPosition = position;
        }

        public void Follow(Vector3 target, float halfLife)
        {
            if (enabled && target == _target)
                return;
            _target = target;
            _halfLife = halfLife;
            enabled = true;
        }

        public void Leave(Vector3 velocity, float duration)
        {
            _isLeaving = true;
            _leaveVelocity = velocity;
            _leaveTimeLeft = duration;
            enabled = true;
        }

        private void Approach()
        {
            var position = Vector3.Lerp(transform.localPosition, _target, ExponentialFollow.Fraction(_halfLife, Time.deltaTime));
            if ((_target - position).sqrMagnitude < SnapDistanceSqr)
            {
                position = _target;
                enabled = false;
            }
            transform.localPosition = position;
        }

        private void SlideOut()
        {
            transform.localPosition += _leaveVelocity * Time.deltaTime;
            _leaveTimeLeft -= Time.deltaTime;
            if (_leaveTimeLeft <= 0f)
                gameObject.SetActive(false);
        }
    }
}
