namespace Core.Haptics
{
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
