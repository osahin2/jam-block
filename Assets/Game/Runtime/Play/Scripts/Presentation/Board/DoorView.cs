using DG.Tweening;
using Game.Data.Levels;
using UnityEngine;

namespace Game.Play
{
    public sealed class DoorView : MonoBehaviour
    {
        [SerializeField] private BoardPiece _piece;
        [SerializeField] private Transform _model;
        [Tooltip("Cubes thrown out of the board while a block passes; it scales with the door (shape scaling), its " +
                 "position on the door's outer face, where blocks are cut.")]
        [SerializeField] private ParticleSystem _burst;
        [SerializeField] private ParticleSystemRenderer _burstRenderer;

        private BlockMotionSettings _motion;
        private Transform _arrow;
        private bool _hasRest;
        private Vector3 _modelRest;
        private Vector3 _burstRest;
        private float _arrowRest;
        private float _drop;
        private Sequence _passing;

        public Side Side { get; private set; }

        public void Init(Side side, Material material, Transform arrow, float drop, float burstHeight,
            BlockMotionSettings motion)
        {
            Side = side;
            RememberRest();
            StopPassing();
            _model.localPosition = _modelRest;
            _piece.SetMaterial(material);
            ResetBurst(material, burstHeight);
            _arrow = arrow;
            _arrowRest = arrow.localPosition.y;
            _drop = drop;
            _motion = motion;
        }

        public void LetThrough(float reach, float gone)
        {
            StopPassing();
            var rise = gone + _motion.DoorRiseDelay;
            var modelDrop = _drop / transform.localScale.y;
            _passing = DOTween.Sequence()
                .Insert(0f, _model.DOLocalMoveY(_modelRest.y - modelDrop, _motion.DoorDropDuration))
                .Insert(0f, _arrow.DOLocalMoveY(_arrowRest - _drop, _motion.DoorDropDuration))
                .InsertCallback(reach, () => _burst.Play())
                .InsertCallback(gone, () => _burst.Stop(true, ParticleSystemStopBehavior.StopEmitting))
                .Insert(rise, _model.DOLocalMoveY(_modelRest.y, _motion.DoorRiseDuration))
                .Insert(rise, _arrow.DOLocalMoveY(_arrowRest, _motion.DoorRiseDuration))
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }

        private void RememberRest()
        {
            if (_hasRest)
                return;
            _modelRest = _model.localPosition;
            _burstRest = _burst.transform.localPosition;
            _hasRest = true;
        }

        private void ResetBurst(Material material, float burstHeight)
        {
            _burstRenderer.sharedMaterial = material;
            _burst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var height = (burstHeight - transform.localPosition.y) / transform.localScale.y;
            _burst.transform.localPosition = new Vector3(_burstRest.x, height, _burstRest.z);
        }

        private void StopPassing()
        {
            _passing?.Kill();
            _passing = null;
        }
    }
}
