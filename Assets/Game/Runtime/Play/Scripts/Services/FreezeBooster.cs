using System;
using Game.Logic.Boosters;
using Game.Logic.Play;

namespace Game.Play
{
    public sealed class FreezeBooster : IBooster
    {
        private readonly FreezeDefinition _definition;
        private readonly BoosterInventory _inventory;

        public BoosterDefinition Definition => _definition;

        public FreezeBooster(FreezeDefinition definition, BoosterInventory inventory)
        {
            _definition = definition;
            _inventory = inventory;
        }

        public bool IsAvailable(LevelAttempt attempt) => attempt.CanFreeze;

        public void Use(BoosterContext context, Action onDone)
        {
            if (_inventory.Count(_definition.Id) > 0 && context.Attempt.Freeze(_definition.Seconds))
                _inventory.TryUse(_definition.Id);
            onDone();
        }

        public void Cancel()
        {
        }
    }
}
