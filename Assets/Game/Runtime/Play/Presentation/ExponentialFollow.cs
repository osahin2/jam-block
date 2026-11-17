using UnityEngine;

namespace Game.Play.Presentation
{
    // Frame-rate independent exponential approach: the distance to the target halves every half-life, however the
    // time is split into frames, and a long frame can never pass the target.
    public static class ExponentialFollow
    {
        private const float Ln2 = 0.6931472f;

        // The part of the remaining distance to cover this frame.
        public static float Fraction(float halfLife, float deltaTime) => 1f - Mathf.Exp(-Ln2 * deltaTime / halfLife);
    }
}
