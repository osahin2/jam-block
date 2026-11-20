using UnityEngine;

namespace Game.Play
{
    public static class ExponentialFollow
    {
        private const float Ln2 = 0.6931472f;

        public static float Fraction(float halfLife, float deltaTime) => 1f - Mathf.Exp(-Ln2 * deltaTime / halfLife);
    }
}
