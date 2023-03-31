using System;
using System.Collections.Generic;
using System.Linq;
using Runner.ReagentSystem;
using UnityEngine;

namespace Level_selection.Map_system
{
    public class ColorRegionProvider
    {
        private readonly List<ColorRegion> _colorRegions;

        public ColorRegionProvider(Settings settings)
        {
            _colorRegions = settings.Regions;
        }

        public Color GetColorByRegionType(Region.RegionType regionType)
        {
            return _colorRegions.FirstOrDefault(region => region.RegionType == regionType).Color;
        }
        
        [Serializable]
        public class Settings
        {
            public List<ColorRegion> Regions = new List<ColorRegion>();
        }

        [Serializable]
        public class ColorRegion
        {
            public Color Color;
            public Region.RegionType RegionType;
        }
    }
}