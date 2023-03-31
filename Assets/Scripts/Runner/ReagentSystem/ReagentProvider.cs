using System;
using System.Collections.Generic;
using SaveAndLoadSystem;
using UnityEngine;

namespace Runner.ReagentSystem
{
    public class ReagentProvider
    {
        public List<ReagentType> GetAvailableReagent => _openReagent;
        public List<ReagentType> GetCloseReagent => _closeReagent;
        public ReagentType LastAdded { get; private set; } = ReagentType.None;
        
        private readonly List<ReagentType> _openReagent;
        private readonly List<ReagentType> _closeReagent = new List<ReagentType>();

        private readonly Save _save;

        public ReagentProvider(Settings settings, Save save, Load load)
        {
            var saveReagent = load.GetReagentTypes();
            _openReagent = saveReagent ?? settings.OpenReagent;
            _save = save;
            
            var allReagent = settings.AllReagent;
            foreach (var reagentType in allReagent)
            {
                if (!_openReagent.Contains(reagentType))
                {
                    _closeReagent.Add(reagentType);
                }
            }
        }

        public void AddReagent(ReagentType reagentType)
        {
            _openReagent.Add(reagentType);
            _closeReagent.Remove(reagentType);
            LastAdded = reagentType;
            _save.SaveReagent(_openReagent);
        }

        [Serializable]
        public class Settings
        {
            public List<ReagentType> OpenReagent = new List<ReagentType>();
            public List<ReagentType> AllReagent = new List<ReagentType>();
        }
    }
}