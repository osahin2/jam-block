using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor.Window
{
    // Lets a thin handle be dragged to resize the pane next to it. The new size goes back through the callback, where
    // the window keeps it within its limits and across reloads.
    internal sealed class PaneResizer : PointerManipulator
    {
        // The drag direction that makes the pane bigger, in panel coordinates (y down).
        private readonly Vector2 _grow;
        private readonly Func<float> _size;
        private readonly Action<float> _resize;
        private Vector2 _start;
        private float _startSize;

        public PaneResizer(Vector2 grow, Func<float> size, Action<float> resize)
        {
            _grow = grow;
            _size = size;
            _resize = resize;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0)
                return;
            _start = evt.position;
            _startSize = _size();
            target.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!target.HasPointerCapture(evt.pointerId))
                return;
            var dragged = Vector2.Dot((Vector2)evt.position - _start, _grow);
            _resize(_startSize + dragged);
            evt.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!target.HasPointerCapture(evt.pointerId))
                return;
            target.ReleasePointer(evt.pointerId);
            evt.StopPropagation();
        }
    }
}
