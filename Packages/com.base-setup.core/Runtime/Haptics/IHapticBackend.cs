namespace Core.Haptics
{
    // Produces the feedback on the device. The module picks the platform's native backend; a game can hand its own
    // (for example an adapter over a third-party haptics package) to RegisterCoreHaptics. The service calls it only
    // while haptics are enabled, so a backend does not check the player's setting.
    public interface IHapticBackend
    {
        void Play(HapticPreset preset);
    }
}
