using Game.Play;
using UnityEngine;

namespace Game.Editor.LevelEditor
{
    public sealed class PreviewBoardPieces : IBoardPieceSource
    {
        public const HideFlags PreviewFlags = HideFlags.DontSave | HideFlags.NotEditable;

        // Nothing is pooled in edit mode: the copies are destroyed with the preview's root.
        public T Take<T>(T prefab, Transform parent) where T : Component
        {
            var piece = Object.Instantiate(prefab, parent);
            foreach (var part in piece.GetComponentsInChildren<Transform>(true))
                part.gameObject.hideFlags = PreviewFlags;
            return piece;
        }
    }
}
