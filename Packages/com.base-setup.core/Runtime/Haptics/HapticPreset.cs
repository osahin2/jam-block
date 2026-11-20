namespace Core.Haptics
{
    public enum HapticPreset
    {
        // Plugins/iOS/CoreHapticFeedback.mm indexes its table by these numbers: append, never reorder.
        Light = 0,
        Medium = 1,
        Heavy = 2,
        Success = 3,
        Failure = 4
    }
}
