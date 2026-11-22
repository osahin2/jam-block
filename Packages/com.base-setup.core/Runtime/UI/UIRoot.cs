using UnityEngine;

namespace Core.UI
{
    public sealed class UIRoot : MonoBehaviour
    {
        [SerializeField] private RectTransform _screens;
        [SerializeField] private RectTransform _popups;

        internal Transform LayerParent(UILayer layer) => layer == UILayer.Popup ? _popups : _screens;

        internal static UIRoot Create(RectTransform screens, RectTransform popups)
        {
            var root = new GameObject("UIRoot").AddComponent<UIRoot>();
            root._screens = screens;
            root._popups = popups;
            return root;
        }
    }
}
