using System;
using System.Collections.Generic;
using Game.Logic.Boosters;
using UnityEngine;
using VContainer;

namespace Game.Play
{
    public sealed class BoosterBar : MonoBehaviour
    {
        [SerializeField] private BoosterSlot[] _slots;

        private PlaySession _session;
        private BoosterInventory _inventory;
        private IReadOnlyList<IBooster> _boosters;
        private Action<IBooster> _onPressed;
        private int _bound;

        [Inject]
        public void Construct(PlaySession session, BoosterInventory inventory, IReadOnlyList<IBooster> boosters)
        {
            _session = session;
            _inventory = inventory;
            _boosters = boosters;
            _bound = Mathf.Min(boosters.Count, _slots.Length);
            for (var i = 0; i < _slots.Length; i++)
            {
                _slots[i].gameObject.SetActive(i < _bound);
                if (i < _bound)
                    _slots[i].Bind(boosters[i], Press);
            }
        }

        private void Update()
        {
            for (var i = 0; i < _bound; i++)
            {
                var booster = _boosters[i];
                _slots[i].Refresh(_session.IsAvailable(booster), _inventory.Count(booster.Definition.Id));
            }
        }

        public void Show(Action<IBooster> onPressed) => _onPressed = onPressed;

        public RectTransform SlotOf(BoosterDefinition booster)
        {
            for (var i = 0; i < _bound; i++)
            {
                if (_slots[i].Definition == booster)
                    return _slots[i].Rect;
            }
            throw new ArgumentException($"No slot shows the booster '{booster.Id}'.", nameof(booster));
        }

        public Vector2 ScreenPointOf(BoosterDefinition booster) =>
            RectTransformUtility.WorldToScreenPoint(null, SlotOf(booster).position);

        private void Press(IBooster booster) => _onPressed?.Invoke(booster);
    }
}
