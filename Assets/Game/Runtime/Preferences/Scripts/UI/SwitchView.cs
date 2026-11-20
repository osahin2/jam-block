using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Preferences
{
    public sealed class SwitchView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _on;
        [SerializeField] private GameObject _off;

        private bool _isOn;
        private Action<bool> _onChanged;

        private void Awake() => _button.onClick.AddListener(Flip);

        public void Listen(Action<bool> onChanged) => _onChanged = onChanged;

        public void Show(bool isOn)
        {
            _isOn = isOn;
            _on.SetActive(isOn);
            _off.SetActive(!isOn);
        }

        private void Flip()
        {
            Show(!_isOn);
            _onChanged?.Invoke(_isOn);
        }
    }
}
