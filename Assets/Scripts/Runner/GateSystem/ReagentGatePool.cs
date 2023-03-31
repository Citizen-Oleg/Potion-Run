using System;
using System.Collections.Generic;
using System.Linq;
using Base.SimpleEventBus_and_MonoPool;
using Events;
using Runner.ReagentSystem;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Runner.GateSystem
{
    public class ReagentGatePool : IDisposable
    {
        private Dictionary<ReagentType, DefaultMonoBehaviourPool<GateReagent>> _gatePool =
            new Dictionary<ReagentType, DefaultMonoBehaviourPool<GateReagent>>();

        private readonly IDisposable _subscription;
        
        public ReagentGatePool(Settings settings, Transform parent)
        {
            _subscription = EventStreams.UserInterface.Subscribe<EventReleaseRunner>(Release);
            foreach (var gate in settings.Gates)
            {
                var pool = new DefaultMonoBehaviourPool<GateReagent>(gate, parent, settings.PoolSize);
                _gatePool.Add(gate.ReagentType, pool);
            }
        }

        public GateReagent GetGateByReagentType(ReagentType reagentType)
        {
            var gate = _gatePool[reagentType].Take();
            return gate;
        }

        private void Release(EventReleaseRunner eventReleaseRunner)
        {
            foreach (var defaultMonoBehaviourPool in _gatePool)
            {
                defaultMonoBehaviourPool.Value.ReleaseAll();
            }
        }
        
        public void Dispose()
        {
            _subscription?.Dispose();
        }

        [Serializable]
        public class Settings
        {
            public int PoolSize;
            public List<GateReagent> Gates = new List<GateReagent>();
        }
    }
}