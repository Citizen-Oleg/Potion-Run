﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace FPS.EnemyComponent
{
    public class EnemyParticle : MonoBehaviour
    {
        [SerializeField]
        private Enemy _enemy;
        [SerializeField]
        private List<ParticleSystem> _blood = new List<ParticleSystem>();

        private void Start()
        {
            _enemy.OnDead += PlayerBloodParticle;
        }

        private void PlayerBloodParticle(Enemy enemy, Monster monster)
        {
            foreach (var system in _blood)
            {
                system.Play();
            }
        }

        private void OnEnable()
        {
            foreach (var system in _blood)
            {
                system.Stop();
            }
        }

        private void OnDestroy()
        {
            _enemy.OnDead -= PlayerBloodParticle;
        }
    }
}