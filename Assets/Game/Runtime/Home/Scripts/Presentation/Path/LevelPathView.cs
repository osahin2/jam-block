using TMPro;
using UnityEngine;

namespace Game.Home
{
    public sealed class LevelPathView : MonoBehaviour
    {
        [Tooltip("The nodes' numbers from the current level upward.")]
        [SerializeField] private TMP_Text[] _numbers;

        public void Show(int currentLevel)
        {
            for (var i = 0; i < _numbers.Length; i++)
                _numbers[i].SetText("{0}", currentLevel + i);
        }
    }
}
