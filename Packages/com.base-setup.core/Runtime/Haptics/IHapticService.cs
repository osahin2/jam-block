namespace Core.Haptics
{
    // The player's on/off choice is saved through ISaveService and restored when the service is created.
    public interface IHapticService
    {
        // Does nothing while haptics are disabled or on a platform without a backend.
        void Play(HapticPreset preset);

        // Each change is saved immediately.
        bool Enabled { get; set; }
    }
}
