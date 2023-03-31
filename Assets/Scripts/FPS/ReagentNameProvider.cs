using System;
using System.Collections.Generic;
using System.Linq;
using Runner.ReagentSystem;

namespace FPS
{
    public class ReagentNameProvider
    {
        private readonly List<NameReagent> _nameReagents;
        
        public ReagentNameProvider(Settings settings)
        {
            _nameReagents = settings.NameReagents;
        }

        public string GetNameByReagentType(ReagentType reagentType)
        {
            return _nameReagents.FirstOrDefault(reagent => reagent.ReagentType == reagentType).Name;
        }
        
        [Serializable]
        public class Settings
        {
            public List<NameReagent> NameReagents = new List<NameReagent>();
        }

        [Serializable]
        public class NameReagent
        {
            public string Name;
            public ReagentType ReagentType;
        }
    }
}