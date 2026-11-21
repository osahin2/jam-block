using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Play
{
    public sealed class AimingOverlay : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private RectTransform _top;
        [SerializeField] private RectTransform _bottom;
        [SerializeField] private RectTransform _left;
        [SerializeField] private RectTransform _right;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _hint;
        [SerializeField] private RectTransform _button;
        [SerializeField] private Image _buttonIcon;
        [SerializeField] private BoosterBar _bar;

        private Action _onCancel;

        public void Show(BoosterDefinition booster, Rect boardOnScreen, Action onCancel)
        {
            _onCancel = onCancel;
            _title.SetText(booster.Title);
            _hint.SetText(booster.Description);
            _buttonIcon.sprite = booster.Icon;
            gameObject.SetActive(true);
            Cut(boardOnScreen);
            Copy(_bar.SlotOf(booster));
        }

        public void Hide()
        {
            _onCancel = null;
            gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                _onCancel?.Invoke();
        }

        private void Cut(Rect boardOnScreen)
        {
            var area = (RectTransform)transform;
            var screen = area.rect;
            var min = ToLocal(area, boardOnScreen.min);
            var max = ToLocal(area, boardOnScreen.max);
            Place(_top, screen.xMin, max.y, screen.xMax, screen.yMax);
            Place(_bottom, screen.xMin, screen.yMin, screen.xMax, min.y);
            Place(_left, screen.xMin, min.y, min.x, max.y);
            Place(_right, max.x, min.y, screen.xMax, max.y);
        }

        private void Copy(RectTransform slot)
        {
            var parentScale = _button.parent.lossyScale;
            var slotScale = slot.lossyScale;
            _button.position = slot.position;
            _button.sizeDelta = slot.rect.size;
            _button.localScale = new Vector3(slotScale.x / parentScale.x, slotScale.y / parentScale.y, 1f);
        }

        private static Vector2 ToLocal(RectTransform area, Vector2 screen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(area, screen, null, out var local);
            return local;
        }

        private static void Place(RectTransform dim, float xMin, float yMin, float xMax, float yMax)
        {
            dim.anchoredPosition = new Vector2(xMin, yMin);
            dim.sizeDelta = new Vector2(Mathf.Max(0f, xMax - xMin), Mathf.Max(0f, yMax - yMin));
        }
    }
}
