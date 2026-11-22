using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Play
{
    public sealed class BoardInputView : MonoBehaviour, IPointerDownHandler, IInitializePotentialDragHandler, IDragHandler,
        IPointerUpHandler
    {
        private const int NoPointer = int.MinValue;

        [SerializeField] private BoxCollider _area;

        private Action<Vector2> _pressed;
        private Action<Vector2> _moved;
        private Action _released;
        private int _pointer = NoPointer;

        public void Listen(Action<Vector2> pressed, Action<Vector2> moved, Action released)
        {
            _pressed = pressed;
            _moved = moved;
            _released = released;
            _pointer = NoPointer;
        }

        public void Listen(Action<Vector2> pressed) => Listen(pressed, null, null);

        public void Mute() => Listen(null, null, null);

        public void Cover(Bounds box)
        {
            _area.center = box.center;
            _area.size = box.size;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_pointer != NoPointer || _pressed == null || eventData.button != PointerEventData.InputButton.Left)
                return;
            _pointer = eventData.pointerId;
            _pressed(eventData.position);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (IsHeld(eventData))
                _moved?.Invoke(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsHeld(eventData))
                return;
            _pointer = NoPointer;
            _released?.Invoke();
        }

        private bool IsHeld(PointerEventData eventData) =>
            _pointer != NoPointer && eventData.pointerId == _pointer && eventData.button == PointerEventData.InputButton.Left;
    }
}
