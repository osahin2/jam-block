using Game.Play;
using UnityEngine;

namespace Game.Editor.LevelEditor
{
    // The scene preview's pieces: plain copies of the piece prefabs, never saved with the scene and not editable, down
    // to the model under each piece's root. They are destroyed with the preview's root, so nothing is pooled in edit
    // mode.
    public sealed class PreviewBoardPieces : IBoardPieceSource
    {
        public const HideFlags PreviewFlags = HideFlags.DontSave | HideFlags.NotEditable;

        public BoardPiece Take(BoardPiece prefab, Transform parent) => Mark(Object.Instantiate(prefab, parent));

        public BlockView Take(BlockView prefab, Transform parent) => Mark(Object.Instantiate(prefab, parent));

        private static T Mark<T>(T piece) where T : Component
        {
            foreach (var part in piece.GetComponentsInChildren<Transform>(true))
                part.gameObject.hideFlags = PreviewFlags;
            return piece;
        }
    }
}
