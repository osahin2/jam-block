using UnityEngine;

namespace Core.UI
{
    // Applied once when a screen is created: the game is portrait-only on phones, so the safe area never changes at run time.
    internal static class SafeAreaFit
    {
        public static void Apply(RectTransform content, Rect safeArea, Vector2 screenSize)
        {
            content.anchorMin = safeArea.position / screenSize;
            content.anchorMax = (safeArea.position + safeArea.size) / screenSize;
            content.offsetMin = Vector2.zero;
            content.offsetMax = Vector2.zero;
        }
    }
}
