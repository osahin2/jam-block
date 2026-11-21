using UnityEngine;

namespace Game.Play
{
    public abstract class BoosterDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _title;
        [SerializeField] private string _description;
        [SerializeField, Min(0)] private int _startingStock = 2;
        [SerializeField, Min(1)] private int _packSize = 3;
        [SerializeField, Min(0)] private int _price = 100;

        public string Id => _id;
        public Sprite Icon => _icon;
        public string Title => _title;
        public string Description => _description;
        public int StartingStock => _startingStock;
        public int PackSize => _packSize;
        public int Price => _price;
    }
}
