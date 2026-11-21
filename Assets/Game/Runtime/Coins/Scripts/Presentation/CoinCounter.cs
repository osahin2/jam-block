using DG.Tweening;
using TMPro;
using UnityEngine;
using VContainer;

namespace Game.Coins
{
    public sealed class CoinCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private RectTransform _icon;

        private CoinFlightSettings _settings;
        private Tween _punch;
        private int _shown;

        public Vector3 Landing => _icon.position;

        [Inject]
        public void Construct(CoinFlightSettings settings) => _settings = settings;

        public void Show(int coins)
        {
            _shown = coins;
            _label.SetText("{0}", coins);
        }

        public void Land(int coins)
        {
            Show(_shown + coins);
            _punch ??= _icon.DOPunchScale(Vector3.one * _settings.LandPunch, _settings.LandPunchDuration, 1, 0f)
                .SetAutoKill(false)
                .SetUpdate(true)
                .SetLink(gameObject)
                .Pause();
            _punch.Restart();
        }
    }
}
