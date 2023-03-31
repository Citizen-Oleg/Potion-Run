using System;
using FPS.EnemyComponent;
using LiquidVolumeFX;
using UnityEngine;

namespace FPS.AttackSystem
{
    public class Projectile : MonoBehaviour
    {
        public Rigidbody Rigidbody => _rigidbody;
        public LiquidVolume LiquidVolume => _liquidVolume;

        [SerializeField]
        private LiquidVolume _liquidVolume;
        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _lifeTime;
        [SerializeField]
        private Transform _rotationObject;
        [SerializeField]
        private float _speedRotation;

        private Vector3 _direction;
        private Rigidbody _rigidbody;
        private float _progress;

        private ProjectileFactory _projectileFactory;
        private bool _isActivate;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Initialize(ProjectileFactory projectileFactory)
        {
            _projectileFactory = projectileFactory;
            _progress = 0;
            _isActivate = false;
            ResetRotation();
        }

        public void ResetRotation()
        {
            _rotationObject.localRotation = Quaternion.identity;
        }

        public void SetDirection(Vector3 direction)
        {
            _rigidbody.isKinematic = false;
            _direction = direction;
            _isActivate = true;
        }

        private void Update()
        {
            if (!_isActivate)
            {
                return;
            }
            
            _progress += Time.deltaTime / _lifeTime;
            if (_progress >= 1f)
            {
                _projectileFactory.Release(this);
                _isActivate = false;
            }
        }

        private void FixedUpdate()
        {
            if (!_isActivate)
            {
                return;
            }
            
            _rotationObject.Rotate(Vector3.forward, -_speedRotation);
            _rigidbody.velocity = _direction * _speed;
            transform.right = _rigidbody.velocity;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActivate)
            {
                return;
            }
            
            if (other.transform.TryGetComponent(out Enemy enemy))
            {
                if (!enemy.IsDead)
                {
                    enemy.Dead();

                    _isActivate = false;
                    _rigidbody.isKinematic = true;
                    _projectileFactory.Release(this);
                }
            }
        }
    }
}