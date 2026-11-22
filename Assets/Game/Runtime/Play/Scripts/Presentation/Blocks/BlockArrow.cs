using System;
using Game.Data.Levels;
using UnityEngine;

namespace Game.Play
{
    public sealed class BlockArrow : MonoBehaviour
    {
        private static readonly Quaternion VerticalPose = Quaternion.Euler(90f, 0f, 0f);
        private static readonly Quaternion HorizontalPose = Quaternion.Euler(0f, 90f, 0f) * VerticalPose;

        [Tooltip("The supplied arrows by length, one, two and three cells long, as imported: long along Y, facing -Z.")]
        [SerializeField] private Renderer[] _byLength;

        public int LongestModel => _byLength.Length;

        public void Show(ArrowData arrow, BoardArtSettings art)
        {
            var length = arrow.IsSet ? Math.Min(arrow.Length, _byLength.Length) : 0;
            for (var i = 0; i < _byLength.Length; i++)
                _byLength[i].gameObject.SetActive(i == length - 1);
            if (length == 0)
                return;
            var middle = arrow.Middle();
            transform.SetLocalPositionAndRotation(
                new Vector3(middle.X * art.CellSize, art.TouchHeight, -middle.Y * art.CellSize),
                arrow.Axis == ArrowAxis.Horizontal ? HorizontalPose : VerticalPose);
            transform.localScale = Vector3.one * art.Scale;
        }
    }
}
