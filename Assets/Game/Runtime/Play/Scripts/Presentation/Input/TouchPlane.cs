using UnityEngine;

namespace Game.Play
{
    public sealed class TouchPlane
    {
        private readonly Camera _camera;
        private readonly Transform _board;
        private readonly BoardArtSettings _art;

        public TouchPlane(Camera camera, BoardRoot board, BoardArtSettings art)
        {
            _camera = camera;
            _board = board.transform;
            _art = art;
        }

        public bool TryProject(Vector2 screen, out Vector3 local)
        {
            var ray = _camera.ScreenPointToRay(screen);
            var onBoard = new Ray(_board.InverseTransformPoint(ray.origin), _board.InverseTransformVector(ray.direction));
            var plane = new Plane(Vector3.up, new Vector3(0f, _art.TouchHeight, 0f));
            if (plane.Raycast(onBoard, out var distance))
            {
                local = onBoard.GetPoint(distance);
                return true;
            }
            local = default;
            return false;
        }

        public Vector2 ToScreen(Vector3 local) => _camera.WorldToScreenPoint(_board.TransformPoint(local));
    }
}
