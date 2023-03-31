using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Level_selection.Map_system
{
    public class MapPlayer : MonoBehaviour
    {
        public bool IsMove { get; private set; } = false;

        [SerializeField]
        private NavMeshAgent _navMeshAgent;
        [SerializeField]
        private float _finishDistance;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private float _rotationTime;
        
        private MapPlayerAnimationController _mapPlayerAnimationController;
        private Transform _target;
        private Sequence _sequence;
        private Action _callback;

        private void Awake()
        {
            _mapPlayerAnimationController = new MapPlayerAnimationController(_animator);
        }

        public void MoveToPoint(Transform point, Action callback)
        {
            IsMove = true;
            _callback = callback;
            _target = point;
            _navMeshAgent.destination = _target.position;
        }

        public void TeleportToPoint(Transform point)
        {
            gameObject.SetActive(false);
            transform.position = point.position;
            transform.rotation = point.rotation;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            _mapPlayerAnimationController.Move(IsMove);
            
            if (!IsMove)
            {
                return;
            }

            if (IsPointReached(_target))
            {
                IsMove = false;
                _sequence = DOTween.Sequence();
                _sequence.Append(transform.DORotateQuaternion(_target.rotation, _rotationTime));
                _sequence.AppendCallback(() => _callback?.Invoke());
            }
        }
        
        private bool IsPointReached(Transform point)
        {
            if (_target == null)
            {
                return false;
            }
            
            var pointOne = new Vector3(point.position.x, point.position.z);
            var pointTwo = new Vector3(_navMeshAgent.transform.position.x, _navMeshAgent.transform.position.z);
            var distance = Vector3.Distance(pointOne, pointTwo);
            return distance <= _finishDistance;
        }
    }
}