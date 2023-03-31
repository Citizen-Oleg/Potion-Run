using System;
using System.Collections.Generic;
using Runner.PlayerComponent;
using UnityEngine;

namespace Runner
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField]
        private List<ObstacleCollider> _obstacleColliders = new List<ObstacleCollider>();
        
        private bool _isBlock;
        
        private void OnEnable()
        {
            _isBlock = false;
        }

        private void Awake()
        {
            foreach (var obstacleCollider in _obstacleColliders)
            {
                obstacleCollider.OnTrigger += ColliderHandler;
            }
        }

        private void ColliderHandler(Collider collider)
        {
            if (!_isBlock && collider.TryGetComponent(out BoilerCollider boiler))
            {
                _isBlock = true;
                boiler.Boiler.RemoveLastReagentType();
            }
        }

        private void OnDestroy()
        {
            foreach (var obstacleCollider in _obstacleColliders)
            {
                obstacleCollider.OnTrigger -= ColliderHandler;
            }
        }
    }
}