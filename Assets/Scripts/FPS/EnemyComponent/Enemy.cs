﻿using System;
using Events;
using FPS.GenerationSystem;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.AI;

namespace FPS.EnemyComponent
{
    public class Enemy : MonoBehaviour
    {
        public StripPoint StripPoint { get; set; }
        public Monster CurrentMonster { private get; set; }
        public bool IsDead { get; private set; }

        public event Action<Enemy, Monster> OnDead;
        public Transform BottomPoint => _bottomPoint;
        public Transform FxPosition => _fxPosition;
        public Ragdoll Ragdoll => _ragdoll;

        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        
        public EnemyType EnemyType => _enemyType;
        
        [SerializeField]
        private EnemyType _enemyType;
        [SerializeField]
        private Ragdoll _ragdoll;
        [SerializeField]
        private Transform _bottomPoint;
        [SerializeField]
        private Transform _fxPosition;
        [SerializeField]
        private NavMeshAgent _navMeshAgent;
        [SerializeField]
        private EnemyAnimationController _enemyAnimationController;
        [SerializeField]
        private float _attackDistance;

        private EnemyPool _enemyPool;
        private FPSPlayer _target;
        private bool _isActivate;

        public void Dead()
        {
            IsDead = true;
            EventStreams.UserInterface.Publish(new EventEnemyDead(this));
            OnDead?.Invoke(this, CurrentMonster);

            if (_enemyPool == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                _enemyPool.ReleaseEnemy(this);
            }
        }

        public void PhysicsDead()
        {
            IsDead = true;
            OnDead?.Invoke(this, null);
        }

        public void Initialize(EnemyPool enemyPool)
        {
            _enemyPool = enemyPool;
        }

        public void MoveToTarget(FPSPlayer target)
        {
            _target = target;
            _isActivate = true;
        }

        private void Update()
        {
            if (!_isActivate || _target == null || _navMeshAgent.enabled == false)  
            {
                return;
            }

            if (_target.IsDead || Vector3.Distance(transform.position, _target.transform.position) < _attackDistance)
            {
                _target.Dead();
                _enemyAnimationController.ResetRun();
                _navMeshAgent.isStopped = true;
            }
            else
            {
                _navMeshAgent.isStopped = false;
                _navMeshAgent.destination = _target.transform.position;
                _enemyAnimationController.Run();
            }
        }

        private void OnEnable()
        {
            _isActivate = false;
            _target = null;
            IsDead = false;
        }
    }
}