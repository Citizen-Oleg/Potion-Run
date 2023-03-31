using System;
using LiquidVolumeFX;
using UnityEngine;

namespace Runner.ReagentSystem
{
    public class Reagent : MonoBehaviour
    {
        public Rigidbody Rigidbody { get; private set; }
        public Vector3 StartLocalScale { get; private set; }

        public ReagentType ReagentType;

        private ReagentModelProvider _reagentModelProvider;

        private void Awake()
        {
            StartLocalScale = transform.localScale;
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            Rigidbody.constraints = RigidbodyConstraints.None;
        }

        public void Initialize(ReagentModelProvider reagentModelProvider)
        {
            _reagentModelProvider = reagentModelProvider;
        }

        public void ResetParent()
        {
            _reagentModelProvider.ResetParent(this);
        }

        public void LockYPosition()
        {
            Rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
        }
        
        public void Release()
        {
            _reagentModelProvider.ReleaseReagent(this);
        }
    }
}