using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace Game.Buttons
{
    [RequireComponent(typeof(Selectable))]
    public sealed class PressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private ButtonFeedbackSettings _settings;
        private Selectable _selectable;
        private Vector3 _restScale;
        private bool _pressed;
        private Tween _motion;

        [Inject]
        public void Construct(ButtonFeedbackSettings settings) => _settings = settings;

        private void Awake()
        {
            _selectable = GetComponent<Selectable>();
            _restScale = transform.localScale;
        }

        private void OnDisable()
        {
            _motion?.Kill();
            _motion = null;
            _pressed = false;
            transform.localScale = _restScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || !_selectable.IsInteractable())
                return;
            _pressed = true;
            ScaleTo(_restScale * _settings.PressedScale, _settings.PressDuration, _settings.PressEase);
        }

        // The EventSystem sends the release to the pressed object, also when the finger left it first.
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_pressed || eventData.button != PointerEventData.InputButton.Left)
                return;
            _pressed = false;
            ScaleTo(_restScale, _settings.ReleaseDuration, _settings.ReleaseEase);
        }

        private void ScaleTo(Vector3 scale, float duration, Ease ease)
        {
            _motion?.Kill();
            _motion = transform.DOScale(scale, duration)
                .SetEase(ease)
                .SetUpdate(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }
    }
}
