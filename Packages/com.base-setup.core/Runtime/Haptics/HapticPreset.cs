namespace Core.Haptics
{
    // What the feedback means, not how it is produced; each backend maps a preset to its platform's closest effect.
    // The numbers are part of the native contract (Plugins/iOS/CoreHapticFeedback.mm indexes its table by them):
    // append new presets, never reorder.
    public enum HapticPreset
    {
        Light = 0,
        Medium = 1,
        Heavy = 2,
        Success = 3,
        Failure = 4
    }
}
