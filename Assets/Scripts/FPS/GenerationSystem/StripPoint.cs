﻿using System;
using System.Collections.Generic;
using FPS.EnemyComponent;
using JetBrains.Annotations;
using UnityEngine;

namespace FPS.GenerationSystem
{
    public class StripPoint : MonoBehaviour
    {
        public event Action OnClear;

        public FPSPlayer FpsPlayer => _fpsPlayer;

        public bool IsCleared { get; private set; }
        public List<EnemyPoint> EnemyPoints => _enemyPosition;
        public List<Enemy> Enemies => _enemies;
        
        [SerializeField]
        private List<EnemyPoint> _enemyPosition = new List<EnemyPoint>();

        private FPSPlayer _fpsPlayer;
        private readonly List<Enemy> _enemies = new List<Enemy>();
        private readonly List<Monster> _activeMonster = new List<Monster>();

        public void AddEnemy(Enemy enemy)
        {
            _enemies.Add(enemy);
            enemy.StripPoint = this;
            enemy.OnDead += RemoveEnemy;
            IsCleared = false;
        }

        public void Initialize(FPSPlayer fpsPlayer)
        {
            _fpsPlayer = fpsPlayer;
        }
        
        [UsedImplicitly]
        public void ActivateEnemy()
        {
            if (_enemies.Count == 0)
            {
                IsCleared = true;
                OnClear?.Invoke();
                return;
            }
            
            foreach (var enemy in _enemies)
            {
                enemy.MoveToTarget(_fpsPlayer);
            }
        }

        private void RemoveEnemy(Enemy enemy, Monster monster)
        {
            enemy.OnDead -= RemoveEnemy;
            _enemies.Remove(enemy);

            if (monster != null)
            {
                _activeMonster.Add(monster);
                monster.OnHide += RemoveMonster;
            }
        }

        private void RemoveMonster(Monster monster)
        {
            _activeMonster.Remove(monster);
            monster.OnHide -= RemoveMonster;
            if (_activeMonster.Count == 0)
            {
                IsCleared = true;
                OnClear?.Invoke();
            }
        }

        private void OnDisable()
        {
            foreach (var monster in _activeMonster)
            {
                monster.OnHide -= RemoveMonster;
            }
            _activeMonster.Clear();
            
            foreach (var enemy in _enemies)
            {
                enemy.OnDead -= RemoveEnemy;
            }
            _enemies.Clear();

        }

        [Serializable]
        public class EnemyPoint
        {
            public Transform Point;
            public List<EnemyType> EnemyTypes = new List<EnemyType>();
        }
    }
}