namespace Core.Haptics
{
    internal static class NativeHapticBackend
    {
        public static IHapticBackend Create()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new IosHapticBackend();
#elif UNITY_ANDROID && !UNITY_EDITOR
            return AndroidHapticBackend.Create();
#else
            return new NoHapticBackend();
#endif
        }
    }
}
