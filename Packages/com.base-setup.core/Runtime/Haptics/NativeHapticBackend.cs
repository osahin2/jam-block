namespace Core.Haptics
{
    // Picks the backend for the platform the player is built for. Android has none yet and gets the silent backend.
    internal static class NativeHapticBackend
    {
        public static IHapticBackend Create()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new IosHapticBackend();
#else
            return new NoHapticBackend();
#endif
        }
    }
}
