using UnityEngine;

namespace Core.Audio
{
    internal sealed class AudioTicker : MonoBehaviour
    {
        private SfxPlayer _sfx;
        private MusicPlayer _music;

        internal void Init(SfxPlayer sfx, MusicPlayer music)
        {
            _sfx = sfx;
            _music = music;
        }

        // Unscaled time, so fades and source returns go on while the game is paused.
        private void Update()
        {
            float deltaTime = Time.unscaledDeltaTime;
            _sfx.Tick(deltaTime);
            _music.Tick(deltaTime);
        }
    }
}
