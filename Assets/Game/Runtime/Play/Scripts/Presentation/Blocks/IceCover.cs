using System.Collections.Generic;
using DG.Tweening;
using Game.Data.Levels;
using TMPro;
using UnityEngine;

namespace Game.Play
{
    public sealed class IceCover : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ShadowColorId = Shader.PropertyToID("_ShadowColor");
        private static readonly int SurfaceMapId = Shader.PropertyToID("_SurfaceMap");
        private static readonly int SurfaceScaleId = Shader.PropertyToID("_SurfaceScale");
        private static readonly int SurfaceStrengthId = Shader.PropertyToID("_SurfaceStrength");

        [SerializeField] private TextMeshPro _count;
        [SerializeField] private ParticleSystem _shards;

        private IceSettings _settings;
        private IReadOnlyList<Renderer> _pieces;
        private MaterialPropertyBlock _properties;
        private IReadOnlyList<Cell> _shape;
        private float _cellSize;
        private float _top;
        private Color _ownColor;
        private Color _ownShadowColor;
        private Vector3 _countRestScale;
        private bool _hasRestScale;

        public int Shown { get; private set; }

        public void Show(int ice, BlockView block, IceSettings settings, BoardArtSettings art)
        {
            DOTween.Kill(this);
            Cover(block, settings, art);
            Shown = _shape.Count > 0 ? ice : 0;
            if (Shown == 0)
            {
                Clear();
                return;
            }
            ShowCount();
            TakeOwnColors(block.Material);
            Freeze(0f);
        }

        public void CatchUp(int iceLeft)
        {
            if (iceLeft >= Shown)
                return;
            Shown = iceLeft;
            if (iceLeft > 0)
                Crack();
            else
                Melt();
        }

        private void Cover(BlockView block, IceSettings settings, BoardArtSettings art)
        {
            _settings = settings;
            _pieces = block.Pieces;
            _shape = block.Shape;
            _cellSize = art.CellSize;
            _top = art.TouchHeight;
            _properties ??= new MaterialPropertyBlock();
            RememberCountRestScale();
        }

        private void RememberCountRestScale()
        {
            if (_hasRestScale)
                return;
            _countRestScale = _count.transform.localScale;
            _hasRestScale = true;
        }

        private void ShowCount()
        {
            var middle = ShapeMiddle.Of(_shape);
            var count = _count.transform;
            count.localPosition = Top(middle.X, middle.Y) + Vector3.up * _settings.CountLift;
            count.localScale = _countRestScale;
            _count.gameObject.SetActive(true);
            _count.SetText("{0}", Shown);
        }

        private void TakeOwnColors(Material blockMaterial)
        {
            _ownColor = blockMaterial.GetColor(BaseColorId);
            _ownShadowColor = blockMaterial.GetColor(ShadowColorId);
        }

        private void Crack()
        {
            DOTween.Kill(this);
            _count.SetText("{0}", Shown);
            _count.transform.localScale = _countRestScale;
            _count.transform
                .DOPunchScale(_countRestScale * _settings.CrackPunch, _settings.CrackDuration, _settings.CrackVibrato)
                .SetTarget(this)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            ThrowShards(_settings.CrackShards);
        }

        private void Melt()
        {
            DOTween.Kill(this);
            DOTween.Sequence()
                .Append(_count.transform.DOScale(Vector3.zero, _settings.CountShrinkDuration))
                .Join(DOVirtual.Float(0f, 1f, _settings.MeltDuration, Freeze).SetEase(_settings.MeltEase))
                .OnComplete(Clear)
                .SetTarget(this)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            ThrowShards(_settings.MeltShards);
        }

        private void Freeze(float melted)
        {
            _properties.SetColor(BaseColorId, Color.Lerp(_settings.Color, _ownColor, melted));
            _properties.SetColor(ShadowColorId, Color.Lerp(_settings.ShadowColor, _ownShadowColor, melted));
            _properties.SetTexture(SurfaceMapId, _settings.Surface);
            _properties.SetFloat(SurfaceScaleId, _settings.SurfaceScale);
            _properties.SetFloat(SurfaceStrengthId, 1f - melted);
            for (var i = 0; i < _pieces.Count; i++)
                _pieces[i].SetPropertyBlock(_properties);
        }

        private void Clear()
        {
            _count.gameObject.SetActive(false);
            for (var i = 0; i < _pieces.Count; i++)
                _pieces[i].SetPropertyBlock(null);
        }

        private void ThrowShards(int perCell)
        {
            var emit = new ParticleSystem.EmitParams { applyShapeToPosition = true };
            for (var i = 0; i < _shape.Count; i++)
            {
                emit.position = Top(_shape[i].X, _shape[i].Y);
                _shards.Emit(emit, perCell);
            }
        }

        private Vector3 Top(float x, float y) => new Vector3(x * _cellSize, _top, -y * _cellSize);
    }
}
