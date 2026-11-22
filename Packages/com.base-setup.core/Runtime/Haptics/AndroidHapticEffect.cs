using UnityEngine;

namespace Core.Haptics
{
    internal sealed class AndroidHapticEffect
    {
        private const int PredefinedSince = 29;
        private const int NoRepeat = -1;

        private readonly string _predefined;
        private readonly long[] _timings;
        private readonly int[] _amplitudes;

        private AndroidHapticEffect(string predefined, long[] timings, int[] amplitudes)
        {
            _predefined = predefined;
            _timings = timings;
            _amplitudes = amplitudes;
        }

        public static AndroidHapticEffect Waveform(long[] timings, int[] amplitudes) =>
            new AndroidHapticEffect(null, timings, amplitudes);

        public static AndroidHapticEffect Predefined(string effect, long[] timings, int[] amplitudes) =>
            new AndroidHapticEffect(effect, timings, amplitudes);

        public AndroidJavaObject Create(AndroidJavaClass vibrationEffect, int sdk) =>
            _predefined != null && sdk >= PredefinedSince
                ? vibrationEffect.CallStatic<AndroidJavaObject>("createPredefined", vibrationEffect.GetStatic<int>(_predefined))
                : vibrationEffect.CallStatic<AndroidJavaObject>("createWaveform", _timings, _amplitudes, NoRepeat);
    }
}
