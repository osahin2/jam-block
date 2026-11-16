using UnityEngine;

namespace Core.Audio
{
    // Drives the players from the engine loop in unscaled time, so fades and source returns continue while the game is paused.
    internal sealed class AudioTicker : MonoBehaviour
    {
        private SfxPlayer _sfx;
        private MusicPlayer _music;

        internal void Init(SfxPlayer sfx, MusicPlayer music)
        {
            _sfx = sfx;
            _music = music;
        }

        private void Update()
        {
            float deltaTime = Time.unscaledDeltaTime;
            _sfx.Tick(deltaTime);
            _music.Tick(deltaTime);
        }
    }
}
