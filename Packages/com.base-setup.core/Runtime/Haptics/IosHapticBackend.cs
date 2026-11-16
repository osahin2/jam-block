#if UNITY_IOS
using System.Runtime.InteropServices;

namespace Core.Haptics
{
    // Calls the UIKit feedback generators through Plugins/iOS/CoreHapticFeedback.mm. Compiled only for iOS, because
    // IL2CPP links an "__Internal" import directly and the symbol exists only in the iOS player.
    internal sealed class IosHapticBackend : IHapticBackend
    {
        public void Play(HapticPreset preset) => CoreHapticFeedback_Play((int)preset);

        [DllImport("__Internal")]
        private static extern void CoreHapticFeedback_Play(int preset);
    }
}
#endif
