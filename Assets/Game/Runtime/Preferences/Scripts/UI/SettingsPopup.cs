using System;
using Core.Audio;
using Core.Haptics;
using Core.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Preferences
{
    // The player's sound, music and vibration choices, written straight to Core's audio and haptics services, which
    // save them. What close and home lead to is the caller's.
    public sealed class SettingsPopup : UIScreen
    {
        [SerializeField] private SwitchView _sound;
        [SerializeField] private SwitchView _music;
        [SerializeField] private SwitchView _vibration;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _homeButton;

        private IAudioService _audio;
        private IHapticService _haptics;
        private Action _onClose;
        private Action _onHome;

        public override UILayer Layer => UILayer.Popup;

        [Inject]
        public void Construct(IAudioService audio, IHapticService haptics)
        {
            _audio = audio;
            _haptics = haptics;
        }

        private void Awake()
        {
            _sound.Listen(on => _audio.SfxMuted = !on);
            _music.Listen(on => _audio.MusicMuted = !on);
            _vibration.Listen(on => _haptics.Enabled = on);
            _closeButton.onClick.AddListener(ChooseClose);
            _homeButton.onClick.AddListener(ChooseHome);
        }

        public void Show(Action onClose, Action onHome)
        {
            _sound.Show(!_audio.SfxMuted);
            _music.Show(!_audio.MusicMuted);
            _vibration.Show(_haptics.Enabled);
            _onClose = onClose;
            _onHome = onHome;
        }

        private void ChooseClose() => End(_onClose);

        private void ChooseHome() => End(_onHome);

        // Clears both callbacks before calling the chosen one, so a second click does nothing.
        private void End(Action chosen)
        {
            _onClose = null;
            _onHome = null;
            Close();
            chosen?.Invoke();
        }
    }
}
