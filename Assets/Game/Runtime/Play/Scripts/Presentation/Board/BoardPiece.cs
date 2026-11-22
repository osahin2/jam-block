using UnityEngine;

namespace Game.Play
{
    public sealed class BoardPiece : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;

        public Renderer Renderer => _renderer;

        public void SetMaterial(Material material) => _renderer.sharedMaterial = material;
    }
}
