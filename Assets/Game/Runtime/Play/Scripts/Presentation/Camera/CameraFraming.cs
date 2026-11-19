using UnityEngine;

namespace Game.Play
{
    // Moves the camera so the whole board, frame included, fits the screen inside the framing margins. The camera
    // keeps its rotation and field of view and moves along its view axis to or from the board's center, so every level
    // is seen from the same angle, as close as its size and the screen's aspect allow.
    public sealed class CameraFraming
    {
        private readonly CameraFramingSettings _settings;

        public CameraFraming(CameraFramingSettings settings)
        {
            _settings = settings;
        }

        public void Frame(Camera camera, BoardLayout layout, Transform board)
        {
            var view = camera.transform;
            var forward = view.forward;
            var right = view.right;
            var up = view.up;
            var tanUp = Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var tanRight = tanUp * camera.aspect * (1f - 2f * _settings.SideMargin);
            tanUp *= 1f - 2f * _settings.TopAndBottomMargin;
            var min = layout.Box.min;
            var max = layout.Box.max;

            // Each of the eight corners of the board's box needs the camera at least this far back to fit across and up
            // the screen.
            var distance = 0f;
            for (var i = 0; i < 8; i++)
            {
                var corner = board.TransformVector(new Vector3(
                    (i & 1) == 0 ? min.x : max.x, (i & 2) == 0 ? min.y : max.y, (i & 4) == 0 ? min.z : max.z));
                var depth = Vector3.Dot(corner, forward);
                distance = Mathf.Max(distance,
                    Mathf.Abs(Vector3.Dot(corner, right)) / tanRight - depth,
                    Mathf.Abs(Vector3.Dot(corner, up)) / tanUp - depth);
            }
            view.position = board.position - forward * distance;
        }
    }
}
