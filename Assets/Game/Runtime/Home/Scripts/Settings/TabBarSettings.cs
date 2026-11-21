using DG.Tweening;
using UnityEngine;

namespace Game.Home
{
    [CreateAssetMenu(menuName = "Game/Home/Tab Bar Settings")]
    public sealed class TabBarSettings : ScriptableObject
    {
        [Tooltip("Seconds for a tab's icon and label to take or give up the selected look; its width and card change at once.")]
        [SerializeField, Min(0f)] private float _selectDuration = 0.18f;
        [SerializeField] private Ease _selectEase = Ease.OutQuad;
        [Tooltip("The selected tab's share of the bar's width, against 1 for each other tab.")]
        [SerializeField, Min(1f)] private float _selectedWidth = 1.25f;
        [Tooltip("How far the selected tab's icon rises, in UI units.")]
        [SerializeField] private float _iconRise = 68f;
        [Tooltip("The selected tab's icon size against its size at rest.")]
        [SerializeField, Min(0f)] private float _iconScale = 1.4f;
        [Tooltip("Seconds a locked tab's icon shakes when pressed.")]
        [SerializeField, Min(0f)] private float _shakeDuration = 0.4f;
        [Tooltip("How far a locked tab's icon shakes sideways, in UI units.")]
        [SerializeField, Min(0f)] private float _shakeStrength = 18f;
        [SerializeField, Min(1)] private int _shakeVibrato = 12;

        public float SelectDuration => _selectDuration;
        public Ease SelectEase => _selectEase;
        public float SelectedWidth => _selectedWidth;
        public float IconRise => _iconRise;
        public float IconScale => _iconScale;
        public float ShakeDuration => _shakeDuration;
        public float ShakeStrength => _shakeStrength;
        public int ShakeVibrato => _shakeVibrato;
    }
}
