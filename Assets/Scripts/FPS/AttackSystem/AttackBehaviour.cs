using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace FPS.AttackSystem
{
    public class AttackBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private Transform _muzzlePosition;
        [SerializeField]
        private Transform _parent;
        [SerializeField]
        private AttackAnimationController _attackAnimationController;
        [SerializeField]
        private FPSPlayer _fpsPlayer;

        [Inject]
        private ProjectileFactory _projectileFactory;

        private Projectile _projectile;

        private Vector2 _input;
        private bool _isActivate;

        private void OnEnable()
        {
            Reload();
            _isActivate = true;
        }

        private void OnDisable()
        {
            if (_projectile != null)
            {
                _projectileFactory.Release(_projectile);
            }
        }

        [UsedImplicitly]
        public void Deactivate()
        {
            _isActivate = false;
        }

        public void Reload()
        {
            _projectile = _projectileFactory.GetProjectile();
            _projectile.Rigidbody.isKinematic = true;
            _projectile.transform.parent = _parent;
            _projectile.transform.localPosition = _muzzlePosition.localPosition;
            _projectile.transform.localRotation = _muzzlePosition.localRotation;
            _projectile.ResetRotation();
        }

        private void Update()
        {
            if (_fpsPlayer.IsDead || !_isActivate)
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                _input = Input.mousePosition;

                if (_attackAnimationController.IsActiveAttack())
                {
                    _attackAnimationController.Attack();
                }
                else
                {
                    _attackAnimationController.SetBoolAttack(true);
                }
            }
        }
        
        [UsedImplicitly]
        public void Shoot()
        {
            var ray = _camera.ScreenPointToRay(_input);

            if (_projectile == null)
            {
                Reload();
            }

            _projectile.SetDirection(Physics.Raycast(ray, out var hit)
                ? Vector3.Normalize(hit.point - _muzzlePosition.position)
                : Vector3.Normalize(ray.GetPoint(100f) - _muzzlePosition.position));

            _projectileFactory.ResetParent(_projectile);
            _projectile = null;
        }
    }
}