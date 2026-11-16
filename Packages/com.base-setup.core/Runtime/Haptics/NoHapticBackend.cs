namespace Core.Haptics
{
    // The editor and platforms without a native backend: presets are accepted and nothing happens.
    internal sealed class NoHapticBackend : IHapticBackend
    {
        public void Play(HapticPreset preset)
        {
        }
    }
}
