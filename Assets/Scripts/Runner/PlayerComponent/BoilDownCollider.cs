using System;
using LiquidVolumeFX;
using UnityEngine;

namespace Runner.PlayerComponent
{
    public class BoilDownCollider : MonoBehaviour
    {
        [SerializeField]
        private LiquidVolume _liquidVolume;
        [SerializeField]
        private Transform _colliderDown;
        [SerializeField]
        private Transform _startPosition;
        [SerializeField]
        private Transform _endPosition;
        [SerializeField]
        private Collider _collider;

        private void Awake()
        {
            _collider.isTrigger = false;
        }

        private void OnEnable()
        {
            _collider.isTrigger = false;
        }

        public void SetTrigger()
        {
            _collider.isTrigger = true;
        }

        private void Update()
        {
            _colliderDown.position =
                Vector3.Lerp(_startPosition.position, _endPosition.position, _liquidVolume.level);
        }
    }
}