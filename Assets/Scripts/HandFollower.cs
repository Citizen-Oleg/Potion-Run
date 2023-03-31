using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class HandFollower : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private float _followSpeed;
        [SerializeField]
        private Vector3 _offSet;
        [SerializeField]
        private bool _isStatic;
        
        private void LateUpdate()
        {
            if (_target != null)
            {
                if (!_isStatic)
                {
                    transform.position = Vector3.Lerp(transform.position, _target.position + _offSet,
                        _followSpeed * Time.deltaTime);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            transform.position = _target.position + _offSet;
        }
    }
}