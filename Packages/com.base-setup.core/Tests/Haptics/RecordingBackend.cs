using System.Collections.Generic;

namespace Core.Haptics.Tests
{
    internal sealed class RecordingBackend : IHapticBackend
    {
        public readonly List<HapticPreset> Played = new List<HapticPreset>();

        public void Play(HapticPreset preset) => Played.Add(preset);
    }
}
