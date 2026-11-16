using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.UI.Tests
{
    public sealed class UIServiceTests
    {
        private sealed class HudScreen : UIScreen
        {
        }

        private sealed class ConfirmPopup : UIScreen
        {
            public override UILayer Layer => UILayer.Popup;

            public void Confirm() => Close();
        }

        private readonly List<Object> _created = new List<Object>();
        private UIRoot _root;
        private RectTransform _screens;
        private RectTransform _popups;

        [SetUp]
        public void CreateRoot()
        {
            _screens = Track(new GameObject("Screens", typeof(RectTransform))).GetComponent<RectTransform>();
            _popups = Track(new GameObject("Popups", typeof(RectTransform))).GetComponent<RectTransform>();
            _root = Track(UIRoot.Create(_screens, _popups).gameObject).GetComponent<UIRoot>();
        }

        [TearDown]
        public void DestroyCreated()
        {
            foreach (Object created in _created)
                if (created != null) Object.DestroyImmediate(created);
            _created.Clear();
        }

        [Test]
        public void OpenCreatesTheScreenOnceThenReusesIt()
        {
            int created = 0;
            UIService service = Service(Catalog("Level", Prefab<HudScreen>()), () => created++);

            HudScreen first = service.Open<HudScreen>();
            HudScreen second = service.Open<HudScreen>();

            Assert.That(second, Is.SameAs(first));
            Assert.That(created, Is.EqualTo(1));
        }

        [Test]
        public void ScreensAndPopupsAreCreatedUnderTheirLayer()
        {
            UIService service = Service(Catalog("Level", Prefab<HudScreen>(), Prefab<ConfirmPopup>()));

            Assert.That(service.Open<HudScreen>().transform.parent, Is.SameAs(_screens));
            Assert.That(service.Open<ConfirmPopup>().transform.parent, Is.SameAs(_popups));
        }

        [Test]
        public void CloseDeactivatesAndOpenActivatesAgain()
        {
            UIService service = Service(Catalog("Level", Prefab<ConfirmPopup>()));
            ConfirmPopup popup = service.Open<ConfirmPopup>();

            popup.Confirm();
            Assert.That(popup.gameObject.activeSelf, Is.False);

            service.Open<ConfirmPopup>();
            Assert.That(popup.gameObject.activeSelf, Is.True);
        }

        [Test]
        public void ScreensFromSeveralCatalogsCanBeOpened()
        {
            UIService service = Service(new[] { Catalog("Level", Prefab<HudScreen>()), Catalog("Shop", Prefab<ConfirmPopup>()) });

            Assert.That(service.Open<HudScreen>(), Is.Not.Null);
            Assert.That(service.Open<ConfirmPopup>(), Is.Not.Null);
        }

        [Test]
        public void TheSameScreenTypeInTwoCatalogsIsRejected()
        {
            var catalogs = new[] { Catalog("Level", Prefab<HudScreen>()), Catalog("Shop", Prefab<HudScreen>()) };

            var error = Assert.Throws<InvalidOperationException>(() => Service(catalogs));
            Assert.That(error.Message, Does.Contain("HudScreen").And.Contain("'Level'").And.Contain("'Shop'"));
        }

        [Test]
        public void OpeningAnUnlistedScreenNamesIt()
        {
            UIService service = Service(Catalog("Level", Prefab<HudScreen>()));

            var error = Assert.Throws<InvalidOperationException>(() => service.Open<ConfirmPopup>());
            Assert.That(error.Message, Does.Contain("ConfirmPopup"));
        }

        [Test]
        public void SafeAreaFitAnchorsContentToTheSafeArea()
        {
            var content = Track(new GameObject("Content", typeof(RectTransform))).GetComponent<RectTransform>();

            SafeAreaFit.Apply(content, new Rect(0f, 100f, 1000f, 1700f), new Vector2(1000f, 2000f));

            Assert.That(content.anchorMin, Is.EqualTo(new Vector2(0f, 0.05f)));
            Assert.That(content.anchorMax, Is.EqualTo(new Vector2(1f, 0.9f)));
            Assert.That(content.offsetMin, Is.EqualTo(Vector2.zero));
            Assert.That(content.offsetMax, Is.EqualTo(Vector2.zero));
        }

        private UIService Service(UIScreenCatalog catalog, Action onCreate = null) => Service(new[] { catalog }, onCreate);

        private UIService Service(UIScreenCatalog[] catalogs, Action onCreate = null) =>
            new UIService(catalogs, _root, (prefab, parent) =>
            {
                onCreate?.Invoke();
                return Track(Object.Instantiate(prefab.gameObject, parent)).GetComponent<UIScreen>();
            });

        private UIScreenCatalog Catalog(string name, params UIScreen[] screens) => Track(UIScreenCatalog.Create(name, screens));

        // A stand-in prefab: an inactive object with the screen component and a Content child.
        private T Prefab<T>() where T : UIScreen
        {
            var root = Track(new GameObject(typeof(T).Name, typeof(RectTransform)));
            root.SetActive(false);
            var content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(root.transform, false);
            var screen = root.AddComponent<T>();

            var serialized = new SerializedObject(screen);
            serialized.FindProperty("_content").objectReferenceValue = content.transform;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return screen;
        }

        private T Track<T>(T created) where T : Object
        {
            _created.Add(created);
            return created;
        }
    }
}
