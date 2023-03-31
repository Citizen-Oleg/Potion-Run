using System;
using Runner.PlayerComponent;
using Runner.ReagentSystem;
using UnityEngine;

namespace Runner.GateSystem
{
    public class GateReagent : Gate
    {
        public ReagentType ReagentType => _reagentType;
        
        [SerializeField]
        private ReagentType _reagentType;

        private Boiler _boiler;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out BoilerCollider boiler))
            {
                _boiler = boiler.Boiler;
                OnCollision();
            }
        }

        public override void Activate()
        {
            _boiler.AddReagents(_reagentType);
        }
    }
}