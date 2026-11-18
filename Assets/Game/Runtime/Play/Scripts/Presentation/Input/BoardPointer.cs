using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Game.Play
{
    // Forwards the pointer (mouse or touch) as points on the plane of the blocks' tops.
    public sealed class BoardPointer : ITickable
    {
        private readonly Camera _camera;
        private readonly Plane _plane;
        private Action<Vector3> _pressed;
        private Action<Vector3> _moved;
        private Action _released;
        private bool _isDown;

        public BoardPointer(Camera camera, BoardArtSettings art)
        {
            _camera = camera;
            _plane = new Plane(Vector3.up, new Vector3(0f, art.TouchHeight, 0f));
        }

        public void Listen(Action<Vector3> pressed, Action<Vector3> moved, Action released)
        {
            _pressed = pressed;
            _moved = moved;
            _released = released;
        }

        public void Tick()
        {
            var pointer = Pointer.current;
            if (pointer == null || _pressed == null)
                return;

            var press = pointer.press;
            if (press.wasPressedThisFrame && TryHit(pointer, out var point))
            {
                _isDown = true;
                _pressed(point);
            }
            else if (_isDown && press.isPressed && TryHit(pointer, out point))
            {
                _moved(point);
            }

            if (_isDown && press.wasReleasedThisFrame)
            {
                _isDown = false;
                _released();
            }
        }

        private bool TryHit(Pointer pointer, out Vector3 point)
        {
            var ray = _camera.ScreenPointToRay(pointer.position.ReadValue());
            if (_plane.Raycast(ray, out var distance))
            {
                point = ray.GetPoint(distance);
                return true;
            }
            point = default;
            return false;
        }
    }
}
