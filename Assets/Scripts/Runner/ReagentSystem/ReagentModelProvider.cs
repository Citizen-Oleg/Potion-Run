using System;
using System.Collections.Generic;
using Base;
using Base.SimpleEventBus_and_MonoPool;
using Tools.SimpleEventBus;
using UnityEngine;

namespace Runner.ReagentSystem
{
    public class ReagentModelProvider
    {
        private readonly Dictionary<ReagentType, DefaultMonoBehaviourPool<Reagent>> _objectPools = 
            new Dictionary<ReagentType,  DefaultMonoBehaviourPool<Reagent>>();
        
        public ReagentModelProvider(Settings settings, Transform parent)
        {
            foreach (var reagentModel in settings.ReagentModels)
            {
                var pool = new DefaultMonoBehaviourPool<Reagent>(reagentModel, parent, settings.PoolSize);
                _objectPools.Add(reagentModel.ReagentType, pool);
            }
        }

        public Reagent GetReagentModel(ReagentType reagentType)
        {
            var reagent = _objectPools[reagentType].Take();
            reagent.Initialize(this);
            return reagent;
        }

        public void ReleaseReagent(Reagent reagent)
        {
            _objectPools[reagent.ReagentType].Release(reagent);
        }

        public void ResetParent(Reagent reagent)
        {
            _objectPools[reagent.ReagentType].ResetParent(reagent);
        }

        [Serializable]
        public class Settings
        {
            public int PoolSize;
            public List<Reagent> ReagentModels = new List<Reagent>();
        }
    }
}