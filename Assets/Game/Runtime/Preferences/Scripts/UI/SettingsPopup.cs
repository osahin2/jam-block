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
        [SerializeField] private RectTransform _panel;
        [Tooltip("How much shorter the panel is without the home button, in UI units.")]
        [SerializeField, Min(0f)] private float _homeRoom = 170f;

        private IAudioService _audio;
        private IHapticService _haptics;
        private Action _onClose;
        private Action _onHome;
        private float _height;

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
            _height = _panel.rect.height;
        }

        public void Show(Action onClose) => Show(onClose, null);

        public void ShowWithHome(Action onClose, Action onHome) => Show(onClose, onHome);

        private void Show(Action onClose, Action onHome)
        {
            _sound.Show(!_audio.SfxMuted);
            _music.Show(!_audio.MusicMuted);
            _vibration.Show(_haptics.Enabled);
            _onClose = onClose;
            _onHome = onHome;
            FitHome(onHome != null);
        }

        private void FitHome(bool shown)
        {
            _homeButton.gameObject.SetActive(shown);
            _panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, shown ? _height : _height - _homeRoom);
        }

        private void ChooseClose()
        {
            var onClose = _onClose;
            Forget();
            Close();
            onClose?.Invoke();
        }

        private void ChooseHome()
        {
            var onHome = _onHome;
            Forget();
            onHome?.Invoke();
        }

        private void Forget()
        {
            _onClose = null;
            _onHome = null;
        }
    }
}
