using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GapBetweenRunnerAndShooter.CraftSystem
{
    public class ModelColorProvider
    {
        private readonly List<ModelColor> _reagentColors;
        private readonly List<ModelColor> _reagentParticleColors;
        private readonly List<ModelColor> _reagentExperimentalColors;
        
        public ModelColorProvider(Settings settings)
        {
            _reagentColors = settings.ModelColors;
            _reagentParticleColors = settings.ModelParticleColors;
            _reagentExperimentalColors = settings.ModelExperimentalColors;
            
            SortReagentType.Sort(_reagentColors);
            SortReagentType.Sort(_reagentParticleColors);
            SortReagentType.Sort(_reagentExperimentalColors);
        }

        public Color GetColorByReagentType(ModelType modelType)
        {
            foreach (var reagentColor in _reagentColors)
            {
                if (reagentColor.Model == modelType)
                {
                    return reagentColor.Color;
                }
            }

            return _reagentColors[0].Color;
        }

        public Color GetColorByReagentTypeToParticle(ModelType modelType)
        {
            foreach (var reagentColor in _reagentParticleColors)
            {
                if (reagentColor.Model == modelType)
                {
                    return reagentColor.Color;
                }
            }

            return _reagentParticleColors[0].Color;
        }
        
        public Color GetColorByReagentTypeToExperimental(ModelType modelType)
        {
            foreach (var reagentColor in _reagentExperimentalColors)
            {
                if (reagentColor.Model == modelType)
                {
                    return reagentColor.Color;
                }
            }

            return _reagentExperimentalColors[0].Color;
        }
        
        [Serializable]
        public class Settings
        {
            public List<ModelColor> ModelColors = new List<ModelColor>();
            public List<ModelColor> ModelParticleColors = new List<ModelColor>();
            public List<ModelColor> ModelExperimentalColors = new List<ModelColor>();
        }

        [Serializable]
        public class ModelColor
        {
            public ModelType Model;
            public Color Color;
        }

        private static class SortReagentType
        {
            public static void Sort(List<ModelColor> reagentColors)
            {
                for (var i = 1; i < reagentColors.Count; i++)
                {
                    for (var j = 0; j < reagentColors.Count - i; j++)
                    {
                        if ((int) reagentColors[j].Model >  (int) reagentColors[j + 1].Model)
                        {
                            (reagentColors[j], reagentColors[j + 1]) = (reagentColors[j + 1], reagentColors[j]);
                        }
                    }
                }
            }
        }
    }
}