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
        private int _shown;

        public Vector3 Landing => _icon.position;
        public float LandingWidth => _icon.rect.width * _icon.lossyScale.x;

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
            _icon.DOKill(true);
            _icon.DOPunchScale(Vector3.one * _settings.LandPunch, _settings.LandPunchDuration, 1, 0f)
                .SetUpdate(true)
                .SetLink(gameObject);
        }
    }
}
