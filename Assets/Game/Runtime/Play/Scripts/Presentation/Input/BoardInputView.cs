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

        // A new listener starts with no pointer down: a finger held from the level before moves and releases nothing.
        public void Listen(Action<Vector2> pressed, Action<Vector2> moved, Action released)
        {
            _pressed = pressed;
            _moved = moved;
            _released = released;
            _pointer = NoPointer;
        }

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

        // The block follows the finger from its first movement, without the EventSystem's drag threshold.
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        // The EventSystem sends drag and release to the pressed object, so both arrive outside the collider too.
        public void OnDrag(PointerEventData eventData)
        {
            if (IsHeld(eventData))
                _moved(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsHeld(eventData))
                return;
            _pointer = NoPointer;
            _released();
        }

        private bool IsHeld(PointerEventData eventData) =>
            _pointer != NoPointer && eventData.pointerId == _pointer && eventData.button == PointerEventData.InputButton.Left;
    }
}
