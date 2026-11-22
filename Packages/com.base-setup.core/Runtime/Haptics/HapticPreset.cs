namespace Core.Haptics
{
    public enum HapticPreset
    {
        // The iOS table (Plugins/iOS/CoreHapticFeedback.mm) and the Android one index by these numbers: append, never reorder.
        Light = 0,
        Medium = 1,
        Heavy = 2,
        Success = 3,
        Failure = 4,
        Selection = 5
    }
}
