using Game.Play;
using UnityEngine;

namespace Game.Editor.LevelEditor
{
    // The scene preview's pieces: plain copies of the piece prefabs, never saved with the scene and not editable. They
    // are destroyed with the preview's root, so nothing is pooled in edit mode.
    public sealed class PreviewBoardPieces : IBoardPieceSource
    {
        private const HideFlags PreviewFlags = HideFlags.DontSave | HideFlags.NotEditable;

        public MeshRenderer Take(MeshRenderer prefab, Transform parent) => Mark(Object.Instantiate(prefab, parent));

        public BlockView Take(BlockView prefab, Transform parent) => Mark(Object.Instantiate(prefab, parent));

        private static T Mark<T>(T piece) where T : Component
        {
            piece.gameObject.hideFlags = PreviewFlags;
            return piece;
        }
    }
}
