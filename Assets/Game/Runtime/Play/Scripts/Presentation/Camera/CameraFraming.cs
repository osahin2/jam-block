using UnityEngine;

namespace Game.Play
{
    public sealed class CameraFraming
    {
        // Each round cuts the error about tenfold, so five leave the board within a pixel of the window.
        private const int Rounds = 5;

        private readonly CameraFramingSettings _settings;

        public CameraFraming(CameraFramingSettings settings)
        {
            _settings = settings;
        }

        public Rect ViewportWindow(Rect viewportSafeArea, float aspect)
        {
            var bottom = viewportSafeArea.yMin + _settings.BottomMargin * aspect;
            var top = viewportSafeArea.yMax - _settings.TopMargin * aspect;
            var window = Rect.MinMaxRect(viewportSafeArea.xMin + _settings.SideMargin, bottom,
                viewportSafeArea.xMax - _settings.SideMargin, top);
            // The margins are made for portrait phones; a wide view, such as the level editor's preview in a
            // landscape Game view, has no room left between them, so the board takes the safe area.
            return window.width > 0f && window.height > 0f ? window : viewportSafeArea;
        }

        // Perspective shows the board's near half larger than its far half, so a correction made from one
        // measurement overshoots a little; measuring again each round closes the gap.
        public void Frame(Camera camera, BoardLayout layout, Transform board, Rect viewportSafeArea)
        {
            var view = camera.transform;
            var window = ViewportWindow(viewportSafeArea, camera.aspect);
            var center = board.TransformPoint(layout.Box.center);
            var tanHalfHeight = Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            // Starting where the board's bounding sphere fits the screen keeps every corner in front of the camera,
            // wherever the camera was, and makes the result the same each time.
            var radius = Vector3.Scale(layout.Box.extents, board.lossyScale).magnitude;
            var narrowerHalfAngle = Mathf.Atan(Mathf.Min(tanHalfHeight, tanHalfHeight * camera.aspect));
            view.position = center - view.forward * (radius / Mathf.Sin(narrowerHalfAngle));
            for (var round = 0; round < Rounds; round++)
            {
                var shown = OnScreen(camera, layout, board);
                var depth = Vector3.Dot(center - view.position, view.forward);
                // A board's size on screen goes with 1 / depth.
                var fittedDepth = depth * Mathf.Max(shown.width / window.width, shown.height / window.height);
                var viewportToWorld = 2f * tanHalfHeight * fittedDepth;
                var offset = shown.center - window.center;
                view.position += view.forward * (depth - fittedDepth) +
                    view.right * (offset.x * viewportToWorld * camera.aspect) + view.up * (offset.y * viewportToWorld);
            }
        }

        private static Rect OnScreen(Camera camera, BoardLayout layout, Transform board)
        {
            var box = layout.Box;
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            for (var i = 0; i < 8; i++)
            {
                var corner = board.TransformPoint(new Vector3((i & 1) == 0 ? box.min.x : box.max.x,
                    (i & 2) == 0 ? box.min.y : box.max.y, (i & 4) == 0 ? box.min.z : box.max.z));
                Vector2 point = camera.WorldToViewportPoint(corner);
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
            }
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}
