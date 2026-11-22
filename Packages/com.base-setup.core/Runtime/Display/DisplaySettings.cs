using UnityEngine;

namespace Core.Display
{
    [CreateAssetMenu(menuName = "Core/Display/Display Settings", fileName = "DisplaySettings")]
    public sealed class DisplaySettings : ScriptableObject
    {
        [Tooltip("Frames per second the app aims for. Android and iOS run at 30 unless this is set.")]
        [SerializeField, Min(1)] private int _targetFrameRate = 60;

        [Tooltip("Keeps the screen from dimming and locking while the app is in front.")]
        [SerializeField] private bool _keepScreenOn = true;

        internal void Apply()
        {
            Application.targetFrameRate = _targetFrameRate;
            Screen.sleepTimeout = _keepScreenOn ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
        }
    }
}
