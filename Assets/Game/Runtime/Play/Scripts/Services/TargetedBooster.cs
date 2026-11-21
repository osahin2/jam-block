using System;
using Game.Logic.Boosters;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    public sealed class TargetedBooster : IBooster
    {
        private readonly TargetedDefinition _definition;
        private readonly IBoardEffect _effect;
        private readonly BoosterInventory _inventory;
        private BoosterContext _context;
        private Action _onDone;

        public BoosterDefinition Definition => _definition;

        public TargetedBooster(TargetedDefinition definition, IBoardEffect effect, BoosterInventory inventory)
        {
            _definition = definition;
            _effect = effect;
            _inventory = inventory;
        }

        public bool IsAvailable(LevelAttempt attempt) => attempt.CanUseBoosters;

        public void Use(BoosterContext context, Action onDone)
        {
            _context = context;
            _onDone = onDone;
            context.Board.Release();
            if (_inventory.Count(_definition.Id) == 0 || !context.Attempt.Aim())
            {
                Finish();
                return;
            }
            context.Overlay.Show(_definition, context.Board.ScreenRect(), Cancel);
            context.Board.EnterPick(Pick);
        }

        public void Cancel()
        {
            if (_context == null)
                return;
            _context.Attempt.CancelAim();
            _context.Overlay.Hide();
            _context.Shots.Stop();
            _context.Board.EnterDrag();
            Finish();
        }

        private void Pick(Vector2 onBoard)
        {
            if (!_context.Attempt.TryApply(_effect, onBoard.x, onBoard.y))
            {
                Cancel();
                return;
            }
            _inventory.TryUse(_definition.Id);
            _context.Overlay.Hide();
            _context.Board.EnterBusy();
            _context.Shots.Play(_definition, LevelAttempt.CellAt(onBoard.x, onBoard.y), _context.Board, Land);
        }

        private void Land()
        {
            _context.Board.CatchUp();
            _context.Board.EnterDrag();
            Finish();
        }

        private void Finish()
        {
            var done = _onDone;
            _context = null;
            _onDone = null;
            done();
        }
    }
}
