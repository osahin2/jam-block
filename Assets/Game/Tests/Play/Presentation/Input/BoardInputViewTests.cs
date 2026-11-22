using System.Collections.Generic;
using Game.Play;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Tests.Play
{
    public sealed class BoardInputViewTests
    {
        private const int First = 1;
        private const int Second = 2;

        private readonly List<Vector2> _pressed = new List<Vector2>();
        private readonly List<Vector2> _moved = new List<Vector2>();
        private int _released;
        private BoardInputView _view;
        private BoxCollider _area;

        [SetUp]
        public void SetUp()
        {
            var host = new GameObject("Board Input");
            _area = host.AddComponent<BoxCollider>();
            _view = host.AddComponent<BoardInputView>();
            var fields = new SerializedObject(_view);
            fields.FindProperty("_area").objectReferenceValue = _area;
            fields.ApplyModifiedPropertiesWithoutUndo();
            _pressed.Clear();
            _moved.Clear();
            _released = 0;
            Listen();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_view.gameObject);
        }

        [Test]
        public void ThePointersPressDragAndRelease_GoOnAsScreenPositions()
        {
            _view.OnPointerDown(Event(First, new Vector2(10f, 20f)));
            _view.OnDrag(Event(First, new Vector2(30f, 40f)));
            _view.OnPointerUp(Event(First, new Vector2(30f, 40f)));

            CollectionAssert.AreEqual(new[] { new Vector2(10f, 20f) }, _pressed);
            CollectionAssert.AreEqual(new[] { new Vector2(30f, 40f) }, _moved);
            Assert.AreEqual(1, _released);
        }

        [Test]
        public void ASecondPointer_WhileTheFirstIsDown_IsIgnored()
        {
            _view.OnPointerDown(Event(First, Vector2.zero));

            _view.OnPointerDown(Event(Second, Vector2.one));
            _view.OnDrag(Event(Second, Vector2.one));
            _view.OnPointerUp(Event(Second, Vector2.one));

            Assert.AreEqual(1, _pressed.Count);
            Assert.AreEqual(0, _moved.Count);
            Assert.AreEqual(0, _released);
            _view.OnDrag(Event(First, Vector2.one));
            _view.OnPointerUp(Event(First, Vector2.one));
            Assert.AreEqual(1, _moved.Count);
            Assert.AreEqual(1, _released);
        }

        [Test]
        public void APointerAfterTheFirstIsUp_IsHeard()
        {
            _view.OnPointerDown(Event(First, Vector2.zero));
            _view.OnPointerUp(Event(First, Vector2.zero));

            _view.OnPointerDown(Event(Second, Vector2.one));

            Assert.AreEqual(2, _pressed.Count);
        }

        [Test]
        public void AnotherButton_IsIgnored()
        {
            var right = PointerEventData.InputButton.Right;

            _view.OnPointerDown(Event(First, Vector2.zero, right));
            _view.OnDrag(Event(First, Vector2.one, right));
            _view.OnPointerUp(Event(First, Vector2.one, right));

            Assert.AreEqual(0, _pressed.Count);
            Assert.AreEqual(0, _moved.Count);
            Assert.AreEqual(0, _released);
        }

        [Test]
        public void ADragOrReleaseWithoutAPress_IsIgnored()
        {
            _view.OnDrag(Event(First, Vector2.one));
            _view.OnPointerUp(Event(First, Vector2.one));

            Assert.AreEqual(0, _moved.Count);
            Assert.AreEqual(0, _released);
        }

        [Test]
        public void ANewListener_StartsWithNoPointerDown()
        {
            _view.OnPointerDown(Event(First, Vector2.zero));

            Listen();
            _view.OnDrag(Event(First, Vector2.one));
            _view.OnPointerUp(Event(First, Vector2.one));
            _view.OnPointerDown(Event(Second, Vector2.one));

            Assert.AreEqual(0, _moved.Count);
            Assert.AreEqual(0, _released);
            Assert.AreEqual(2, _pressed.Count);
        }

        [Test]
        public void AListenerForPressesOnly_HearsThePress_AndLetsTheDragAndReleaseGo()
        {
            _view.Listen(_pressed.Add);

            _view.OnPointerDown(Event(First, Vector2.zero));
            _view.OnDrag(Event(First, Vector2.one));
            _view.OnPointerUp(Event(First, Vector2.one));
            _view.OnPointerDown(Event(Second, Vector2.one));

            Assert.AreEqual(2, _pressed.Count);
            Assert.AreEqual(0, _moved.Count);
            Assert.AreEqual(0, _released);
        }

        [Test]
        public void Mute_WhileAPointerIsDown_HearsNothingMore()
        {
            _view.OnPointerDown(Event(First, Vector2.zero));

            _view.Mute();
            _view.OnDrag(Event(First, Vector2.one));
            _view.OnPointerUp(Event(First, Vector2.one));
            _view.OnPointerDown(Event(Second, Vector2.one));

            Assert.AreEqual(1, _pressed.Count);
            Assert.AreEqual(0, _moved.Count);
            Assert.AreEqual(0, _released);
        }

        [Test]
        public void OnInitializePotentialDrag_TurnsTheDragThresholdOff()
        {
            var data = Event(First, Vector2.zero);

            _view.OnInitializePotentialDrag(data);

            Assert.IsFalse(data.useDragThreshold);
        }

        [Test]
        public void Cover_FitsTheColliderToTheBox()
        {
            var box = new Bounds(new Vector3(0f, 0.8f, 0f), new Vector3(12f, 1.6f, 14f));

            _view.Cover(box);

            Assert.AreEqual(box.center, _area.center);
            Assert.AreEqual(box.size, _area.size);
        }

        private static PointerEventData Event(int pointer, Vector2 position,
            PointerEventData.InputButton button = PointerEventData.InputButton.Left) =>
            new PointerEventData(null) { pointerId = pointer, position = position, button = button };

        private void Listen()
        {
            _view.Listen(_pressed.Add, _moved.Add, () => _released++);
        }
    }
}
