using UnityEngine;

namespace Game.Play
{
    // The root of a board piece prefab. The root stands at the piece's pivot with no turn; the supplied model sits under
    // it, turned and offset inside the prefab so every piece follows one convention (top up, a frame piece long along
    // X and facing out along +Z), and the builder places every piece the same way.
    public sealed class BoardPiece : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;

        public void SetMaterial(Material material) => _renderer.sharedMaterial = material;
    }
}
