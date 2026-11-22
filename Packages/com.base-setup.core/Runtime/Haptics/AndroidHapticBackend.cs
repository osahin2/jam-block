using System;
using UnityEngine;

namespace Core.Haptics
{
    internal sealed class AndroidHapticBackend : IHapticBackend
    {
        private const int EffectsSince = 26;
        private const string VibrateWithEffect = "(Landroid/os/VibrationEffect;)V";

        // Indexed by HapticPreset.
        internal static readonly AndroidHapticEffect[] Effects =
        {
            AndroidHapticEffect.Predefined("EFFECT_CLICK", new long[] { 0, 20 }, new[] { 0, 100 }),
            AndroidHapticEffect.Waveform(new long[] { 0, 35 }, new[] { 0, 170 }),
            AndroidHapticEffect.Predefined("EFFECT_HEAVY_CLICK", new long[] { 0, 50 }, new[] { 0, 255 }),
            AndroidHapticEffect.Waveform(new long[] { 0, 40, 70, 90 }, new[] { 0, 140, 0, 255 }),
            AndroidHapticEffect.Waveform(new long[] { 0, 60, 60, 60, 60, 100 }, new[] { 0, 255, 0, 200, 0, 255 }),
            AndroidHapticEffect.Predefined("EFFECT_TICK", new long[] { 0, 10 }, new[] { 0, 70 })
        };

        private readonly AndroidJavaObject _vibrator;
        private readonly AndroidJavaObject[] _effects;
        private readonly IntPtr _vibrate;
        private readonly jvalue[][] _arguments;

        private AndroidHapticBackend(AndroidJavaObject vibrator, int sdk)
        {
            _vibrator = vibrator;
            _effects = CreateEffects(sdk);
            _vibrate = AndroidJNI.GetMethodID(vibrator.GetRawClass(), "vibrate", VibrateWithEffect);
            _arguments = Arguments(_effects);
        }

        // Silent below API 26 and on devices without a vibrator.
        public static IHapticBackend Create()
        {
            var sdk = SdkLevel();
            if (sdk < EffectsSince)
                return new NoHapticBackend();

            var vibrator = FindVibrator();
            if (vibrator != null && vibrator.Call<bool>("hasVibrator"))
                return new AndroidHapticBackend(vibrator, sdk);

            vibrator?.Dispose();
            return new NoHapticBackend();
        }

        public void Play(HapticPreset preset)
        {
            AndroidJNI.CallVoidMethod(_vibrator.GetRawObject(), _vibrate, _arguments[(int)preset]);
            ClearException();
        }

        private static int SdkLevel()
        {
            using var version = new AndroidJavaClass("android.os.Build$VERSION");
            return version.GetStatic<int>("SDK_INT");
        }

        private static AndroidJavaObject FindVibrator()
        {
            using var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            return activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        }

        private static AndroidJavaObject[] CreateEffects(int sdk)
        {
            using var vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
            var effects = new AndroidJavaObject[Effects.Length];
            for (var i = 0; i < effects.Length; i++)
                effects[i] = Effects[i].Create(vibrationEffect, sdk);
            return effects;
        }

        private static jvalue[][] Arguments(AndroidJavaObject[] effects)
        {
            var arguments = new jvalue[effects.Length][];
            for (var i = 0; i < effects.Length; i++)
                arguments[i] = new[] { new jvalue { l = effects[i].GetRawObject() } };
            return arguments;
        }

        // A raw JNI call does not throw; a failed vibrate must not leave a pending Java exception for the next call.
        private static void ClearException()
        {
            var exception = AndroidJNI.ExceptionOccurred();
            if (exception == IntPtr.Zero)
                return;

            AndroidJNI.ExceptionClear();
            AndroidJNI.DeleteLocalRef(exception);
        }
    }
}
