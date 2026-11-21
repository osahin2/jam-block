using System;
using Core.Audio;
using Core.Haptics;
using Core.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Preferences
{
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

        private void ChooseClose()
        {
            var onClose = _onClose;
            Forget();
            Close();
            onClose?.Invoke();
        }

        // Stays open: home covers the screen with the scene fade first, so the dimmer never drops over the board.
        private void ChooseHome()
        {
            var onHome = _onHome;
            Forget();
            onHome?.Invoke();
        }

        // Both callbacks go before the chosen one runs, so a second click does nothing.
        private void Forget()
        {
            _onClose = null;
            _onHome = null;
        }
    }
}
