using UnityEngine;

namespace Game.Play.Presentation
{
    public sealed class BlockView : MonoBehaviour
    {
        private Vector3 _leaveVelocity;
        private float _leaveTimeLeft;

        private void Awake() => enabled = false;

        private void Update()
        {
            transform.localPosition += _leaveVelocity * Time.deltaTime;
            _leaveTimeLeft -= Time.deltaTime;
            if (_leaveTimeLeft <= 0f)
                gameObject.SetActive(false);
        }

        public void SetPose(Vector3 position) => transform.localPosition = position;

        public void Leave(Vector3 velocity, float duration)
        {
            _leaveVelocity = velocity;
            _leaveTimeLeft = duration;
            enabled = true;
        }
    }
}
