using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Play
{
    // The board's place in the EventSystem: one collider over the board, frame included, on the board root's object,
    // so the collider is in the space the board is built in. A press the UI above it does not take arrives here; the
    // first pointer's press, drag and release go on as screen positions. The EventSystem sends a drag and a release to
    // the object that was pressed, so a drag that leaves the collider and a release outside it still arrive. Other
    // pointers and buttons are ignored.
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

        // Fits the collider to a box in the board root's local space.
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
