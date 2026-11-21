using UnityEngine;

namespace Game.Play
{
    public sealed class CameraFraming
    {
        private const int Rounds = 5;

        private readonly CameraFramingSettings _settings;

        public CameraFraming(CameraFramingSettings settings)
        {
            _settings = settings;
        }

        public Rect ViewportWindow(Rect viewportSafeArea, float aspect)
        {
            var reference = _settings.UiReferenceResolution;
            var unit = Mathf.Min(aspect / reference.x, 1f / reference.y);
            return Rect.MinMaxRect(viewportSafeArea.xMin + _settings.SideMargin,
                viewportSafeArea.yMin + _settings.BottomMargin * unit,
                viewportSafeArea.xMax - _settings.SideMargin, viewportSafeArea.yMax - _settings.TopMargin * unit);
        }

        public void Frame(Camera camera, BoardLayout layout, Transform board, Rect viewportSafeArea)
        {
            var view = camera.transform;
            var window = ViewportWindow(viewportSafeArea, camera.aspect);
            var center = board.TransformPoint(layout.Box.center);
            var tanHalfHeight = Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var radius = Vector3.Scale(layout.Box.extents, board.lossyScale).magnitude;
            var narrowerHalfAngle = Mathf.Atan(Mathf.Min(tanHalfHeight, tanHalfHeight * camera.aspect));
            view.position = center - view.forward * (radius / Mathf.Sin(narrowerHalfAngle));
            for (var round = 0; round < Rounds; round++)
            {
                var shown = OnScreen(camera, layout, board);
                var depth = Vector3.Dot(center - view.position, view.forward);
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
