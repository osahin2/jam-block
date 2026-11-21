using System;
using System.Collections.Generic;
using Game.Data.Levels;
using Game.Logic.Play;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

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
        [SerializeField] private RectTransform _crosshair;
        [SerializeField, Range(0f, 1f)] private float _crosshairSize = 0.6f;

        private readonly List<RectTransform> _crosshairs = new List<RectTransform>();
        private BoosterMotionSettings _motion;
        private Action _onCancel;

        [Inject]
        public void Construct(BoosterMotionSettings motion) => _motion = motion;

        public void Show(BoosterDefinition booster, Rect boardOnScreen, Action onCancel)
        {
            _onCancel = onCancel;
            HideCrosshairs();
            _title.SetText(booster.Title);
            _hint.SetText(booster.Description);
            _buttonIcon.sprite = booster.Icon;
            gameObject.SetActive(true);
            Cut(boardOnScreen);
            Copy(_bar.SlotOf(booster));
        }

        public void MarkTargets(IBoardEffect effect, IReadOnlyBoard board, BoardInteraction interaction)
        {
            var marked = 0;
            var blocks = board.Blocks;
            for (var i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                if (block.IsGone)
                    continue;
                for (var k = 0; k < block.Shape.Count; k++)
                {
                    var cell = block.Position + block.Shape[k];
                    if (effect.CanTarget(board, cell))
                        Mark(marked++, cell, interaction);
                }
            }
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

        private void Mark(int index, Cell cell, BoardInteraction interaction)
        {
            if (index == _crosshairs.Count)
                _crosshairs.Add(Instantiate(_crosshair, _crosshair.parent));
            var area = (RectTransform)transform;
            var center = ToLocal(area, interaction.ScreenPointOf(cell, _motion.TargetLift));
            var next = ToLocal(area, interaction.ScreenPointOf(cell + new Cell(1, 0), _motion.TargetLift));
            var size = Vector2.Distance(center, next) * _crosshairSize;
            var mark = _crosshairs[index];
            mark.anchoredPosition = center;
            mark.sizeDelta = new Vector2(size, size);
            mark.gameObject.SetActive(true);
        }

        private void HideCrosshairs()
        {
            for (var i = 0; i < _crosshairs.Count; i++)
                _crosshairs[i].gameObject.SetActive(false);
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
