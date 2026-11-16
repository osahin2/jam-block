using System.Collections.Generic;

namespace Core.Haptics.Tests
{
    // Records every preset it is asked to play.
    internal sealed class RecordingBackend : IHapticBackend
    {
        public readonly List<HapticPreset> Played = new List<HapticPreset>();

        public void Play(HapticPreset preset) => Played.Add(preset);
    }
}
