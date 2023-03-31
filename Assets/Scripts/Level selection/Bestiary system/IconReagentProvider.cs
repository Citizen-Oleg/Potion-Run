using System;
using System.Collections.Generic;
using System.Linq;
using GapBetweenRunnerAndShooter.CraftSystem;
using Runner.ReagentSystem;
using UnityEngine;

namespace Level_selection.Bestiary_system
{
    public class IconReagentProvider
    {
        private readonly CraftController _craftController;
        private readonly List<ReagentIcon> _reagentIcons;
        private readonly List<Sprite> _sprites = new List<Sprite>();
        
        public IconReagentProvider(Settings settings, CraftController craftController)
        {
            _reagentIcons = settings.ReagentIcons;
            _craftController = craftController;
        }

        public List<Sprite> GetSpriteByModelType(ModelType modelType)
        {
            _sprites.Clear();
            var recipe = _craftController.GetReagentRecipeByModelType(modelType);
            
            foreach (var reagentType in recipe)
            {
                _sprites.Add(_reagentIcons.FirstOrDefault(reagent => reagent.ReagentType == reagentType).Sprite);
            }
            
            return _sprites;
        }
        
        [Serializable]
        public class Settings
        {
            public List<ReagentIcon> ReagentIcons = new List<ReagentIcon>();
        }

        [Serializable]
        public class ReagentIcon
        {
            public ReagentType ReagentType;
            public Sprite Sprite;
        }
    }
}