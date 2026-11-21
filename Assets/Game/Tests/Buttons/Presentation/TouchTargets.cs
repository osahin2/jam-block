using UnityEngine;
using UnityEngine.UI;

namespace Game.Tests.Buttons
{
    public static class TouchTargets
    {
        public static bool TakesTouches(Component target)
        {
            foreach (var graphic in target.GetComponentsInChildren<Graphic>(true))
            {
                if (graphic.raycastTarget)
                    return true;
            }

            return false;
        }
    }
}
