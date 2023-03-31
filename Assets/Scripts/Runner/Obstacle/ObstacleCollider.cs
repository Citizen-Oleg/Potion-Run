using System;
using UnityEngine;

namespace Runner
{
    [RequireComponent(typeof(Collider))]
    public class ObstacleCollider : MonoBehaviour
    {
        public event Action<Collider> OnTrigger;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTrigger?.Invoke(other);
        }
    }
}