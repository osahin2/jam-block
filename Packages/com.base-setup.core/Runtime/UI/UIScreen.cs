using UnityEngine;

namespace Core.UI
{
    // A screen declares its own typed Show method; buttons the caller reacts to are named callbacks there.
    // Screens are prefab variants of the Core screen template, so Content is the safe-area child of a full-screen root.
    public abstract class UIScreen : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;

        private IUIService _service;

        public virtual UILayer Layer => UILayer.Screen;

        internal RectTransform Content => _content;

        internal void Bind(IUIService service) => _service = service;

        protected void Close() => _service.Close(this);
    }
}
