using UnityEngine;

namespace Game.Play
{
    // Finds the point under a screen position on the plane of the blocks' tops, in the board root's local space: the
    // space the board is laid out and built in, so the point goes straight to BoardLayout and a moved, turned or
    // scaled board root stays right.
    public sealed class TouchPlane
    {
        private readonly Camera _camera;
        private readonly Transform _board;
        private readonly Plane _plane;

        public TouchPlane(Camera camera, BoardRoot board, BoardArtSettings art)
        {
            _camera = camera;
            _board = board.transform;
            _plane = new Plane(Vector3.up, new Vector3(0f, art.TouchHeight, 0f));
        }

        // False when the ray under the position runs along the plane or away from it.
        public bool TryProject(Vector2 screen, out Vector3 local)
        {
            var ray = _camera.ScreenPointToRay(screen);
            var onBoard = new Ray(_board.InverseTransformPoint(ray.origin), _board.InverseTransformVector(ray.direction));
            if (_plane.Raycast(onBoard, out var distance))
            {
                local = onBoard.GetPoint(distance);
                return true;
            }
            local = default;
            return false;
        }
    }
}
