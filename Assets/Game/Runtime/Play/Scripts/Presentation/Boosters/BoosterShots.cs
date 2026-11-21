using System;
using System.Collections.Generic;
using Game.Data.Levels;
using UnityEngine;
using VContainer;

namespace Game.Play
{
    public sealed class BoosterShots : MonoBehaviour
    {
        [SerializeField] private BoosterBar _bar;

        private readonly Dictionary<BoosterShot, BoosterShot> _shots = new Dictionary<BoosterShot, BoosterShot>();
        private BoosterMotionSettings _motion;

        [Inject]
        public void Construct(BoosterMotionSettings motion) => _motion = motion;

        public void Play(TargetedDefinition booster, Cell target, BoardInteraction board, Action onLanded)
        {
            if (!_shots.TryGetValue(booster.Shot, out var shot))
            {
                shot = Instantiate(booster.Shot, transform);
                _shots.Add(booster.Shot, shot);
            }
            shot.Play(_bar.ScreenPointOf(booster), target, board, _motion, onLanded);
        }

        public void Stop()
        {
            foreach (var shot in _shots.Values)
                shot.Stop();
        }
    }
}
