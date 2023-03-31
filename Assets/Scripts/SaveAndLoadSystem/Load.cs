using System.Collections.Generic;
using GapBetweenRunnerAndShooter.CraftSystem;
using Level_selection.Map_system;
using Newtonsoft.Json;
using Runner.ReagentSystem;
using UnityEngine;

namespace SaveAndLoadSystem
{
    public class Load
    {
        public bool HasInformationIndexLocation()
        {
            return PlayerPrefs.HasKey(GlobalConstant.SAVE_CURRENT_LOCATION);
        }
        
        public int GetIndexLocation()
        {
            return PlayerPrefs.GetInt(GlobalConstant.SAVE_CURRENT_LOCATION);
        }

        public List<Region.RegionInformation> GetLevel()
        {
            var regionSave = PlayerPrefs.GetString(GlobalConstant.SAVE_LEVEL);
            var region = string.IsNullOrEmpty(regionSave)
                ? null
                : JsonConvert.DeserializeObject<List<Region.RegionInformation>>(regionSave);
            return region;
        }

        public bool HasCurrentRegion()
        {
            return PlayerPrefs.HasKey(GlobalConstant.SAVE_CURRENT_REGION);
        }

        public WorldType GetCurrentWorld()
        {
            return (WorldType) PlayerPrefs.GetInt(GlobalConstant.SAVE_CURRENT_REGION);
        }

        public List<ReagentType> GetReagentTypes()
        {
            var reagentSave = PlayerPrefs.GetString(GlobalConstant.SAVE_REAGENT);
            var reagentTypes = string.IsNullOrEmpty(reagentSave)
                ? null
                : JsonConvert.DeserializeObject<List<ReagentType>>(reagentSave);
            return reagentTypes;
        }

        public List<ModelType> GetBestiaryItemByWorldType(WorldType worldType)
        {
            var itemsSave = PlayerPrefs.GetString(GlobalConstant.SAVE_OPEN_ITEMS_TAB + worldType);
            var bestiaryItem = string.IsNullOrEmpty(itemsSave)
                ? null
                : JsonConvert.DeserializeObject<List<ModelType>>(itemsSave);

            return bestiaryItem;
        }
    }
}