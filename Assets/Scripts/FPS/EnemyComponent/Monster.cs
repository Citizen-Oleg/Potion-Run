﻿using System;
using Events;
using FPS.GenerationSystem;
using JetBrains.Annotations;
using ParticleFactory;
using Tools.SimpleEventBus;
 using UnityEngine;
using UnityEngine.AI;

namespace FPS.EnemyComponent
{
    public class Monster : MonoBehaviour
    {
        public float AttackDistance => _attackDistance;
        public event Action<Monster> OnHide;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        public Transform FxPosition => _fxPosition == null ? transform : _fxPosition;
        
        [SerializeField]
        private Transform _fxPosition;
        [SerializeField]
        private float _attackDistance;
        [SerializeField]
        private EnemyAnimationController _enemyAnimationController;
        [SerializeField]
        private NavMeshAgent _navMeshAgent;
        [SerializeField]
        private Transform _positionHit;
        [SerializeField]
        private float _radiusHit;
        [SerializeField]
        private float _force;
        [SerializeField]
        private float _rotationSpeed = 2.5f;
        [SerializeField]
        private bool _isAttack = true;
    
        private StripPoint _stripPoint;
        private Enemy _currentAttackEnemy;
        private MonsterSpawner _monsterSpawner;
        private FPSPlayer _fpsPlayer;

        private bool _isActive;
        private float _angularSpeed;

        private void Awake()
        {
            _angularSpeed = _navMeshAgent.angularSpeed;
        }

        private void OnDisable()
        {
            _isActive = false;
            _enemyAnimationController.Reset();
            _navMeshAgent.enabled = true;
        } 

        public void Initialize(StripPoint stripPoint, MonsterSpawner monsterSpawner)
        {
            _fpsPlayer = stripPoint.FpsPlayer;
            _stripPoint = stripPoint;
            _monsterSpawner = monsterSpawner;

            _isActive = true;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.angularSpeed = _angularSpeed;
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            if (_stripPoint.Enemies.Count == 0 || _fpsPlayer.IsDead)
            {
                HideAnimation();
            }
            else if (_isAttack)
            {
                MoveToEnemy(NearestTargetProvider.GetNearestTarget(_stripPoint.Enemies, transform.position));
            }
        }

        [UsedImplicitly]
        public void Hide()
        {
            OnHide?.Invoke(this);
            _monsterSpawner.Release(this);
        }

        private void HideAnimation()
        {
            if (!_isAttack)
            {
                Hide();
                return;
            }
            
            _navMeshAgent.isStopped = true;
            if (!_enemyAnimationController.IsActiveHide())
            {
                _enemyAnimationController.Hide();
            }
        }

        private void MoveToEnemy(Enemy target)
        {
            var distance = Vector3.Distance(transform.position, target.transform.position);
          
            if (distance < _attackDistance)
            {
                _navMeshAgent.angularSpeed = 0;
                var direction = target.transform.position - transform.position;
                direction.y = 0f;
                var rotation = Quaternion.LookRotation(direction);
                _navMeshAgent.transform.rotation = Quaternion.Lerp(_navMeshAgent.transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
                
                if (!_enemyAnimationController.IsActiveAttack() || _currentAttackEnemy.Equals(target))
                {
                    _navMeshAgent.isStopped = true;
                    _currentAttackEnemy = target;
                    _enemyAnimationController.ResetRun();
                    _enemyAnimationController.Attack();
                }
                else
                {
                    _enemyAnimationController.ResetAttack();
                }
            }
            else
            {
                _navMeshAgent.angularSpeed = _angularSpeed;
                _navMeshAgent.isStopped = false;
                _navMeshAgent.destination = target.transform.position;
                _enemyAnimationController.Run();
                _enemyAnimationController.ResetAttack();
            }
        }

        [UsedImplicitly]
        public void HitTarget()
        {
            if (_currentAttackEnemy == null || _currentAttackEnemy.IsDead)
            {
                return;
            }
            
            _currentAttackEnemy.Ragdoll.ActivateRagdoll();

            var hitColliders = Physics.OverlapSphere(_positionHit.position, _radiusHit);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent(out Rigidbody rigidbody))
                {
                    rigidbody.AddForce(_force * _positionHit.forward, ForceMode.Impulse);
                }
            }
            
            _currentAttackEnemy.PhysicsDead();
            var eventShowParticle = new EventShowParticle
            {
                Transform = _positionHit, ParticleType = ParticleType.MonsterHit
            };
            EventStreams.UserInterface.Publish(eventShowParticle);
        }

        private void OnDrawGizmos()
        {
            if (_positionHit == null)
            {
                return;
            }
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_positionHit.position, _radiusHit);
        }
    }
}