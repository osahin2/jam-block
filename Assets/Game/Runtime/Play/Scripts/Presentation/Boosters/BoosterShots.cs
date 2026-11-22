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
        private BoardProjection _projection;
        private BoardPresenter _board;

        [Inject]
        public void Construct(BoosterMotionSettings motion, BoardProjection projection, BoardPresenter board)
        {
            _motion = motion;
            _projection = projection;
            _board = board;
        }

        public void Play(TargetedDefinition booster, Cell target, Action onLanded) =>
            ShotOf(booster).Play(_bar.ScreenPointOf(booster), target, onLanded);

        public void Stop()
        {
            foreach (var shot in _shots.Values)
                shot.Stop();
        }

        private BoosterShot ShotOf(TargetedDefinition booster)
        {
            if (_shots.TryGetValue(booster.Shot, out var shot))
                return shot;
            shot = Instantiate(booster.Shot, transform);
            shot.Init(_projection, _board, _motion);
            _shots.Add(booster.Shot, shot);
            return shot;
        }
    }
}
