namespace Core.Haptics
{
    // The service calls it only while haptics are enabled, so a backend does not check the player's setting.
    public interface IHapticBackend
    {
        void Play(HapticPreset preset);
    }
}
