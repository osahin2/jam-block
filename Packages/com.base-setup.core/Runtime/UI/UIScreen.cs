using UnityEngine;

namespace Core.UI
{
    public abstract class UIScreen : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;

        private IUIService _service;

        public virtual UILayer Layer => UILayer.Screen;

        // Screens are variants of the Core screen template, where Content is the safe-area child of a full-screen root.
        internal RectTransform Content => _content;

        internal void Bind(IUIService service) => _service = service;

        protected void Close() => _service.Close(this);
    }
}
