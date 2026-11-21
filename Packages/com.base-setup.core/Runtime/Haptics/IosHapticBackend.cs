// Only for iOS: IL2CPP links the "__Internal" import directly and the symbol exists only in the iOS player.
#if UNITY_IOS
using System.Runtime.InteropServices;

namespace Core.Haptics
{
    internal sealed class IosHapticBackend : IHapticBackend
    {
        public void Play(HapticPreset preset) => CoreHapticFeedback_Play((int)preset);

        [DllImport("__Internal")]
        private static extern void CoreHapticFeedback_Play(int preset);
    }
}
#endif
