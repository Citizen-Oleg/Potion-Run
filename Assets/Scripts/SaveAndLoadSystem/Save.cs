using System.Collections.Generic;
using System.Linq;
using GapBetweenRunnerAndShooter.CraftSystem;
using Level_selection.Map_system;
using Newtonsoft.Json;
using Runner.ReagentSystem;
using UnityEngine;

namespace SaveAndLoadSystem
{
    public class Save
    {
        public void SaveIndexLocation(int index)
        {
            PlayerPrefs.SetInt(GlobalConstant.SAVE_CURRENT_LOCATION, index);
        }

        public void SaveReagent(List<ReagentType> reagentTypes)
        {
            var serializeReagent = JsonConvert.SerializeObject(reagentTypes);
            PlayerPrefs.SetString(GlobalConstant.SAVE_REAGENT, serializeReagent);
        }

        public void SaveRegion(List<Region> regions)
        {
            List<Region.RegionInformation> informations = new List<Region.RegionInformation>(regions.Count);
            informations.AddRange(regions.Select(region => region.Information));

            var serializeRegion = JsonConvert.SerializeObject(informations);
            PlayerPrefs.SetString(GlobalConstant.SAVE_LEVEL, serializeRegion);
        }

        public void SaveCurrentRegion(Region region)
        {
            PlayerPrefs.SetInt(GlobalConstant.SAVE_CURRENT_REGION, (int) region.Information.WorldType);
        }

        public void SaveOpenModelByWorldType(WorldType worldType, List<ModelType> bestiaryItems)
        {
            var serializeOpenModel = JsonConvert.SerializeObject(bestiaryItems);
            PlayerPrefs.SetString(GlobalConstant.SAVE_OPEN_ITEMS_TAB + worldType, serializeOpenModel);
        }
    }
}